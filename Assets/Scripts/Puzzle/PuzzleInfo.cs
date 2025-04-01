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
            board = Hex.GetHexMap(4).ToArray();
            
            pieces = new Piece[]
            {
                new(PieceColor.White, PieceType.King, new Hex(0, 0)),
                new(PieceColor.Black, PieceType.King, new Hex(1, 0)),
            };
        }
    }
}