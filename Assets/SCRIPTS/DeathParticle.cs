using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticle : MonoBehaviour {
	private new static ParticleSystem particleSystem;
	public static void PlayEffect (Vector3 location, Color color) {
		particleSystem.transform.position = location;
		var settings = particleSystem.main;
		settings.startColor = color;
		particleSystem.Play ();
	}

	void Start () {
		particleSystem = GetComponent<ParticleSystem> ();
	}
}
