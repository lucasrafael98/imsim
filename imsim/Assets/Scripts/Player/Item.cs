using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item{
    public enum ItemType{
        Prod,
        Shiv
    }

    public ItemType itemType;
    public int amount;
    public int sizeX;
    public int sizeY;

    public Texture GetSprite(){
        switch(itemType){
            default:
            case ItemType.Prod: return ItemAssets.Instance.defaultSprite;
            case ItemType.Shiv: return ItemAssets.Instance.defaultSprite;
        }
    }
}
