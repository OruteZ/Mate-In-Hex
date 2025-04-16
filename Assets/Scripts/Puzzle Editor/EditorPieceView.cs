using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EditorPieceView : PieceView, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;

    /// <summary>
    /// Piece moved / before hex
    /// </summary>
    /// <returns></returns>
    public UnityEvent<Piece,Hex> onPieceMoved = new();
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    // Begin dragging: calculate the pointer offset from the object
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPos);
        offset = transform.position - worldPoint;
    }
    
    // Dragging: update the object position following the pointer
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPos);
        transform.position = worldPoint + offset;
    }
    
    // End dragging: round the drop position to the nearest hex center and update piece data accordingly
    public void OnEndDrag(PointerEventData eventData)
    {
        Round();
        UpdatePieceData();
    }
    
    // Update the internal piece data with the new rounded position.
    private void UpdatePieceData()
    {
        Hex from = piece.position; // Store the original position before moving

        Hex newHex = Hex.GetHexFromPixel(transform.position);
        piece.position = newHex;

        onPieceMoved.Invoke(piece, from); // Notify listeners about the piece movement

        // 1. if piece deleted : delete it
        if(piece.position == Hex.NONE)
        {
            Destroy(gameObject);
            return;
        }

        
        // Optionally, you can also update the visual representation of the piece here if needed.
        // For example, you might want to snap the piece to the center of the hexagon.
        Vector3 targetPosition = piece.position.ToPixel();
        transform.position = targetPosition;
    }
}
