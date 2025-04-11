using UnityEngine;
public class PaletteManager : Singleton<PaletteManager>
{
    [Header("Palette")]
    public Palette[] palettes;

    public Palette currentPalette;
    public Palette CurrentPalette
    {
        get => currentPalette;
        set
        {
            currentPalette = value;
            ApplyPalette();
        }
    }

    public void Awake()
    {
        if (palettes.Length == 0)
        {
            Debug.LogError("No palettes found. Please assign palettes in the inspector.");
            return;
        }

        // Set the first palette as the current one
        CurrentPalette = palettes[0];
    }

    private void ApplyPalette()
    {
        // Apply the current palette to all relevant components in the scene
        foreach (var palette in palettes)
        {
            if (palette == currentPalette)
            {
                // Apply this palette's colors to the relevant components
                // For example, you can set the color of UI elements, sprites, etc.
                // This is just a placeholder for demonstration purposes
                Debug.Log($"Applying palette: {palette.name}");
            }
        }
    }
}