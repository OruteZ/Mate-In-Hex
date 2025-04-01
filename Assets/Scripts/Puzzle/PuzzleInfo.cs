using Chess;
using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu(fileName = "PuzzleInfo", menuName = "Puzzle/PuzzleInfo", order = 0)]
    public class PuzzleInfo : ScriptableObject
    {
        public Hex[] board;
        public Piece[] pieces;
        public Move[] solution;

        [ContextMenu("Create basic puzzle")]
        public void CreateBasicPuzzle()
        {
            //function cube_spiral(center, radius):
            // var results = list(center)
            // for each 1 ≤ k ≤ radius:
            // results = list_append(results, cube_ring(center, k))
            // return results
            board = Hex.GetHexMap(4).ToArray();
            
            pieces = new Piece[]
            {
                new Piece (PieceColor.White, PieceType.King, new Hex(0, 0)),
                new Piece (PieceColor.Black, PieceType.King, new Hex(1, 0)),
            };
        }
    }
}