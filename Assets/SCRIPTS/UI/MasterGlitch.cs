using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterGlitch : MonoBehaviour {
	private RawImage m_image;

	private static MasterGlitch staticRef;

	private float duration = -1;
	private float timeElapsed;

	[SerializeField]
	private Material glitchMaterial;

	// Use this for initialization
	void Start () {
		staticRef = this;
		m_image = GetComponent<RawImage> ();
	}

	void Update () {
		if (m_image.material != null) {
			if (timeElapsed < duration) {
				timeElapsed += Time.deltaTime;
			}
			else {
				m_image.material = null;
			}
		}

	}

	public static void Glitch (float duration) {
		staticRef.timeElapsed = 0f;
		staticRef.m_image.material = staticRef.glitchMaterial;
		staticRef.duration = duration;
	}

	public static void FadeToBlack (float fadeDuration) {
		Glitch (fadeDuration);
		staticRef.StartCoroutine (staticRef.FadeToBlackHelper (fadeDuration));
	}

	private IEnumerator FadeToBlackHelper (float fadeDuration) {
		Color initialColor = m_image.color;
		float fadeTimeElapsed = 0f;
		bool alreadyStartedScoreboard = false;
		while (fadeTimeElapsed < fadeDuration) {
			fadeTimeElapsed += Time.deltaTime;
			float ratio = fadeTimeElapsed / fadeDuration;
			if (!alreadyStartedScoreboard && ratio > 0.25f) {
				alreadyStartedScoreboard = true;
				Scoreboard.FrontAndCenter (fadeDuration * 0.5f);
			}
			m_image.color = Interpolation.Interpolate (initialColor, Color.black, ratio, InterpolationMethod.SquareRoot);
			yield return null;
		}
	}
}
