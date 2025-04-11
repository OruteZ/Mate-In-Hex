using System;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "Pallete", menuName = "Pallete", order = 0)]
public class Palette : ScriptableObject 
{
    [Header("Background Color")]
    public Color backGround;

    [Header("Board Tiles (dark, gray, white 순서)")]
    public Color[] tileColor;

    [Header("Piece Colors (black, white 순서)")]
    public Color[] pieceColor;

    [Header("Point Indicator Color")]
    public Color point;

    [TextArea(5, 10)]
    public string csvString;

    [ContextMenu("Read Json")]
    public void ReadCSV() 
    {
        string[] parts = csvString.Split(',');
        if (parts.Length < 8)
        {
            Debug.LogError("CSV string does not contain enough values.");
            return;
        }

        // 첫 번째 값은 팔레트 이름 (필요하면 사용)
        string paletteName = parts[0].Trim();
        // scriptable obj 이름 변경
        name = paletteName;
    #if UNITY_EDITOR
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        UnityEditor.AssetDatabase.RenameAsset(assetPath, paletteName);
    #endif

        static Color ParseColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex.Trim(), out Color color))
            {
            return color;
            }
            Debug.LogError("Invalid color code: " + hex);
            return Color.magenta;
        }

        // 배경 색상
        backGround = ParseColor(parts[1]);

        // Board Tiles 색상 (dark, gray, white 순서)
        tileColor = new Color[3];
        for (int i = 0; i < 3; i++)
        {
            tileColor[i] = ParseColor(parts[2 + i]);
        }

        // Piece Colors (black, white 순서)
        pieceColor = new Color[2];
        for (int i = 0; i < 2; i++)
        {
            pieceColor[i] = ParseColor(parts[5 + i]);
        }

        // Point Indicator 색상
        point = ParseColor(parts[7]);
    }
}
