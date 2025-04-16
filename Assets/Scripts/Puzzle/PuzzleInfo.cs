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
    }
}