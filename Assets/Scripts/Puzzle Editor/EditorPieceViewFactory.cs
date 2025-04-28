using System;
using System.Collections;
using System.Collections.Generic;
using Chess;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class EditorPieceViewFactory : MonoBehaviour
{
    [SerializeField] private GameObject piecePrefabs;
    [SerializeField] private List<GameObject> pieceViews = new ();

    private void Awake() {
        foreach (var piece in pieceViews)
        {
            if (piece.TryGetComponent<EditorPieceView>(out var editorPieceView))
            {
                editorPieceView.onStartMoved.AddListener(() => OnStartMoved(editorPieceView));
                editorPieceView.piece.position = Hex.NONE;
            }
            else
            {
                Debug.LogError("EditorPieceView component not found on the instantiated prefab.");
            }
        }
    }

    private GameObject CreatePieceView(Piece pieceData) {
        GameObject pieceView = Instantiate(piecePrefabs, transform);

        if (pieceView.TryGetComponent<EditorPieceView>(out var editorPieceView))
        {
            editorPieceView.SetPiece(pieceData);
            editorPieceView.onStartMoved.AddListener(() => OnStartMoved(editorPieceView));
        }
        else
        {
            Debug.LogError("EditorPieceView component not found on the instantiated prefab.");
        }
        pieceViews.Add(pieceView);

        return pieceView;
    }

    private void OnStartMoved(EditorPieceView pieceView) {
        pieceViews.Remove(pieceView.gameObject);
        pieceView.onStartMoved.RemoveAllListeners();

        var piece = pieceView.piece;
        var newObj = CreatePieceView(piece.Clone());

        newObj.transform.position = pieceView.transform.position;
        newObj.transform.localScale = pieceView.transform.localScale;
        newObj.transform.rotation = pieceView.transform.rotation;
    }
}

#endif