using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Chess
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PieceView : MonoBehaviour
    {
        public Piece piece;
        public SpriteData spriteData;
        public float speed = 5f;
        
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
        
        public void TweenMove(Vector3 targetPosition, Action onComplete = null)
        {
            // Calculate distance between current position and target position
            float distance = Vector3.Distance(transform.position, targetPosition);

            // Calculate duration based on fixed speed
            float duration = distance / speed;

            // Move the piece to the target position over the calculated duration
            Debug.Log("PieceView TweenMove: " + "distance" + distance + " duration: " + duration);
            LeanTween.move(gameObject, targetPosition, duration)
                .setOnComplete(onComplete);
        }

        [ContextMenu("Round")]
        public void Round()
        {
            Hex h = Hex.GetHexFromPixel(transform.position);
            
            Vector3 targetPosition = h.ToPixel();
            transform.position = targetPosition;
        }

        [ContextMenu("Reload Sprite")]
        public void ResetPosition()
        {
            // Get the SpriteRenderer component attached to this GameObject
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Check if the SpriteRenderer component is found
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer component not found on this GameObject." + gameObject.name);
                return;
            }
            
            // Set the sprite to the default sprite
            spriteRenderer.sprite = GetSprite(piece);
        }
    }
}