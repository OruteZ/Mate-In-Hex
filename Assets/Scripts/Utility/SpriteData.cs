using UnityEngine;
using UnityEngine.Serialization;
using Chess;

[CreateAssetMenu(fileName = "SpriteData", menuName = "Chess/SpriteData", order = 0)]
public class SpriteData : ScriptableObject
{
    [Header("0: Pawn, 1: Rook, 2: Knight, 3: Bishop, 4: Queen, 5: King")]
    public Sprite[] white;
    public Sprite[] black;
     
    public Sprite GetSprite(Piece piece)
    {
        return piece.color == PieceColor.White ? 
            white[(int) piece.type] : 
            black[(int) piece.type];
    }
}