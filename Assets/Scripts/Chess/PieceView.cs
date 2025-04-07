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
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component not found on this GameObject." + gameObject.name);
                return;
            }
            spriteRenderer.sprite = GetSprite(newPiece);
            
            // set name
            name = $"{newPiece.color} {newPiece.type} {newPiece.position}";
        }

        private Sprite GetSprite(Piece targetPiece)
        {
            return spriteData.GetSprite(targetPiece);
        }
    }
}