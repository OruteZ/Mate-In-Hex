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
            List<Move> moves = new();

            moves = piece.type switch
            {
                PieceType.Pawn => GetPawnMoves(board, piece).ToList(),
                PieceType.Rook => GetRookMoves(board, piece).ToList(),
                PieceType.Knight => GetKnightMoves(board, piece).ToList(),
                PieceType.Bishop => GetBishopMoves(board, piece).ToList(),
                PieceType.Queen => GetQueenMoves(board, piece).ToList(),
                PieceType.King => GetKingMoves(board, piece).ToList(),
                _ => throw new ArgumentOutOfRangeException("Invalid piece type : " + piece.type),
            };

            
            return moves;
        }

        private IEnumerable<Move> GetKingMoves(Board board, Piece piece) {
            List<Move> moves = new();
            
            // get all 6 directions
            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                Hex nextPos = piece.position.Add(Hex.Direction(direction));
                
                if (board.IsTileEmpty(nextPos))
                {
                    moves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos
                    });
                }

                if(board.IsTileOccupiedByOpponent(nextPos, piece.color))
                {
                    var captureMove = new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos,
                        flags = MoveFlag.Capture
                    };
                }
            }

            //...and all 6 diagonal directions
            foreach (HexDiagonalDirection direction in Enum.GetValues(typeof(HexDiagonalDirection)))
            {
                Hex nextPos = piece.position.Add(Hex.diagonals[(int)direction]);
                
                if (board.IsTileEmpty(nextPos))
                {
                    moves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos
                    });
                }

                if(board.IsTileOccupiedByOpponent(nextPos, piece.color))
                {
                    var captureMove = new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos,
                        flags = MoveFlag.Capture
                    };
                }
            }

            // todo : check for castling
            return moves;
        }

        private IEnumerable<Move> GetQueenMoves(Board board, Piece piece) {
            List<Move> moves = new();
            
            // get rook moves
            moves.AddRange(GetRookMoves(board, piece));
            
            // get bishop moves
            moves.AddRange(GetBishopMoves(board, piece));
            
            return moves;
        }

        private IEnumerable<Move> GetBishopMoves(Board board, Piece piece)
        {
            List<Move> bishopMoves = new();

            void RecursiveMove(Hex curPos, Hex direction)
            {
                Hex nextPos = curPos.Add(direction);
                
                if (board.IsTileEmpty(nextPos))
                {
                    bishopMoves.Add(new Move
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
                    bishopMoves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = curPos,
                        to = nextPos,
                        flags = MoveFlag.Capture
                    });
                }
            }
            
            // Use only the diagonal directions from Hex.diagonals
            foreach (Hex diagonal in Hex.diagonals)
            {
                RecursiveMove(piece.position, diagonal);
            }
            
            return bishopMoves;
        }

        private IEnumerable<Move> GetKnightMoves(Board board, Piece piece)
        {
            List<Move> moves = new();
            List<Hex> targetDirections = new()
            {
                new Hex(1, 2, -3),
                new Hex(2, 1, -3),
                new Hex(3, -1, -2),
                new Hex(3, -2, -1),
                new Hex(2, -3, 1),
                new Hex(1, -3, 2),
                new Hex(-1, -2, 3),
                new Hex(-2, -1, 3),
                new Hex(-3, 1, 2),
                new Hex(-3, 2, 1),
                new Hex(-2, 3, -1),
                new Hex(-1, 3, -2)
            };

            // Loop through all target directions
            foreach (Hex targetDirection in targetDirections)
            {
                Hex nextPos = piece.position.Add(targetDirection);
                
                // Check if the target position is valid
                if (board.IsTileEmpty(nextPos))
                {
                    moves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos
                    });
                }
                else if (board.IsTileOccupiedByOpponent(nextPos, piece.color))
                {
                    moves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos,
                        flags = MoveFlag.Capture
                    });
                }
            }
            
            return moves;
        }

        private IEnumerable<Move> GetPawnMoves(Board board, Piece piece)
        {
            List<Move> moves = new();
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