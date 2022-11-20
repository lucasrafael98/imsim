using UnityEngine;

public class AIResponse : Stim.Response {
	AI ai;

	void Awake() {
		this.ai = GetComponent<AI>();

		stims = new();
		stims.Add(Stim.Key.Water, WaterResponse);
		stims.Add(Stim.Key.Stun, StunResponse);
	}

	private void WaterResponse(Stim.Stim s) {
		Water w = (Water)s;

		// TODO: too fragile, find some extensible way to figure this out.
		if (ai._state.GetType() != typeof(AIChase)) {
			ai._state = new AIAlert(ai._ctrl);
			ai._ctrl.currTarget = transform.position - w.dir * 3;
			ai._ctrl.detec = Mathf.Clamp(ai._ctrl.detec + 25, 0, ai._ctrl.chaseThr - 1);
		}
	}

	private void StunResponse(Stim.Stim stim) {
		Stun s = (Stun)stim;

		var angle = Vector3.Angle(s.dir, transform.forward);
		var rad = ai._ctrl.stunAngle;

		if (ai._ctrl.detec < ai._ctrl.alertThr &&
				(angle < rad && angle > -rad)) {
			Destroy(gameObject);
		}
	}
}
