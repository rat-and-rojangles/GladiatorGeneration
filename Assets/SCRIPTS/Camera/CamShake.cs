using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

	public float duration = 0.25f;
	private float timeElapsed = 0f;
	public float strength = 1f;

	private void Shake () {
		// transform.position = Random.insideUnitCircle * Mathf.Lerp(0f,strength)
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {

		}
	}
}
