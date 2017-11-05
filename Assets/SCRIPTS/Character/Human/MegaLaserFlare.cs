using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaLaserFlare : MonoBehaviour {

	public float launchDuration = 1f;
	private float timeElapsed = 0f;

	private float openFraction = 0.33f;
	private float closeFraction = 0.1f;

	private bool m_firing = false;
	public bool firing {
		get { return m_firing; }
	}

	[SerializeField]
	private Rigidbody2D playerRB;

	[SerializeField]
	private Character player;

	public float recoilStrength = 10f;

	[ContextMenu("fire in the hole")]
	public void Fire () {
		player.weakened = true;
		m_firing = true;
		transform.localScale = new Vector3 (1f, 0f, 1f);
		gameObject.SetActive (true);
		timeElapsed = 0f;
		CamShake.Shake (launchDuration, 0.5f);
		StopAllCoroutines ();
		StartCoroutine (FiringRoutine ());
	}

	private IEnumerator FiringRoutine () {
		playerRB.AddForce (transform.right * -recoilStrength, ForceMode2D.Impulse);
		float openTime = launchDuration * openFraction;
		float closeStart = launchDuration - launchDuration * closeFraction;
		float closeDuration = launchDuration * closeFraction;
		while (timeElapsed < launchDuration) {
			timeElapsed += Time.deltaTime;
			float ratio = timeElapsed / launchDuration;
			if (ratio < openTime) {
				transform.localScale = new Vector3 (1f, Interpolation.Interpolate (0f, 1f, ratio / openTime, InterpolationMethod.SquareRoot), 1f);
			}
			else if (ratio > closeStart) {
				player.weakened = false;
				transform.localScale = new Vector3 (1f, Interpolation.Interpolate (1f, 0f, (ratio - closeStart) / closeDuration, InterpolationMethod.Quadratic), 1f);
			}
			// playerRB.AddForce (transform.right * -recoilStrength);
			yield return null;
		}
		m_firing = false;
		gameObject.SetActive (false);
	}
}
