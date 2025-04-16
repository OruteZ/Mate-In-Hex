using System;
using System.Collections.Generic;
using System.Linq;
using Puzzle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Chess
{
    [System.Serializable]
    public class Board : ScriptableObject
    {
        // 직렬화하면 null일수 없지않나?
        [SerializeField] private List<Hex> tiles;
        [SerializeField] private List<Piece> pieces;
        [SerializeField] private List<Move> moves;
        
        // getter / private setter
        public List<Hex> Tiles => tiles;
        public IEnumerable<Piece> Pieces => pieces;
        public List<Move> Moves => moves;

        public void InitBoard(PuzzleInfo puzzleInfo)
        {
            // 여기가 문제인가?
            tiles = new List<Hex>(puzzleInfo.tileList);
            //sort tiles
            tiles.Sort((a, b) => a.CompareTo(b));
            
            pieces = new List<Piece>(puzzleInfo.pieces.Select(piece => piece.Clone()));
            moves ??= new List<Move>();
            moves.Clear();
        }

        public void DeepCopyBoard(Board board)
        {
            tiles = new List<Hex>(board.tiles);
            tiles.Sort((a, b) => a.CompareTo(b));
            
            pieces = new List<Piece>(board.pieces.Select(piece => piece.Clone()));
            moves = new List<Move>(board.moves);
        }
        
        /// <summary>
        /// color가 checkmate인지 확인
        /// </summary>
        /// <param name="attackColor"></param>
        /// <returns></returns>
        public bool IsCheckmate(PieceColor attackColor)
        {
            if(!IsCheck(attackColor)) return false;

            // check opponent's movable count
            // opponent의 모든 piece에 대해
            // 1. piece가 이동할 수 있는지 확인
            // 2. 이동할 수 있는 경우, 이동 후 check인지 확인
            // 3. 이동할 수 있는 경우, 이동 후 check이 아닌 경우가 하나라도 있으면 false
            // 4. 이동할 수 있는 경우가 하나도 없으면 true
            
            int movableCount = 0;
            foreach (Piece piece in pieces)
            {
                if (piece.color == attackColor) continue;
                
                // check if the piece can move to any tile
                List<Move> moves = MoveGenerator.GetAvailableMoves(this, piece);
                movableCount += moves.Count;
            }

            return movableCount == 0;
        }

        /// <summary>
        /// color가 opponent상대로 check를 걸었는지 확인
        /// </summary>
        /// <param name="attackColor"></param>
        /// <returns></returns>
        public bool IsCheck(PieceColor attackColor)
        {
            // attack color에 해당하는 piece를 찾는다.
            foreach(Piece p in pieces) {
                if (p.color != attackColor) continue;

                // check if the piece is in attack range of opponent king
                List<Move> AtkMoves = MoveGenerator.GetAttacks(this, p);

                foreach (Move move in AtkMoves)
                {
                    Piece targetPiece = GetPieceAt(move.to);
                    if (targetPiece == null || targetPiece.type is not PieceType.King) continue;
                    
                    if (targetPiece.OpponentColor == attackColor)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public void ApplyMove(Move move)
        {
            // 1. find target Piece that was moved
            // 2. move it to the new position
            foreach (Piece piece in pieces)
            {
                if (piece.position != move.from) continue;
                if (piece.type != move.pieceType) continue;
                if (piece.color != move.color) continue;

                
                if(move.HasFlag(MoveFlag.Capture)) 
                {
                    // remove the captured piece from the board
                    pieces.Remove(GetPieceAt(move.to));
                }
                piece.position = move.to;

                break;
            }

            // 3. add move to the list of moves
            moves.Add(move);
        }
        
        public void UndoMove(Move move)
        {
            if (!Equals(move, moves.Last()))
            {
                Debug.LogError("UndoMove: move is not the last move");
                return;
            }
            
            // 1. find target Piece that was moved
            // 2. move it back to the original position
            foreach (Piece piece in pieces)
            {
                if (piece.position != move.to) continue;
                
                piece.position = move.from;

                if (move.HasFlag(MoveFlag.Capture)) 
                {
                    // add the captured piece back to the board
                    Piece capturedPiece = new(1 - move.color, move.capturedPieceType, move.to);
                    pieces.Add(capturedPiece);
                }

                break;
            }

            // 3. remove move from the list of moves
            moves.Remove(move);
        }

        public Piece GetPieceAt(Hex position)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.position == position)
                {
                    return piece;
                }
            }

            return null;
        }

        public bool IsTileAvailable(Hex position)
        {   
            foreach (Piece piece in pieces)
            {
                if (piece.position == position)
                {
                    return false;
                }
            }

            // check if the tile is empty
            // use binary search
            // int idx = tiles.BinarySearch(position);
            // if (idx < 0)
            // {
            //     // tile is not in the list
            //     return false;
            // }
            
            // use linear search
            foreach (Hex tile in tiles)
            {
                if (tile == position)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsTileOccupiedByOpponent(Hex targetPos, PieceColor pieceColor)
        {
            foreach (Piece piece in pieces)
            {
                if (piece.position == targetPos && piece.color != pieceColor)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsEnPassantCapture(Hex targetPos, Piece piece)
        {
            if (piece.type != PieceType.Pawn) return false;
            if (moves.Count == 0) return false;
            Move lastMove = moves[^1];
            
            if (lastMove.pieceType is not PieceType.Pawn) return false;
            if (lastMove.color == piece.color) return false;
            
            Hex movVector = lastMove.to - lastMove.from;
            if (movVector.Length() != 2) return false;
            
            Hex middlePos = lastMove.from + movVector.Scale(0.5f);
            return targetPos == middlePos;
        }

        public bool IsAttackableRelation(Piece atk, Piece def) 
        {
            // check if the target piece is in the attack range of the attacker
            if (atk.color == def.color) return false;
            
            Hex movVector = def.position - atk.position;

            bool diagonal = movVector.IsDiagonalVector();
            bool straight = movVector.IsStraightVector();

            return atk.type switch
            {
                // check if the target piece is in the attack range of the attacker
                PieceType.Knight => true,
                PieceType.Bishop => diagonal,
                PieceType.Rook => straight,
                PieceType.Queen => diagonal || straight,
                PieceType.King => (diagonal || straight) && movVector.Length() == 1,
                // check if the target piece is in the attack range of the attacker
                PieceType.Pawn when atk.color == PieceColor.White => movVector.Equals(Hex.Direction(HexDirection.NE)) ||
                                                                     movVector.Equals(Hex.Direction(HexDirection.NW)),
                PieceType.Pawn => movVector.Equals(Hex.Direction(HexDirection.SE)) ||
                                  movVector.Equals(Hex.Direction(HexDirection.SW)),
                _ => false
            };
        }
    }
}