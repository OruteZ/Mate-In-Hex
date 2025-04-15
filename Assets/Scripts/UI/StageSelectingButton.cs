using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chess;
using Puzzle;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class StageSelectingButton : MonoBehaviour
{
    [SerializeField] private PuzzleInfo puzzleInfo;
    [SerializeField] private Button button;

    public void SetPuzzleInfo(PuzzleInfo puzzleInfo)
    {
        this.puzzleInfo = puzzleInfo;
    }


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // check if puzzleInfo is null
        if (puzzleInfo == null)
        {
            throw new System.NullReferenceException("puzzleInfo is null");
        }
        
        // set curSelectedPuzzleInfo to puzzleInfo
        GameManager.Instance.CurSelectedPuzzleInfo = puzzleInfo;
        
        // load game scene
        SceneManager.LoadScene("Puzzle");
    }
}
