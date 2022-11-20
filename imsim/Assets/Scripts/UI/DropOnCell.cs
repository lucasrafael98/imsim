using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnCell : MonoBehaviour, IDropHandler{
    public RectTransform gridTransform;

    public void OnDrop(PointerEventData eventData){
        if(eventData.pointerDrag != null){
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(gridTransform.offsetMin.x + GetComponent<RectTransform>().anchoredPosition.x, 
                            gridTransform.offsetMax.y + GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
}
