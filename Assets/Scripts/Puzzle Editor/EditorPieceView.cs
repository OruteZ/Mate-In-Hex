using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;
using UnityEngine.EventSystems;
using UnityEngine.Events;

#if UNITY_EDITOR

public class EditorPieceView : PieceView, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;
    private BoardEditor editor;

    public UnityEvent onStartMoved = new UnityEvent();
    
    private void Awake()
    {
        mainCamera = Camera.main;

        //오우 오우 쓰레기 코드
        editor = FindObjectOfType<BoardEditor>();
        if (editor == null)
        {
            Debug.LogError("EditorPieceView: BoardEditor not found in the scene.");
            return;
        }
    }
    
    // Begin dragging: calculate the pointer offset from the object
    public void OnBeginDrag(PointerEventData eventData)
    {
        onStartMoved.Invoke(); // Notify that the piece has started moving


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

        // 0. try apply move
        editor.TryApplyMove(piece, from);

        // 1. if piece deleted : delete it
        if(piece.position == Hex.NONE)
        {
            Destroy(gameObject);
            return;
        }

        // 2. if crated piece (from == NONE) : add it to the board
        if (from == Hex.NONE)
        {
            // Add the piece to the board
            editor.currentBoard.TryAddPiece(piece);
        }

        
        // Optionally, you can also update the visual representation of the piece here if needed.
        // For example, you might want to snap the piece to the center of the hexagon.
        Vector3 targetPosition = piece.position.ToPixel();
        transform.position = targetPosition;
    }
}

#endif