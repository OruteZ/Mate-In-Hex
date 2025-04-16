using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuArt : MonoBehaviour
{
    public PaletteManager manager;

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

    private void Awake() {
        ApplyPalette();
    }

    [ContextMenu("Create Tile")]
    private void CreateTile()
    {
        // remove all tiles in the scene
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile.gameObject);
        }
        tiles.Clear();

        var hexList = Hex.GetHexMap(5);
        foreach (var hex in hexList)
        {
            GameObject tile = Instantiate(tilePrefab, tileParent.transform);
            tile.name = $"Tile {hex.Q} {hex.R}";
            // Adjust the tile position based on tileDistance
            tile.transform.localPosition = hex.ToPixel() * tileDistance;
            tile.transform.localScale *= tileScale;
            var hover = tile.AddComponent<HoverScale>();
            hover.scaleFactor = hoverScale;
            hover.duration = hoverDuration;

            if (tile.TryGetComponent(out SpriteRenderer tileSpriteRenderer))
            {
                tiles.Add(tileSpriteRenderer);
            }
            else
            {
                Debug.LogError("Tile prefab does not have a SpriteRenderer component.");
                DestroyImmediate(tile);
            }
        }
    }

    [ContextMenu("Apply Palette")]
    public void ApplyPalette()
    {
        // Get the palette from the PaletteManager
        if (manager == null)
        {
            manager = PaletteManager.Instance;
            if (manager == null)
            {
                Debug.LogError("PaletteManager instance is null. Please ensure it is set up correctly.");
                return;
            }
        }

        Palette curPalette = manager.currentPalette;
        if (curPalette == null)
        {
            Debug.LogError("Current palette is null. Please assign a palette in the inspector.");
            return;
        }

        backGround.color = curPalette.backGround;
        foreach (var tile in tiles)
        {
            Hex tileHex = Hex.NONE;
            string[] parts = tile.name.Split(' ');
            if (parts.Length >= 3 &&
                int.TryParse(parts[1], out int col) &&
                int.TryParse(parts[2], out int row))
            {
                tileHex = new Hex(col, row);
            }

            int tileKind = tileHex.GetTileKind();
            Color tileColor = curPalette.tileColor[tileKind];
            tile.color = new Color(tileColor.r, tileColor.g, tileColor.b, tileAlpha);

            
        }
    }
}
