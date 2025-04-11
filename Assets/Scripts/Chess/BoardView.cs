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
        
        public void CreateBoardView([NotNull] Board board)
        {
            // instantiate tiles
            // 이거 왜 null이지지
            var tiles = board.Tiles;
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
            }
        }
        
        public void RefreshBoardView(Board board)
        {
            // destroy all children
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            // create board view
            CreateBoardView(board);
        }
    }
}