using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableView : MonoBehaviour
{
    [SerializeField] private GameObject movableUIPrefab;
    [SerializeField] private List<GameObject> movableUIList = new List<GameObject>();

    public void ShowMovable(List<Hex> hexList)
    {
        if(movableUIList.Count > 0)
        {
            HideMovable();
        }

        foreach (Hex hex in hexList)
        {
            GameObject movableUI = Instantiate(movableUIPrefab, transform);

            movableUI.transform.position = hex.ToPixel();
            movableUI.transform.position = new Vector3(
                movableUI.transform.position.x,
                movableUI.transform.position.y,
                -1f // avoid z-fighting with pieces
            );

            movableUIList.Add(movableUI);
        }
    }

    public void HideMovable()
    {
        foreach (GameObject child in movableUIList)
        {
            Destroy(child.gameObject);
        }
        movableUIList.Clear();
    }
}
