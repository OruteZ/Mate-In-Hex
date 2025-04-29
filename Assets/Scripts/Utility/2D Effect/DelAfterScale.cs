using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelAfterScale : MonoBehaviour
{
    public LeanTweenType easeType;
    public float duration;
    public float scaleSize;
    public bool deleteAfterScale = true;

    public void Start()
    {
        LeanTween.scale(gameObject, Vector3.one * scaleSize, duration)
            .setEase(easeType)
            .setOnComplete(                () =>
                {
                    if (deleteAfterScale)
                    {
                        Destroy(gameObject);
                    }
                }            );
    }
}
