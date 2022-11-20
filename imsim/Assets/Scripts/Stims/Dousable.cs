using UnityEngine;

public class Dousable : Stim.Response {
    private Light l;

    void Awake() {
        stims = new();
        stims.Add(Stim.Key.Water, WaterResponse);
        l = gameObject.GetComponent<Light>();
    }

    private void WaterResponse(Stim.Stim s) {
        l.intensity = 0;
    }
}
