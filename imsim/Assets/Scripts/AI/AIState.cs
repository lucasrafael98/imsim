using System.Collections.Generic;
using UnityEngine;

public abstract class AIState {
	/// <summary>
	/// Executes the state's action (e.g. movement).
	/// </summary>
	public abstract void Do(AIController ctrl);
	/// <summary>
	/// Checks if a change of state is required.
	/// If the returned state is null, no change needs to happen.
	/// </summary>
	public abstract AIState Change(AIController ctrl);
}


public class AIPatrol : AIState {
	public AIPatrol(AIController ctrl) {
		var mat = ctrl.transform.GetComponent<Renderer>().material;

		mat.SetColor("_Color", Color.white);
		mat.SetColor("_EmissionColor", Color.white);
	}
	public override void Do(AIController ctrl) {
		ctrl.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
		ctrl.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
		if (!ctrl.agent.pathPending && ctrl.agent.remainingDistance < 0.5f) {
			ctrl.GoToNextPoint();
		}
	}
	public override AIState Change(AIController ctrl) {
		if (ctrl.detec > ctrl.chaseThr) return new AIChase();
		else if (ctrl.detec > ctrl.alertThr) return new AIAlert(ctrl);
		return null;
	}
}


public class AIChase : AIState {
	public override void Do(AIController ctrl) {
		ctrl.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
		ctrl.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);

		// when chasing, the ai is always aware of the player. 
		// avoiding its sight for enough time should drop it back into alert.
		ctrl.agent.SetDestination(ctrl.player.transform.position);
	}
	public override AIState Change(AIController ctrl) {
		if (ctrl.detec < ctrl.chaseThr) {
			return new AIAlert(ctrl);
		}
		return null;
	}
}

public class AIAlert : AIState {
	private bool _nearTarget = false;

	public AIAlert(AIController ctrl) {
		var mat = ctrl.transform.GetComponent<Renderer>().material;

		mat.SetColor("_Color", Color.yellow);
		mat.SetColor("_EmissionColor", Color.yellow);
	}

	public override void Do(AIController ctrl) {
		ctrl.agent.SetDestination(ctrl.currTarget);

		_nearTarget = !ctrl.agent.pathPending && ctrl.agent.remainingDistance < 0.5f;
	}

	public override AIState Change(AIController ctrl) {
		if (ctrl.detec > ctrl.chaseThr) {
			return new AIChase();
		} else if (ctrl.detec < ctrl.alertThr) {
			return new AIPatrol(ctrl);
		} else if (_nearTarget) {
			return new AIPatrolAlert(ctrl);
		}
		return null;
	}
}

public class AIPatrolAlert : AIState {
	const float PATROL_JITTER = 2.5f;

	public AIPatrolAlert(AIController ctrl) {
		var mat = ctrl.transform.GetComponent<Renderer>().material;

		mat.SetColor("_Color", Color.yellow);
		mat.SetColor("_EmissionColor", Color.yellow);
	}

	// keep generating random points for the agent to go (navmeshagent will
	// handle clipping) until it changes to another state.
	public override void Do(AIController ctrl) {
		if (!ctrl.agent.pathPending && ctrl.agent.remainingDistance < 0.5f) {
			var jitter = new Vector3(
					Random.Range(-PATROL_JITTER, PATROL_JITTER), 0f,
					Random.Range(-PATROL_JITTER, PATROL_JITTER));
			ctrl.agent.SetDestination(ctrl.transform.position + jitter);
		}
	}

	public override AIState Change(AIController ctrl) {
		if (ctrl.detec > ctrl.chaseThr) return new AIChase();
		else if (ctrl.detec < ctrl.alertThr) return new AIPatrol(ctrl);
		return null;
	}
}

public class AIAttack : AIState {
	public override void Do(AIController ctrl) { }
	public override AIState Change(AIController ctrl) { return null; }
}
