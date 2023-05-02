using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 touchOffset;

    public void OnPointerDown(PointerEventData eventData)
    {
        touchOffset = transform.position - eventData.pointerCurrentRaycast.worldPosition;
        transform.position = eventData.pointerCurrentRaycast.worldPosition + touchOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.isValid)
        {
            transform.position = eventData.pointerCurrentRaycast.worldPosition + touchOffset;
        }
    }

    
}
