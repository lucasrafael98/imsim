using UnityEngine;

public class AI : MonoBehaviour {
	internal AIState _state;
	internal AIController _ctrl;
	internal Stim.Response _resp;

	void Awake() {
		this._ctrl = GetComponent<AIController>();
		this._resp = GetComponent<Stim.Response>();
	}
}
