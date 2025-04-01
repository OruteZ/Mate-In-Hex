using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess
{
    public class MoveGenerator : MonoBehaviour
    {
        public List<Move> GetAvailableMoves(Board board, Piece piece)
        {
            List<Move> moves = new List<Move>();

            switch (piece.type)
            {
                case PieceType.Pawn:
                    moves = GetPawnMoves(board, piece).ToList();
                    break;
                case PieceType.Rook:
                    moves = GetRookMoves(board, piece).ToList();
                    break;
                case PieceType.Knight:
                    moves = GetKnightMoves(board, piece).ToList();
                    break;
                case PieceType.Bishop:
                    break;
                case PieceType.Queen:
                    break;
                case PieceType.King:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return moves;
        }

        private IEnumerable<Move> GetKnightMoves(Board board, Piece piece)
        {
            List<Move> moves = new List<Move>();
            

            return moves;
        }

        private IEnumerable<Move> GetPawnMoves(Board board, Piece piece)
        {
            List<Move> moves = new List<Move>();
            Hex startPos = piece.position;

            Hex direction, forwardMove, captureLeftMove, captureRightMove;

        
            // Move forward one tile
            if(piece.color is PieceColor.White){
                direction = Hex.Direction(HexDirection.N);
                forwardMove = startPos.Add(direction);
                captureLeftMove = startPos.Add(Hex.Direction(HexDirection.NW));
                captureRightMove = startPos.Add(Hex.Direction(HexDirection.NE));
            }

            // if black pawn, reverse direction
            else //if (piece.color is PieceColor.Black)
            {
                direction = Hex.Direction(HexDirection.S);
                forwardMove = startPos.Add(direction);
                captureLeftMove = startPos.Add(Hex.Direction(HexDirection.SW));
                captureRightMove = startPos.Add(Hex.Direction(HexDirection.SE));
            }

            // Check if the forward move is valid
            if (board.IsTileEmpty(forwardMove))
            {
                moves.Add(new Move
                {
                    color = piece.color,
                    pieceType = piece.type,
                    from = startPos,
                    to = forwardMove
                });
            }

            // check capture moves
            if (board.IsTileOccupiedByOpponent(captureLeftMove, piece.color))
            {
                moves.Add(new Move
                {
                    color = piece.color,
                    pieceType = piece.type,
                    from = startPos,
                    to = captureLeftMove
                });
            }
            if (board.IsTileOccupiedByOpponent(captureRightMove, piece.color))
            {
                moves.Add(new Move
                {
                    color = piece.color,
                    pieceType = piece.type,
                    from = startPos,
                    to = captureRightMove
                });
            }

            return moves;
        }

        private IEnumerable<Move> GetRookMoves(Board board, Piece piece)
        {
            List<Move> rookMoves;
            
            void RecursiveMove(Hex curPos, Hex direction)
            {
                Hex nextPos = curPos.Add(direction);
                
                // check if the next position is valid
                if (board.IsTileEmpty(nextPos))
                {
                    rookMoves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = curPos,
                        to = nextPos
                    });
                    
                    RecursiveMove(nextPos, direction);
                }

                else if (board.IsTileOccupiedByOpponent(nextPos, piece.color))
                {
                    rookMoves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = curPos,
                        to = nextPos
                    });
                }
            }
            
            rookMoves = new List<Move>();

            // get recursive move in every direction
            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {   
                // get the direction vector
                Hex dirVector = Hex.Direction(direction);
                
                // start recursive move from the current position
                RecursiveMove(piece.position, dirVector);
            }

            
            return rookMoves;
        }
    }
}