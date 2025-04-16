using System.Collections;
using System.Collections.Generic;
using Puzzle;
using UnityEngine;

public class StageViewer : MonoBehaviour
{
    [SerializeField] private GameObject stageBtnPrefab;
    [SerializeField] private Transform stageBtnParent;

    [SerializeField] private List<PuzzleInfo> puzzleInfos = new List<PuzzleInfo>();

    private void Awake()
    {
        // Create puzzle buttons
        foreach (var puzzleInfo in puzzleInfos)
        {
            var btn = Instantiate(stageBtnPrefab, stageBtnParent);
            btn.GetComponent<StageSelectingButton>().SetPuzzleInfo(puzzleInfo);
        }
    }
}
