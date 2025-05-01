using System.Collections;
using System.Collections.Generic;
using Chess;
using Puzzle;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageViewer : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("Tile의 Scale입니다. 1일경우 화면을 완벽하게 메우게 됩니다. ")]
    public float tileScale;
    public float tileDistance = 1.0f;
    public float tileAlpha = 0.5f;
    public float hoverScale = 1.2f;
    public float hoverDuration = 0.5f;
    
    [Header("Sprite Renderers")]
    public SpriteRenderer backGround;
    public List<SpriteRenderer> tiles;
    
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject tileParent;

    [Header("Puzzle Info")]
    public List<PuzzleInfo> puzzleInfos;
    private readonly Dictionary<Hex, PuzzleInfo> puzzleInfoDict = new ();

    [SerializeField] private PuzzleInfo selected;

    [Header("UI")]
    public TMP_Text stageNameText;
    public Button startButton;

    private void Awake()
    {
        ShowStageList();

        // set button event
        startButton.onClick.AddListener(() =>
        {
            if (selected != null)
            {
                GameManager.Instance.CurSelectedPuzzleInfo = selected;
                SceneManager.LoadScene("Puzzle");
            }
            else
            {
                Debug.LogError("No puzzle info selected.");
            }
        });
    }

    private void Update()
    {  
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }


        if (Input.GetMouseButtonDown(0))
        {
            Hex h = GetClickedTileHex();

            Debug.Log("Clicked on tile: " + h);
            if (puzzleInfoDict.TryGetValue(h, out PuzzleInfo puzzleInfo))
            {
                Debug.Log("Clicked on tile: " + h + " with puzzle info: " + puzzleInfo.name);
                selected = puzzleInfo;
                stageNameText.text =  $"Stage {puzzleInfo.name} : Mate in {puzzleInfo.GetMoveLimit()}";

                // 해당 타일이 중앙에 오도록 camera leanTween
                Vector3 targetPosition = h.ToPixel() * tileDistance;
                targetPosition.z = Camera.main.transform.position.z; // Preserve the original z position
                LeanTween.move(Camera.main.gameObject, targetPosition, 0.5f).setEaseOutQuad();
            }
            else
            {
                Debug.Log("Clicked on empty tile or no puzzle info found.");
            }
        }
    }

    private void ShowStageList() {
        // remove all tiles in the scene
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile.gameObject);
        }
        tiles.Clear();
        
        int cnt = 0;
        foreach (var info in puzzleInfos) {
            int q = cnt;
            int r = -(cnt / 2);

            Hex hex = new Hex(q, r);
            GameObject tile = Instantiate(tilePrefab, tileParent.transform);
            tile.name = $"Stage Tile : {info.name}";

            // Adjust the tile position based on tileDistance
            tile.transform.localPosition = hex.ToPixel() * tileDistance;
            tile.transform.localScale *= tileScale;
            var hover = tile.AddComponent<HoverScale>();
            hover.scaleFactor = hoverScale;
            hover.duration = hoverDuration;

            SpriteRenderer pieceSprite = tile.transform.GetChild(0).GetComponent<SpriteRenderer>();
            Piece samplePiece = new(PieceColor.Black, info.showPieceType, hex);
            pieceSprite.sprite = GameManager.Instance.PieceSpriteData.GetSprite(samplePiece);

            if (tile.TryGetComponent(out SpriteRenderer tileSpriteRenderer))
            {
                tiles.Add(tileSpriteRenderer);
                puzzleInfoDict.Add(hex, info);
            }
            else
            {
                Debug.LogError("Tile prefab does not have a SpriteRenderer component.");
                DestroyImmediate(tile);
            }

            cnt++;
        }
    }

    private Hex GetClickedTileHex()
    {
        // Alternative method: directly convert mouse position from screen to world coordinates
        Vector2 screenPos = Input.mousePosition;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Hex result = Hex.GetHexFromPixel(worldPos);
        return result;
    }
}
