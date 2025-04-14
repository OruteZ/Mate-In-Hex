using UnityEngine;

public class HoverScale : MonoBehaviour
{
    public float scaleFactor = 1.2f; // 커질 배수
    public float duration = 0.5f;    // Tween 지속 시간

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale * scaleFactor, duration).setEase(LeanTweenType.easeOutBack);
    }

    void OnMouseExit()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale, duration).setEase(LeanTweenType.easeOutBack);
    }
}
