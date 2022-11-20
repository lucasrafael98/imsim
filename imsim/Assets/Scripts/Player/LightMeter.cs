using System.Collections;
using UnityEngine;

public class LightMeter : MonoBehaviour {
	public Camera cam;
	public Material mat;
	public Renderer gem;
	public float updTime = 0.1f;

	public float val;

	private int texSize = 256;
	private Texture2D tex;
	private Rect rect;
	private RenderTexture renderTex;
	private Material gemMat;

	void Awake() {
		tex = new Texture2D(texSize, texSize);
		renderTex = new RenderTexture(texSize, texSize, 24);
		rect = new Rect(0f, 0f, texSize, texSize);

		gem.GetComponent<Renderer>().material = new Material(mat);
		gemMat = gem.GetComponent<Renderer>().material;

		StartCoroutine(LightDetectionUpdate(updTime));
	}

	private IEnumerator LightDetectionUpdate(float updTime) {
		while (true) {
			cam.enabled = true;
			//Set the target texture of the cam.
			cam.targetTexture = renderTex;
			//Render into the set target texture.
			cam.Render();

			//Set the target texture as the active rendered texture.
			RenderTexture.active = renderTex;
			//Read the active rendered texture.
			tex.ReadPixels(rect, 0, 0, false);
			tex.Apply();

			//Reset the active rendered texture.
			RenderTexture.active = null;
			//Reset the target texture of the cam.
			cam.targetTexture = null;
			cam.enabled = false;

			//Read the pixel in middle of the texture.
			Color c = tex.GetPixel(texSize / 2, texSize / 2);

			float h, s;
			Color.RGBToHSV(c, out h, out s, out val);
			// even in full darkness, slightly raise detection.
			val = Mathf.Clamp(val, 0.05f, 1f);
			Debug.Log(val);

			Color gemColor = new Color(0.0f, 0.8f, 0.9f, 1f);
			gemMat.SetColor("_EmissiveColor", gemColor * val);
			yield return new WaitForSeconds(updTime);
		}
	}
}
