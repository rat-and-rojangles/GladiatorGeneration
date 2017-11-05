using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

	private float duration;
	private float timeElapsed = 1f;
	private float strength = 1f;

	private static CamShake staticRef;

	void Awake () {
		staticRef = this;
	}
	void OnDestroy () {
		staticRef = null;
	}

	/// <summary>
	/// Shake the camera.
	/// </summary>
	public static void Shake (float duration, float strength) {
		staticRef.H_Shake (duration, strength);
	}

	private void H_Shake (float duration, float strength) {
		timeElapsed = 0f;
		this.duration = duration;
		this.strength = strength;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			H_Shake (0.25f, 1f);
		}
		transform.localPosition = Random.insideUnitCircle * Mathf.Lerp (strength, 0f, timeElapsed / duration);
		timeElapsed += Time.deltaTime;
	}
}
