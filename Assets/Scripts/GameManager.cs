using System.Collections;
using System.Collections.Generic;
using Puzzle;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PuzzleInfo curSelectedPuzzleInfo;

    public PuzzleInfo CurSelectedPuzzleInfo
    {
        get => curSelectedPuzzleInfo;
        set => curSelectedPuzzleInfo = value;
    }

    public bool IsPause
    {
        get => Time.timeScale == 0;
        set => Time.timeScale = value ? 0 : 1;
    }

    protected override void Awake()
    {
        base.Awake();
        if (this == null)
        {
            return;
        }
    }
}
