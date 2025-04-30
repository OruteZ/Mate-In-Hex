using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Chess;

public static class AIController
{
    public static Move GetAnyMove(Board board, PieceColor color)
    {
        // Implement AI logic to get any move for the given color
        // This is a placeholder for the actual AI logic
        IEnumerable<Piece> pieces = board.Pieces.Where(p => p.color == color);
        
        List<Move> moves = new List<Move>();
        foreach (Piece piece in pieces)
        {
            // Generate possible moves for each piece
            List<Move> possibleMoves = MoveGenerator.GetAvailableMoves(board, piece);
            moves.AddRange(possibleMoves);
        }

        // Return the first available move
        return moves.Count > 0 ? moves[0] : Move.NONE;
    }
}