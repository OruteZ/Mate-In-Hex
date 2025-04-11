using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Chess
{
    [System.Serializable]
    public class BoardView : MonoBehaviour
    {
        public GameObject tilePrefab;
        public GameObject piecePrefab;
        
        public Dictionary<Hex, GameObject> pieceList = new ();

        public SpriteRenderer backBoard;

        private void Awake()
        {
            NullCheck();
            backBoard.color = PaletteManager.Instance.CurrentPalette.backGround;
        }
        
        private void NullCheck()
        {
            if (tilePrefab == null)
            {
                throw new NullReferenceException("tilePrefab is null");
            }
            
            if (piecePrefab == null)
            {
                throw new NullReferenceException("piecePrefab is null");
            }
        }
        
        public void CreateBoardView([NotNull] Board board, bool showLastMove = false)
        {
            Move lastMove = Move.NONE;
            if (showLastMove && board.Moves.Count > 0)
            {
                // undo last move
                lastMove = board.Moves.Last();
                board.UndoMove(lastMove);
            }
            
            
            
            List<Hex> tiles = board.Tiles;
            if(tiles is null) 
            {
                Debug.LogError("왜 null이지?");
                tiles = new List<Hex>();
            }

            foreach (Hex tile in tiles)
            {
                Vector3 tilePosition = tile.ToPixel();
                tilePosition.z = 1; // avoid z-fighting with pieces
                GameObject tileGo = Instantiate(
                    tilePrefab, 
                    tilePosition, 
                    Quaternion.identity
                );
                tileGo.name = $"Tile_{tile.Q}_{tile.R}";
                
                tileGo.transform.SetParent(transform);
                
                // set tile color
                int colorNum = tile.GetTileKind();
                tileGo.GetComponent<SpriteRenderer>().color = PaletteManager.Instance.CurrentPalette.tileColor[colorNum];
            }
            
            pieceList.Clear();
            // instantiate pieces
            foreach (Piece piece in board.Pieces)
            {
                GameObject pieceGo = Instantiate(
                    piecePrefab,
                    piece.position.ToPixel(),
                    Quaternion.identity
                    );
                
                // get the piece component
                if (pieceGo.TryGetComponent(out PieceView pieceView))
                {
                    pieceView.SetPiece(piece);
                }
                else
                {
                    throw new NullReferenceException("PieceView component is null on Piece prefab");
                }
                
                pieceGo.transform.SetParent(transform);
                pieceGo.name = $"{piece.color} {piece.type} {piece.position}";
                pieceList.Add(piece.position, pieceGo);
            }

            if (lastMove.IsNoneValue()) return;
            board.ApplyMove(lastMove);
            
            // show last move
            // 1. get piece view from
            if (pieceList.TryGetValue(lastMove.from, out var go))
            {
                // 2. move piece to target position
                Vector3 targetPosition = lastMove.to.ToPixel();
                targetPosition.z = go.transform.position.z;
                go.GetComponent<PieceView>().TweenMove(targetPosition, () =>
                {
                    // 3. set the piece position in the pieceList
                    pieceList.Remove(lastMove.from);
                    pieceList.Add(lastMove.to, go);
                });
            }
            else
            {
                Debug.LogError("Piece not found in pieceList");
            }
        }
        
        public void RefreshBoardView(Board board, bool animLastMove = false)
        {
            // destroy all children
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            // create board view
            CreateBoardView(board, animLastMove);
        }
    }
}