using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lives : MonoBehaviour {

	public bool infiniteLives = false;

	public int lives = 3;

	public UnityEngine.UI.Image [] lifeImages;

	[SerializeField]
	private Sprite deadIcon;

	private static Lives staticRef;

	void Start () {
		staticRef = this;
		if (infiniteLives) {
			staticRef.lifeImages [0].sprite = staticRef.deadIcon;
			staticRef.lifeImages [0].color = Color.gray;
			staticRef.lifeImages [2].sprite = staticRef.deadIcon;
			staticRef.lifeImages [2].color = Color.gray;
		}
	}

	public static bool dead {
		get { return staticRef.lives <= 0; }
	}

	public static void LoseLife (float glitchTime) {
		if (!staticRef.infiniteLives && staticRef.lives > 0) {
			staticRef.lives--;
			staticRef.lifeImages [2 - staticRef.lives].sprite = staticRef.deadIcon;
			staticRef.lifeImages [2 - staticRef.lives].color = Color.gray;
		}

		if (staticRef.lives == 0) {
			NeuralNetController.staticRef.gameObject.SetActive (false);
			Crossfade.fadeAmount = 0f;
			MasterGlitch.FadeToBlack (2f);
			SoundCatalog.PlayDeathSound ();
		}
		else {
			SoundCatalog.PlayPainSound ();
			MasterGlitch.Glitch (glitchTime);
		}
	}
}
