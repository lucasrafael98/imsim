using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour{
    public GameObject inventoryMenu;
    public bool invActive = false;

    private Inventory inventory;

    void Start(){
        inventoryMenu.SetActive(invActive);
    }

    void Update(){
        if(Input.GetKeyDown("tab")){
            inventoryMenu.SetActive(!invActive);
            invActive = !invActive;
            Cursor.visible = invActive;
            Cursor.lockState = invActive ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }
}
