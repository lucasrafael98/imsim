using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetImage : MonoBehaviour{
    public Image img;

    public void setImage(Texture texture){
        // image = Component.FindObjectOfType<Image>();
        img.image = texture;
    }
}
