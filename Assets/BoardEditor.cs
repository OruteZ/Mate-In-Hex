using System.Collections;
using System.Collections.Generic;
using Chess;
using Puzzle;
using UnityEngine;

[RequireComponent(typeof(BoardView))]
public class BoardEditor : MonoBehaviour
{
    public PuzzleInfo currentEditing;
    public BoardView boardView;

    private void Awake() {
        boardView = GetComponent<BoardView>();
    }


    [ContextMenu("Create New Puzzle")]
    public void CreateNewPuzzle() {
        currentEditing = ScriptableObject.CreateInstance<PuzzleInfo>();
        currentEditing.CreateBasicPuzzle();

        currentEditing.name = "New Puzzle";
        LoadInfo(currentEditing);
    }

    private void LoadInfo(PuzzleInfo puzzleInfo) {
        currentEditing = puzzleInfo;
        Board newBoard = new();
        newBoard.InitBoard(puzzleInfo);

        boardView.CreateBoardView(newBoard, false);
    }
}
