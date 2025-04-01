using Chess;
using UnityEngine;

namespace Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        public PuzzleInfo puzzleInfo;
        
        [SerializeField]
        private BoardView boardView;
        
        [SerializeField]
        private Board board;
        
        private void Start()
        {
            // check is puzzleInfo is null
            if (puzzleInfo == null)
            {
                throw new System.NullReferenceException("puzzleInfo is null");
            }
            
            // init board
            board = ScriptableObject.CreateInstance<Board>();
            board.InitBoard(puzzleInfo);
            
            // create board view
            boardView.CreateBoardView(board);
        }
        
        
        [ContextMenu("Refresh Board View")]
        public void RefreshBoardView()
        {
            boardView.RefreshBoardView(board); 
        }
    }
}