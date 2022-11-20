using UnityEngine;

// TODO: this could be changed to an AreaStim with a public key/radius, 
// thus allowing code reuse for e.g. grenades or other "explosion" type things.
public class Water : Stim.Stim {
    // We give this to the response so it knows where the object came from.
    public Vector3 dir;

    void Awake() {
        this.key = Stim.Key.Water;
    }

    void FixedUpdate() {
        // We record forward every fixed update because when the collision
        // happens, the forward vector is likely FUBAR.
        // (This is _very_ fragile, honestly.)
        dir = transform.forward;
    }

    void OnCollisionEnter() {
        Collider[] cols = Physics.OverlapSphere(transform.position, 1);

        foreach (Collider c in cols) {
            var s = c.gameObject.GetComponent<Stim.Response>();
            if (s != null) {
                s.Respond(this);
            }
        }
        Destroy(gameObject);
    }

    void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, this.dir);
    }
}
