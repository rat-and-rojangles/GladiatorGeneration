using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scoreboard : MonoBehaviour {
	private static Scoreboard staticRef;

	[SerializeField]
	private HighScore highScore;

	private Text text;
	private int generations = 1;
	private int score = 0;


	/// <summary>
	/// Automatically increases score.
	/// </summary>
	public static void AddGeneration () {
		staticRef.generations++;
		AddScore (staticRef.generations);
	}
	public static void AddScore (int score) {
		if (!Lives.dead) {
			staticRef.score += score;
		}
		staticRef.UpdateText ();
	}

	private void UpdateText () {
		text.text = "GENERATIONS: " + generations + "\nSCORE: " + score;
	}

	void Start () {
		staticRef = this;
		text = GetComponent<Text> ();
	}

	void Update () {
		text.color = new ColorHSV ((Time.time * 0.1f).Normalized01 (), 1f, 1f, 1f);
		if (Input.GetKeyDown (KeyCode.R)) {
			transform.localPosition = Vector3.zero;
		}
	}

	public static void FrontAndCenter (float moveDuration) {
		staticRef.StartCoroutine (staticRef.FrontAndCenterHelper (moveDuration));
	}

	private IEnumerator FrontAndCenterHelper (float moveDuration) {
		Vector3 magicPos = new Vector3 (600f, 100f, 0f);
		Vector3 magicScale = Vector3.one * 2f;
		Vector3 initialPos = transform.localPosition;
		Vector3 initialScale = transform.localScale;
		float timeElapsed = 0f;
		while (timeElapsed < moveDuration) {
			timeElapsed += Time.deltaTime;
			transform.localPosition = Interpolation.Interpolate (initialPos, magicPos, timeElapsed / moveDuration, InterpolationMethod.SquareRoot);
			transform.localScale = Interpolation.Interpolate (initialScale, magicScale, timeElapsed / moveDuration, InterpolationMethod.SquareRoot);
			if (timeElapsed / moveDuration > 0.5f) {
				if (!Lives.staticRef.infiniteLives && score > PlayerPrefs.GetInt ("HighScore", 0)) {
					PlayerPrefs.SetInt ("HighScore", score);
				}
				highScore.gameObject.SetActive (true);
			}
			yield return null;
		}
		while (!Input.GetMouseButtonDown (0)) {
			yield return null;
		}
		Crossfade.fadeAmount = 1f;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}
