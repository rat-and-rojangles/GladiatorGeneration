using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {


	public float invincibilityTime = 1f;

	[SerializeField]
	private Character player;

	private bool vulnerable = true;

	public void OnTriggerEnter2D (Collider2D other) {
		if (vulnerable && other != null && other.CompareTag ("Enemy")) {
			vulnerable = false;
			Lives.LoseLife (invincibilityTime * 0.33f);
			StartCoroutine (Invincible ());
		}
	}

	private IEnumerator Invincible () {
		float timeElapsed = 0f;
		Color initialColor = player.color;
		while (timeElapsed < invincibilityTime) {
			timeElapsed += Time.deltaTime;
			ColorHSV newColor = initialColor;
			newColor.v = (timeElapsed / invincibilityTime * 3f).Normalized01 ();
			player.color = newColor;
			yield return null;
		}
		player.color = initialColor;
		vulnerable = true;
		if (Lives.dead) {
			player.Kill ();
		}
	}
}
