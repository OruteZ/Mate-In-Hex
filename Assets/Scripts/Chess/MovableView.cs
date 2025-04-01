using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableView : MonoBehaviour
{
    [SerializeField] private GameObject movableUIPrefab;

    public void ShowMovable(List<Hex> hexList)
    {
        foreach (Hex hex in hexList)
        {
            GameObject movableUI = Instantiate(movableUIPrefab, transform);

            movableUI.transform.position = hex.ToPixel();
            movableUI.transform.position = new Vector3(
                movableUI.transform.position.x,
                movableUI.transform.position.y,
                1f // avoid z-fighting with pieces
            );
        }
    }

    public void HideMovable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
