using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour{
    private Inventory inventory;

    public void setInventory(Inventory inventory){
        this.inventory = inventory;
    }

    private void RefreshInventory(){
        foreach(Item item in inventory.getItemList()){
            
        }
    }
}
