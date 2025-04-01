// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace Chess
// {
//     public class MoveGenerator : MonoBehaviour
//     {
//         public List<Move> GetAvailableMoves(Board board, Piece piece)
//         {
//             List<Move> moves = new List<Move>();
//
//             switch (piece.type)
//             {
//                 case PieceType.Pawn:
//                     moves = GetPawnMoves(board, piece).ToList();
//                     break;
//                 case PieceType.Rook:
//                     moves = GetRookMoves(board, piece).ToList();
//                     break;
//                 case PieceType.Knight:
//                     moves = GetKnightMoves(board, piece).ToList();
//                     break;
//                 case PieceType.Bishop:
//                     break;
//                 case PieceType.Queen:
//                     break;
//                 case PieceType.King:
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//
//             return moves;
//         }
//
//         private IEnumerable<Move> GetKnightMoves(Board board, Piece piece)
//         {
//             List<Move> moves = new List<Move>();
//             Vector2Int startPos = piece.position;
//             Vector2Int[] knightMoves =
//             {
//                 new Vector2Int(startPos.x + 1, startPos.y + 2),
//                 new Vector2Int(startPos.x + 2, startPos.y + 1),
//                 new Vector2Int(startPos.x + 2, startPos.y - 1),
//                 new Vector2Int(startPos.x + 1, startPos.y - 2),
//                 new Vector2Int(startPos.x - 1, startPos.y - 2),
//                 new Vector2Int(startPos.x - 2, startPos.y - 1),
//                 new Vector2Int(startPos.x - 2, startPos.y + 1),
//                 new Vector2Int(startPos.x - 1, startPos.y + 2)
//             };
//
//             foreach (Vector2Int knightMove in knightMoves)
//             {
//                 if (board.IsTileEmpty(knightMove) || board.IsTileOccupiedByOpponent(knightMove, piece.color))
//                 {
//                     moves.Add(new Move
//                     {
//                         color = piece.color,
//                         pieceType = piece.type,
//                         from = startPos,
//                         to = knightMove
//                     });
//                 }
//             }
//
//             return moves;
//         }
//
//         private IEnumerable<Move> GetPawnMoves(Board board, Piece piece)
//         {
//             List<Move> moves = new List<Move>();
//             int direction = piece.color == PieceColor.White ? 1 : -1;
//             Hex startPos = piece.position;
//
//             // One step forward
//             Vector2Int oneStepForward = new Vector2Int(startPos.x, startPos.y + direction);
//             if (board.IsTileEmpty(oneStepForward))
//             {
//                 moves.Add(new Move
//                 {
//                     color = piece.color,
//                     pieceType = piece.type,
//                     from = startPos,
//                     to = oneStepForward
//                 });
//             }
//
//             // Two steps forward from starting position
//             if ((piece.color == PieceColor.White && startPos.y == 1) ||
//                 (piece.color == PieceColor.Black && startPos.y == 6))
//             {
//                 Vector2Int twoStepsForward = new Vector2Int(startPos.x, startPos.y + 2 * direction);
//                 if (board.IsTileEmpty(twoStepsForward) && board.IsTileEmpty(oneStepForward))
//                 {
//                     moves.Add(new Move
//                     {
//                         color = piece.color,
//                         pieceType = piece.type,
//                         from = startPos,
//                         to = twoStepsForward
//                     });
//                 }
//             }
//
//             // Diagonal captures
//             Vector2Int[] diagonalMoves =
//             {
//                 new Vector2Int(startPos.x - 1, startPos.y + direction),
//                 new Vector2Int(startPos.x + 1, startPos.y + direction)
//             };
//
//             foreach (var diagonalMove in diagonalMoves)
//             {
//                 if (board.IsTileOccupiedByOpponent(diagonalMove, piece.color))
//                 {
//                     moves.Add(new Move
//                     {
//                         color = piece.color,
//                         pieceType = piece.type,
//                         from = startPos,
//                         to = diagonalMove
//                     });
//                 }
//             }
//
//             // En passant captures (assuming board has a method to check for en passant)
//             foreach (Vector2Int diagonalMove in diagonalMoves)
//             {
//                 if (board.IsEnPassantCapture(diagonalMove, piece))
//                 {
//                     moves.Add(new Move
//                     {
//                         color = piece.color,
//                         pieceType = piece.type,
//                         from = startPos,
//                         to = diagonalMove
//                     });
//                 }
//             }
//
//             return moves;
//         }
//
//         private IEnumerable<Move> GetRookMoves(Board board, Piece piece)
//         {
//             List<Move> rookMoves;
//             
//             void RecursiveMove(Vector2Int direction)
//             {
//                 Vector2Int nextPos = piece.position + direction;
//                 while (board.IsTileEmpty(nextPos))
//                 {
//                     rookMoves.Add(new Move
//                     {
//                         color = piece.color,
//                         pieceType = piece.type,
//                         from = piece.position,
//                         to = nextPos
//                     });
//                     nextPos += direction;
//                 }
//
//                 if (board.IsTileOccupiedByOpponent(nextPos, piece.color))
//                 {
//                     rookMoves.Add(new Move
//                     {
//                         color = piece.color,
//                         pieceType = piece.type,
//                         from = piece.position,
//                         to = nextPos
//                     });
//                 }
//             }
//             
//             rookMoves = new List<Move>();
//             RecursiveMove(Vector2Int.up);
//             RecursiveMove(Vector2Int.down);
//             RecursiveMove(Vector2Int.left);
//             RecursiveMove(Vector2Int.right);
//             
//             return rookMoves;
//         }
//     }
// }