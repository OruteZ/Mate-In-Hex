using System.Collections.Generic;
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

        [Header("About Moves")]
        [SerializeField]
        private MoveGenerator moveGenerator;
        [SerializeField] private MovableView movableView;
        
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

        private void Update() {
            // check if mouse is clicked
            if (Input.GetMouseButtonDown(0))
            {
                Piece clickedPiece = GetClickedPiece();
                if (clickedPiece != null)
                {
                    OnClickPiece(clickedPiece);
                }
                else {
                    Debug.Log("Clicked on empty tile"); 
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                movableView.HideMovable();
            }
        }
        
        
        [ContextMenu("Refresh Board View")]
        public void RefreshBoardView()
        {
            boardView.RefreshBoardView(board); 
        }

        public Piece GetClickedPiece()
        {
            // Raycast to get the clicked piece : has Layer "ChessPiece"
        
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask layerMask = LayerMask.GetMask("ChessPiece");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // check if the clicked object has a PieceView component
                PieceView pieceView = hit.transform.GetComponent<PieceView>();
                if (pieceView != null)
                {
                    return pieceView.piece;
                }
            }

            return null;
        }
    
        public void OnClickPiece(Piece p) {
            // test : show movable range
            Debug.Log($"Clicked on {p.color} {p.type} at {p.position}");

            // show movable tiles
            List<Move> movaleableTiles = moveGenerator.GetAvailableMoves(board, p);

            movableView.ShowMovable(movaleableTiles.ConvertAll(move => move.to));
        }
    }
}