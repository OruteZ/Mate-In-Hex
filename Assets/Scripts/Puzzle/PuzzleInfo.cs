using System.Collections.Generic;
using System.Linq;
using Chess;
using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu(fileName = "PuzzleInfo", menuName = "Puzzle/PuzzleInfo", order = 0)]
    public class PuzzleInfo : ScriptableObject
    {
        public List<Hex> board;
        public List<Piece> pieces;
        public List<Move> solution;

        [ContextMenu("Create basic puzzle")]
        public void CreateBasicPuzzle()
        {
            board = Hex.GetHexMap(4).ToList();
            
            pieces = new List<Piece>() {
                new(PieceColor.White, PieceType.Pawn, new Hex(0, 0)),
                new(PieceColor.White, PieceType.Rook, new Hex(1, 0)),
            };
        }
    }
}