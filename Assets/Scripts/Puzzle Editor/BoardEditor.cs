using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess;
using Puzzle;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Added for InputField

#if UNITY_EDITOR

[RequireComponent(typeof(BoardView))]
public class BoardEditor : MonoBehaviour
{
    public Board currentBoard;
    public BoardView boardView;
    public TMP_InputField puzzleNameInputField; // Reference to the InputField for puzzle name

    private void Awake() {
        boardView = GetComponent<BoardView>();
    }


    [ContextMenu("Create New Puzzle")]
    public void CreateNewPuzzle() {
        var currentEditing = ScriptableObject.CreateInstance<PuzzleInfo>();
        currentEditing.CreateBasicPuzzle();

        currentEditing.name = "New Puzzle";
        LoadInfo(currentEditing);
    }

    private void LoadInfo(PuzzleInfo puzzleInfo) {
        currentBoard = ScriptableObject.CreateInstance<Board>();
        currentBoard.InitBoard(puzzleInfo);

        boardView.RefreshBoardView(currentBoard, false);
    }

    public void TryApplyMove(Piece piece, Hex from)
    {
        if (currentBoard == null)
        {
            Debug.LogError("No current board to apply move to.");
            piece.position = Hex.NONE;
            return;
        }
        if (currentBoard.IsTileAvailable(piece.position))
        {
            return;
        }

        // case 1 : 다른 Piece와 위치가 겹칠 경우 : piece의 위치를 from으로 변경
        var pieces = currentBoard.Pieces.ToList();
        foreach (var p in pieces)
        {
            if (p.position == piece.position && p != piece)
            {
                piece.position = from;
                return;
            }
        }

        // case 2 : 타일 자체가 없을 경우 : piece의 위치를 NONE으로 변경
        if (currentBoard.Tiles.Contains(piece.position) == false)
        {
            piece.position = Hex.NONE;
        }
    }

    public void SaveBoardData() {
        // Save the current board data to the currentEditing PuzzleInfo
        if (currentBoard != null)
        {
            PuzzleInfo saveInfo = ScriptableObject.CreateInstance<PuzzleInfo>();
            saveInfo.tileList = currentBoard.Tiles;
            saveInfo.pieces = currentBoard.Pieces.ToList();

            // Save the PuzzleInfo as an asset
            UnityEditor.AssetDatabase.CreateAsset(
                saveInfo, 
                "Assets/Resources/Puzzles/" + puzzleNameInputField.text + ".asset"
            );
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("No PuzzleInfo to save.");
        }
    }
}

#endif