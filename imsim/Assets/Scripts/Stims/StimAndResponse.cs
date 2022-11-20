using System.Collections.Generic;
using System;
using UnityEngine;

namespace Stim {
	public enum Key {
		Water,
		Stun
	}

	///<summary>
	/// Stims contain a key which identifies the stim type.
	///</summary>
	public abstract class Stim : MonoBehaviour {
		public Key key;
	}

	///<summary>
	/// Responses have a collection of stims to which they respond.
	/// The response is modelled as an Action<Stim>, allowing each stim to
	/// have relevant data (e.g. damage, intensity, etc.)
	///</summary>
	public abstract class Response : MonoBehaviour {
		public Dictionary<Key, Action<Stim>> stims;

		public void Respond(Stim s) {
			if (stims.ContainsKey(s.key)) {
				stims[s.key](s);
			}
		}
	}
}

