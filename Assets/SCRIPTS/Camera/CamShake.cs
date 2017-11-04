using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

	public float duration = 0.25f;
	private float timeElapsed = 1f;
	public float strength = 1f;

	public void Shake () {
		timeElapsed = 0f;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			Shake ();
		}
		transform.localPosition = Random.insideUnitCircle * Mathf.Lerp (strength, 0f, timeElapsed / duration);
		timeElapsed += Time.deltaTime;
	}
}
