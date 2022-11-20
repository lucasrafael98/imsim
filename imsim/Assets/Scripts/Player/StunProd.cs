using UnityEngine;

internal class Stun : Stim.Stim {
	internal Vector3 dir;
	void Awake() {
		this.key = Stim.Key.Stun;
	}
}

public class StunProd : MonoBehaviour {

	// TODO: see Throw.cs
	public Camera cam;
	private Stun s;

	void Awake() {
		// TODO: ehhh maybe consider turning stim.stim into a scriptable object
		this.s = gameObject.AddComponent<Stun>();
	}

	void Update() {
		if (Input.GetKeyDown("v")) {
			RaycastHit hit;
			Ray xhair = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));

			if (Physics.Raycast(xhair.origin, xhair.direction, out hit, 1f)) {
				var resp = hit.collider.gameObject.GetComponent<Stim.Response>();
				if (resp != null) {
					this.s.dir = transform.forward;
					resp.Respond(this.s);
				}
			}
		}
	}
}
