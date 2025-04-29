using System.Collections.Generic;
using System.Linq;
using Chess;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        [SerializeField] private Transform finishCanvas;

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
                puzzleInfo = GameManager.Instance.CurSelectedPuzzleInfo;
            }
            
            // init board
            board = ScriptableObject.CreateInstance<Board>();
            board.InitBoard(puzzleInfo);
            
            // create board view
            boardView.RefreshBoardView(board);
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
                                    
                                    if(board.IsCheckmate(move.color)) 
                                    {
                                        Debug.Log($"{move.color} is checkmate after move {move}");

                                        board.Moves.Last().SetFlag(MoveFlag.Checkmate);
                                        Invoke(nameof(ShowFinishCanvas), 1);
                                    }
                                    
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


                                    

                                    break;
                                }
                            }
                        }
                        else Debug.Log($"Clicked on empty tile {clickedPos}");
                    }
                    break;
                case ControlState.ReadyToFinish:
                    // press any button to move scene "Selecting Level"
                    if (Input.anyKeyDown)
                    {
                        SceneManager.LoadScene("Selecting Level");
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
                if (hit.collider.TryGetComponent(out PieceView pieceView))
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
            return result;
        }
    
        public void SelectPiece(Piece p) 
        {
            // test : show movable range
            // show movable tiles
            curMovable = MoveGenerator.GetAvailableMoves(board, p);
        

            movableView.ShowMovable(curMovable.ConvertAll(move => move.to));
        }
        
        private void ShowFinishCanvas()
        {
            // Ensure the finishCanvas is active
            finishCanvas.gameObject.SetActive(true);

            // Get or add a CanvasGroup component
            CanvasGroup canvasGroup = finishCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = finishCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            // Set initial alpha to 0
            canvasGroup.alpha = 0;

            // Animate the alpha to 1
            LeanTween.alphaCanvas(canvasGroup, 1f, 0.5f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    Debug.Log("Finish canvas fade-in animation completed.");
                    // Additional actions after animation (if needed)
                    controlState = ControlState.ReadyToFinish;
                });
        }
    }

    [System.Serializable]
    enum ControlState
    {
        Ready,
        SelectPiece,
        NotControllable,
        ReadyToFinish
    }
}