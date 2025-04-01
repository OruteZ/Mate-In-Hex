using UnityEngine;
using UnityEngine.Serialization;

namespace Chess
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PieceView : MonoBehaviour
    {
        public Piece piece;
        public SpriteData spriteData;
        
        public void SetPiece(Piece newPiece)
        {
            piece = newPiece;
            
            // Set sprite
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetSprite(newPiece);
            
            // set name
            name = $"{newPiece.color} {newPiece.type} {newPiece.position}";
        }

        private Sprite GetSprite(Piece targetPiece)
        {
            return spriteData.GetSprite(targetPiece);
        }
    }
    
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
                black[(int) piece.type] ;
        }
    }
}