using System.Collections.Generic;
using Puzzle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Chess
{
    [System.Serializable]
    public class Board : ScriptableObject
    {
        [SerializeField] private Hex[] tiles;
        [SerializeField] private Piece[] pieces;
        [SerializeField] private List<Move> moves;
        
        // getter / private setter
        public Hex[] Tiles => tiles;
        public Piece[] Pieces => pieces;
        public List<Move> Moves => moves;

        public void InitBoard(PuzzleInfo puzzleInfo)
        {
            tiles = puzzleInfo.board;
            pieces = puzzleInfo.pieces;
            moves ??= new List<Move>();
            moves.Clear();
        }
        
        /// <summary>
        /// color가 checkmate인지 확인
        /// </summary>
        /// <param name="attackColor"></param>
        /// <returns></returns>
        public bool IsCheckmate(PieceColor attackColor)
        {
            return false;
        }

        /// <summary>
        /// color가 opponent상대로 check를 걸었는지 확인
        /// </summary>
        /// <param name="attackColor"></param>
        /// <returns></returns>
        public bool IsCheck(PieceColor attackColor)
        {
            return false;
        }
        
        public void ApplyMove(Move move)
        {
            // 1. find target Piece that was moved
            // 2. move it to the new position
            foreach (Piece piece in pieces)
            {
                if (piece.position != move.from) continue;
                
                piece.position = move.to;
                break;
            }
        }
        
        public void UndoMove(Move move)
        {
            // 1. find target Piece that was moved
            // 2. move it back to the original position
            foreach (Piece piece in pieces)
            {
                if (piece.position != move.to) continue;
                
                piece.position = move.from;
                break;
            }
        }

        public bool IsTileEmpty(Hex position)
        {   
            foreach (Piece piece in pieces)
            {
                if (piece.position == position)
                {
                    return false;
                }
            }

            // check if the tile is empty
            foreach (Hex tile in tiles)
            {
                if (tile == position) return true;
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

        public bool IsEnPassantCapture(Vector2Int targetPos, Piece piece)
        {
            // if (piece.type != PieceType.Pawn) return false;
            // if (moves.Count == 0) return false;
            // Move lastMove = moves[^1];
            //
            // if (lastMove.pieceType is not PieceType.Pawn) return false;
            // if (lastMove.color == piece.color) return false;
            //
            // int moveLength = Mathf.Abs(lastMove.from.y - lastMove.to.y);
            // if (moveLength != 2) return false;
            //
            // Hex middlePos = new (targetPos.x, (lastMove.from.y + lastMove.to.y) / 2);
            // return lastMove.to == middlePos;
            
            return false;
        }
    }
}