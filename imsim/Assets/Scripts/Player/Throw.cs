using UnityEngine;

public class Throw : MonoBehaviour {
    public GameObject throwable;
    // TODO: argh! too dependent on the stupid cam being set in the editor
    public Camera cam;

    void Update() {
        if (Input.GetKeyDown("b")) {
            Ray xhair = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));

            var thr = Instantiate(throwable, xhair.origin + xhair.direction, transform.rotation);
            thr.GetComponent<Rigidbody>().velocity = xhair.direction * 20 + transform.up;
        }
    }
}
