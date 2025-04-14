using System.Collections.Generic;
using Chess;
using UnityEngine;
using UnityEngine.Events;

namespace Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        [Header("Puzzle Info")]
        public PuzzleInfo puzzleInfo;
        public bool turnBased = false;
        
        [Header("References")]
        [SerializeField] private BoardView boardView;
        [SerializeField] private Board board;

        [Header("About Moves")]
        [SerializeField] private MovableView movableView;
        [SerializeField] private ControlState controlState = ControlState.Ready;
        [SerializeField] private List<Move> curMovable = new ();
        [SerializeField] private UnityEvent onMoveFinished;
        
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
            switch(controlState)
            {
                case ControlState.NotControllable:
                    // do nothing
                    break;
                case ControlState.Ready:
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A))
                    {
                        Piece clickedPiece = GetClickedPiece();
                        if (clickedPiece != null)
                        {
                            SelectPiece(clickedPiece);
                            controlState = ControlState.SelectPiece;
                        }
                        else Debug.Log("Clicked on empty tile or no piece found.");
                    }
                    break;

                case ControlState.SelectPiece:
                    if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.S)) 
                    {
                        // cancel selection
                        movableView.HideMovable();
                        controlState = ControlState.Ready;
                    }

                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A)) 
                    {
                        Hex clickedPos = GetClickedHex();
                        if (clickedPos != Hex.NONE) 
                        {
                            // check if the clicked tile is in the movable range
                            foreach (Move move in curMovable) 
                            {
                                if (move.to.Equals(clickedPos)) 
                                {
                                    // apply the move
                                    board.ApplyMove(move);
                                    
                                    // refresh the board view
                                    boardView.RefreshBoardView(board, true); 
                                    
                                    // hide movable range
                                    movableView.HideMovable();
                                    
                                    // reset control state
                                    controlState = ControlState.Ready;
                                    if (turnBased) 
                                    {
                                        // wait for player to click again
                                        controlState = ControlState.NotControllable;
                                    }


                                    // debug : show check
                                    if(board.IsCheckmate(move.color)) 
                                    {
                                        Debug.Log($"{move.color} is checkmate after move {move}");
                                    }

                                    break;
                                }
                            }
                        }
                        else Debug.Log($"Clicked on empty tile {clickedPos}");
                    }
                    break;

                default:
                    break;
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
            LayerMask layerMask = LayerMask.GetMask("ChessPiece");
            Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, Mathf.Infinity, layerMask);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<PieceView>(out var pieceView))
                {
                    return pieceView.piece;
                }
                else Debug.LogError("Hit collider does not have PieceView component.");
            }

            return null;
        }

        public Hex GetClickedHex()
        {
            // Alternative method: directly convert mouse position from screen to world coordinates
            Vector2 screenPos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            Hex result = Hex.GetHexFromPixel(worldPos);
            Debug.Log($"Clicked on tile {result} at {worldPos}");
            return result;
        }
    
        public void SelectPiece(Piece p) 
        {
            // test : show movable range
            Debug.Log($"Clicked on {p.color} {p.type} at {p.position}");

            // show movable tiles
            curMovable = MoveGenerator.GetAvailableMoves(board, p);
            Debug.Log($"Available moves for {p.color} {p.type} at {p.position}: {curMovable.Count} moves found.");
            foreach (Move move in curMovable) 
            {
                Debug.Log($"Move: {move.from} -> {move.to}");
            }

            movableView.ShowMovable(curMovable.ConvertAll(move => move.to));
        }
    }

    [System.Serializable]
    enum ControlState
    {
        Ready,
        SelectPiece,
        NotControllable,
    }
}