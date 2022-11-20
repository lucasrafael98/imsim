using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOnWorld : MonoBehaviour, IDropHandler {
    public Camera cam;

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null && eventData.pointerDrag.transform.childCount != 0) {
            Transform child = eventData.pointerDrag.transform.GetChild(0);
            child.parent = null;
            eventData.pointerDrag.transform.SetParent(child);
            eventData.pointerDrag.GetComponent<CanvasGroup>().blocksRaycasts = true;
            eventData.pointerDrag.GetComponent<CanvasGroup>().alpha = 1f;
            eventData.pointerDrag.SetActive(false);
            child.gameObject.SetActive(true);
            child.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 2f));
        }
    }

    public void OnDrop(PointerEventData eventData, bool rclick) {
        if (eventData.pointerDrag != null && eventData.pointerDrag.transform.childCount != 0) {
            Transform child = eventData.pointerDrag.transform.GetChild(0);
            child.SetParent(null);
            eventData.pointerDrag.transform.SetParent(child);
            eventData.pointerDrag.GetComponent<CanvasGroup>().blocksRaycasts = true;
            eventData.pointerDrag.GetComponent<CanvasGroup>().alpha = 1f;
            eventData.pointerDrag.SetActive(false);
            child.gameObject.SetActive(true);
            child.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 2f));
            if (rclick) {
                child.gameObject.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 500f);
            }
        }
    }
}
