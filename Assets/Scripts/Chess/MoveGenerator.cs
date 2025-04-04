using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess
{
    public static class MoveGenerator
    {
        // 해당 이동이 정말 유효한지를 시뮬레이션 하기 위해서 사용하는 보드입니다. 실제 게임 보드와는 다릅니다.
        private static Board moveResultingBoard = ScriptableObject.CreateInstance<Board>();

        private static List<Move> GetMoves(Board board, Piece piece)
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

            for (int i = 0; i < moves.Count; i++)
            {
                if (moves[i].HasFlag(MoveFlag.Capture))
                {
                    // save the captured piece
                    Piece captured = board.GetPieceAt(moves[i].to);
                    if (captured != null)
                    {
                        Move move = moves[i];
                        move.capturedPieceType = captured.type;
                        moves[i] = move;
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// 현재 이 기물이 공격중인 기물을 확인합니다. 체크에 사용하기 때문에 Pin같은 사유를 고려하지 않습니다.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static List<Move> GetAttacks(Board board, Piece piece)
        {
            List<Move> moves = GetMoves(board, piece);
            return moves.Where(m => m.flags.HasFlag(MoveFlag.Capture)).ToList();
        }

        public static List<Move> GetAvailableMoves(Board board, Piece piece)
        {
            List<Move> moves = GetMoves(board, piece);
            List<Move> availableMoves = new List<Move>(moves.Count);

            // 이동후 check를 당하는지 확인
            moveResultingBoard.DeepCopyBoard(board);

            foreach (Move move in moves)
            {
                // 이동
                moveResultingBoard.ApplyMove(move);
                
                if (moveResultingBoard.IsCheck(piece.OpponentColor) is false) availableMoves.Add(move);
                moveResultingBoard.UndoMove(move);
            }

            return availableMoves;
        }

        private static IEnumerable<Move> GetKingMoves(Board board, Piece piece) 
        {
            List<Move> moves = new();
            
            // get all 6 directions
            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                Hex nextPos = piece.position.Add(Hex.Direction(direction));
                
                if (board.IsTileAvailable(nextPos))
                {
                    moves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos
                    });
                }

                if (board.IsTileOccupiedByOpponent(nextPos, piece.color))
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
                
                if (board.IsTileAvailable(nextPos))
                {
                    moves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
                        to = nextPos
                    });
                }

                if (board.IsTileOccupiedByOpponent(nextPos, piece.color))
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

        private static IEnumerable<Move> GetQueenMoves(Board board, Piece piece) 
        {
            List<Move> moves = new();
            
            // get rook moves
            moves.AddRange(GetRookMoves(board, piece));
            
            // get bishop moves
            moves.AddRange(GetBishopMoves(board, piece));
            
            return moves;
        }

        private static IEnumerable<Move> GetBishopMoves(Board board, Piece piece)
        {
            List<Move> bishopMoves = new();

            void RecursiveMove(Hex curPos, Hex direction)
            {
                Hex nextPos = curPos.Add(direction);
                
                if (board.IsTileAvailable(nextPos))
                {
                    bishopMoves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
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
                        from = piece.position,
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

        private static IEnumerable<Move> GetKnightMoves(Board board, Piece piece)
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
                if (board.IsTileAvailable(nextPos))
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

        private static IEnumerable<Move> GetPawnMoves(Board board, Piece piece)
        {
            List<Move> moves = new();
            Hex startPos = piece.position;

            Hex direction, forwardMove, captureLeftMove, captureRightMove;

            // Move forward one tile
            if(piece.color is PieceColor.White)
            {
                direction = Hex.Direction(HexDirection.N);
                forwardMove = startPos.Add(direction);
                captureLeftMove = startPos.Add(Hex.Direction(HexDirection.NW));
                captureRightMove = startPos.Add(Hex.Direction(HexDirection.NE));
            }
            else
            {
                direction = Hex.Direction(HexDirection.S);
                forwardMove = startPos.Add(direction);
                captureLeftMove = startPos.Add(Hex.Direction(HexDirection.SW));
                captureRightMove = startPos.Add(Hex.Direction(HexDirection.SE));
            }

            // Check if the forward move is valid
            if (board.IsTileAvailable(forwardMove))
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
                    to = captureLeftMove,
                    flags = MoveFlag.Capture
                });
            }
            if (board.IsTileOccupiedByOpponent(captureRightMove, piece.color))
            {
                moves.Add(new Move
                {
                    color = piece.color,
                    pieceType = piece.type,
                    from = startPos,
                    to = captureRightMove,
                    flags = MoveFlag.Capture
                });
            }

            return moves;
        }

        private static IEnumerable<Move> GetRookMoves(Board board, Piece piece)
        {
            List<Move> rookMoves = new();
            
            void RecursiveMove(Hex curPos, Hex direction)
            {
                Hex nextPos = curPos.Add(direction);
                
                // check if the next position is valid
                if (board.IsTileAvailable(nextPos))
                {
                    rookMoves.Add(new Move
                    {
                        color = piece.color,
                        pieceType = piece.type,
                        from = piece.position,
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
                        from = piece.position,
                        to = nextPos,
                        flags = MoveFlag.Capture
                    });
                }
            }
            
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
