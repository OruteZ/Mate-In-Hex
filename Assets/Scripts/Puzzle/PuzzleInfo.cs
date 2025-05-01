using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu(fileName = "PuzzleInfo", menuName = "Puzzle/PuzzleInfo", order = 0)]
    public class PuzzleInfo : ScriptableObject
    {
        [Header("Puzzle Info")]
        public List<Hex> tileList;
        public List<Piece> pieces;
        public PieceType showPieceType = PieceType.Pawn;

        [Header("Solution")]
        [Tooltip("List of moves to solve the puzzle.")]
        public List<Move> solution;

        [ContextMenu("Create basic puzzle")]
        public void CreateBasicPuzzle()
        {
            tileList = Hex.GetHexMap(4).ToList();
            
            pieces = new List<Piece>() {
                new(PieceColor.White, PieceType.Pawn, new Hex(0, 0)),
                new(PieceColor.White, PieceType.Rook, new Hex(1, 0)),
            };
        }

        [ContextMenu("Remove null hex")]
        public void RemoveNullHex()
        {
            tileList.RemoveAll(hex => hex == Hex.NONE);
            pieces.RemoveAll(piece => piece.position == Hex.NONE);
        }

        public int GetMoveLimit() {
            // return white moves cnt
            return Mathf.Clamp(solution.Count(mov => mov.color == PieceColor.White), 1, 10000);
        }
    }
}