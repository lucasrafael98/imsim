using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabRigidbody : MonoBehaviour {
    public Camera cam;
    public Transform grabPos;
    RaycastHit hit;
    GameObject grabbedObject;
    void Start() {

    }

    void Update() {
        if (Input.GetMouseButtonDown(1) && !grabbedObject
            && Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)), out hit, 2)
            && hit.transform.GetComponent<Rigidbody>()) {
            grabbedObject = hit.transform.gameObject;
        }
        if (grabbedObject) {
            if (Input.GetMouseButtonUp(1)) {
                grabbedObject = null;
                return;
            }
            else if (Input.GetMouseButtonDown(0)) {
                grabbedObject.GetComponent<Rigidbody>().velocity = cam.transform.forward * 10;
                grabbedObject = null;
                return;
            }
            grabbedObject.GetComponent<Rigidbody>().velocity = 50 * (grabPos.position - grabbedObject.transform.position);
        }
    }
}
