using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
	[Header("Vision")]
	[Range(0f, 180f)][SerializeField] float hostileFOV;
	[Range(0f, 50f)][SerializeField] float hostileRange;
	[Range(0f, 180f)][SerializeField] float periphFOV;
	[Range(0f, 90f)][SerializeField] float periphRange;
	[Range(0f, 1f)][SerializeField] float lightSensitivity;
	[SerializeField] GameObject eyes;
	[Space(1)]

	[Header("Sixth Sense")]
	[Range(0f, 360f)][SerializeField] float ssAngleFront;
	[Range(0f, 360f)][SerializeField] float ssAngleBack;
	[Range(0f, 10f)][SerializeField] float ssRange;
	[Space(1)]

	[Header("Detection")]
	[Range(0f, 100f)][SerializeField] internal float chaseThr;
	[Range(0f, 100f)][SerializeField] internal float alertThr;
	[Range(0f, 25f)][SerializeField] float ssIncr;
	[Range(0f, 25f)][SerializeField] float periIncr;
	[Range(0f, 25f)][SerializeField] float dirIncr;
	[Range(0f, 25f)][SerializeField] float decrease;
	[Range(0f, 50f)][SerializeField] internal float stunAngle;
	[Space(10)]

	public Transform patrolPoints;
	public GameObject player;

	// main detection meter, rises/falls with player detection
	// or lack thereof. 
	// TODO: should not be so coupled to the player!
	internal float detec;

	internal NavMeshAgent agent;
	internal Vector3 currTarget;
	internal List<Vector3> patrol;
	internal int patrolIdx = 0;
	internal AI ai;

	void Awake() {
		this.ai = GetComponent<AI>();

		patrol = new List<Vector3>();
		List<Transform> children = new(patrolPoints.gameObject.GetComponentsInChildren<Transform>());
		children.RemoveAt(0);
		foreach (Transform t in children) {
			if (!t.Equals(transform)) patrol.Add(t.position);
		}

		agent = GetComponent<NavMeshAgent>();

		ai._state = new AIPatrol(this);
		GoToNextPoint();
	}

	void Update() {
		ai._state.Do(this);

		float attenuation;
		if (checkDirectSight(out attenuation)) {
			this.detec += dirIncr * Time.deltaTime * attenuation;
		} else if (checkSixthSense()) {
			this.detec += ssIncr * Time.deltaTime;
		} else if (checkPeriphSight(out attenuation)) {
			this.detec += periIncr * Time.deltaTime * attenuation;
		} else if (this.detec > 0) {
			this.detec -= decrease * Time.deltaTime;
		}

		var newState = ai._state.Change(this);
		if (newState != null) {
			ai._state = newState;
		}
	}

	internal bool checkDirectSight(out float att) {
		att = 0f;

		Vector3 plrPos = player.transform.position;
		Vector3 eyePos = eyes.transform.position;
		Vector3 eyeFwd = eyes.transform.forward;

		Vector3 fovRad = eyeFwd * hostileRange;
		float plrDist = Vector3.Distance(plrPos - eyePos, fovRad);
		float plrAngle = Vector3.Angle(plrPos - eyePos, fovRad);
		var lightMeter = player.GetComponent<LightMeter>();

		if (plrAngle < hostileFOV / 2 && plrDist < hostileRange && lightMeter.val > lightSensitivity) {
			RaycastHit hit;
			if (Physics.Raycast(eyePos, plrPos - eyePos, out hit)) {
				if (hit.collider.CompareTag("Player")) {
					currTarget = hit.transform.position;
					att = hostileRange / 2 / Vector3.Distance(eyePos, plrPos)
						* lightMeter.val;
					return true;
				}
			}
		}

		return false;
	}

	internal bool checkPeriphSight(out float att) {
		att = 0f;
		Vector3 plrPos = player.gameObject.transform.position;
		Vector3 eyePos = eyes.gameObject.transform.position;
		Vector3 eyeFwd = eyes.gameObject.transform.forward;

		Vector3 fovRad = eyeFwd * hostileRange;
		float plrDist = Vector3.Distance(plrPos - eyePos, fovRad);
		float plrAngle = Vector3.Angle(plrPos - eyePos, fovRad);

		var lightMeter = player.GetComponent<LightMeter>();

		if (plrAngle < periphFOV / 2 && plrDist < periphRange && lightMeter.val > lightSensitivity) {
			RaycastHit hit;
			Debug.DrawRay(eyePos, plrPos - eyePos);
			if (Physics.Raycast(eyePos, plrPos - eyePos, out hit)) {
				if (hit.collider.CompareTag("Player")) {
					currTarget = hit.transform.position;
					att = periphRange / Vector3.Distance(eyePos, plrPos)
						* lightMeter.val;
					return true;
				}
			}
		}

		return false;
	}

	internal bool checkSixthSense() {
		Vector3 plrPos = player.transform.position;
		Vector3 eyePos = eyes.transform.position;
		Vector3 eyeFwd = eyes.transform.forward;

		Vector3 ssRad = -eyeFwd * ssRange;
		float plrDist = Vector3.Distance(plrPos - eyePos, ssRad);
		float plrAngle = Vector3.Angle(plrPos - eyePos, ssRad);

		if (plrAngle > ssAngleFront / 2 && plrAngle < ssAngleBack / 2 && plrDist < ssRange) {
			RaycastHit hit;
			Debug.DrawRay(eyePos, plrPos - eyePos);
			if (Physics.Raycast(eyePos, plrPos - eyePos, out hit)) {
				if (hit.collider.CompareTag("Player")) {
					currTarget = hit.transform.position;
					return true;
				}
			}
		}

		return false;
	}

	public void GoToNextPoint() {
		if (patrol.Count == 0) return;
		GetComponent<NavMeshAgent>().destination = patrol[patrolIdx];
		patrolIdx = (patrolIdx + 1) % patrol.Count;
	}

	void OnDrawGizmosSelected() {
		List<Transform> children = new List<Transform>(patrolPoints.GetComponentsInChildren<Transform>());
		children.RemoveAt(0);
		Transform prev = children[children.Count - 1];
		foreach (Transform t in children) {
			Gizmos.DrawSphere(t.position, 0.5f);
			Gizmos.DrawLine(prev.position, t.position);
			prev = t;
		}
		if (agent != null) Gizmos.DrawSphere(agent.destination, 0.2f);

		Vector3 pos = eyes.transform.position;
		Vector3 fwd = eyes.transform.forward;

		Gizmos.color = Color.red;
		drawTwinRays(pos, fwd, hostileFOV / 2f, hostileRange);

		Gizmos.color = Color.yellow;
		drawTwinRays(pos, fwd, periphFOV / 2f, periphRange);

		Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 1.0f);
		drawTwinRays(pos, -fwd, ssAngleFront / 2f, ssRange);
		drawTwinRays(pos, -fwd, ssAngleBack / 2f, ssRange);

		Gizmos.color = new Color(0f, 0.5f, 0.5f, 1.0f);
		drawTwinRays(pos, -fwd, stunAngle, 1f);
	}

	private void drawTwinRays(Vector3 pos, Vector3 fwd, float fov, float dist) {
		var leftRot = Quaternion.AngleAxis(-fov, Vector3.up);
		var leftDir = leftRot * fwd;
		Gizmos.DrawRay(pos, leftDir * dist);

		var rightRot = Quaternion.AngleAxis(fov, Vector3.up);
		var rightDir = rightRot * fwd;
		Gizmos.DrawRay(pos, rightDir * dist);
	}

}
