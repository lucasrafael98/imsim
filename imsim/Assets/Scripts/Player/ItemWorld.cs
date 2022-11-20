using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemWorld : MonoBehaviour{
    public Item item;
    public GameObject image;
    public Canvas canvas;

    public void OnMouseDown(){
        if(UnityEngine.Cursor.visible){
            image.transform.SetParent(canvas.transform);
            this.gameObject.transform.SetParent(image.transform);
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.gameObject.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos); 
            image.GetComponent<RectTransform>().anchoredPosition = pos;
            image.transform.position = new Vector3(image.transform.position.x, image.transform.position.y, 0);
            image.SetActive(true);
            image.transform.rotation = new Quaternion(0,0,0,1);
            this.gameObject.SetActive(false);
        }
    }
}
