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
            LeanTween.move(gameObject, targetPosition, duration)
                .setEase(LeanTweenType.linear)
                .setOnComplete(onComplete);
        }
    }
}