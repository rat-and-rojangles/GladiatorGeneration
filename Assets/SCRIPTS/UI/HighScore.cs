using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScore : MonoBehaviour {

	private UnityEngine.UI.Text teqqxt;

	void OnEnable () {
		teqqxt = GetComponent<UnityEngine.UI.Text> ();
		teqqxt.text = "HACKNC HIGH SCORE: " + PlayerPrefs.GetInt ("HighScore", 0);
	}

	void Update () {
		teqqxt.color = new ColorHSV ((Time.time * 0.1f).Normalized01 (), 1f, 1f, 1f);
	}

	[ContextMenu ("reset")]
	private void ResetHighScore () {
		PlayerPrefs.SetInt ("HighScore", 0);
	}
}
