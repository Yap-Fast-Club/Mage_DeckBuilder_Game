using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform _handZone;
    [SerializeField]
    private RectTransform _playAreaZone;
    [SerializeField]
    private Card _card;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging started");
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;
    }

    private void CheckZone()
    {
        Vector3[] handCorners = new Vector3[4];
        _handZone.GetWorldCorners(handCorners);
        Vector3[] playAreaCorners = new Vector3[4];
        _playAreaZone.GetWorldCorners(playAreaCorners);

        // Assuming the UI element's pivot is at its center
        Vector3 uiElementCenter = transform.position;

        // Check if the UI element is in the Hand zone
        if (handCorners[0].x <= uiElementCenter.x && uiElementCenter.x <= handCorners[2].x &&
            handCorners[0].y <= uiElementCenter.y && uiElementCenter.y <= handCorners[2].y)
        {
            Debug.Log("In Hand zone");
            // Perform action for being in the Hand zone
        }
        // Check if the UI element is in the Play Area zone
        else if (playAreaCorners[0].x <= uiElementCenter.x && uiElementCenter.x <= playAreaCorners[2].x &&
                 playAreaCorners[0].y <= uiElementCenter.y && uiElementCenter.y <= playAreaCorners[2].y)
        {
            Debug.Log("In Play Area zone");
            // Perform action for being in the Play Area zone
        }
        else
        {
            Debug.Log("Outside zones");
            // Perform action for being outside both zones
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging ended");
        CheckZone();
    }
}


