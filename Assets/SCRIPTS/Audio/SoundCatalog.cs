using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCatalog : MonoBehaviour {
	[SerializeField]
	private AudioSource source;
	[SerializeField]
	private AudioClip gunSound;
	[SerializeField]
	private AudioClip groanSound;
	[SerializeField]
	private AudioClip painSound;
	[SerializeField]
	private AudioClip deathSound;
	[SerializeField]
	private AudioClip generationSound;

	[SerializeField]
	private AudioSource noiseSource;

	private static SoundCatalog staticRef;
	void Start () {
		staticRef = this;
	}

	public static void PlayGunshotSound () {
		staticRef.source.PlayOneShot (staticRef.gunSound);
	}
	public static void PlayGroanSound () {
		staticRef.source.PlayOneShot (staticRef.groanSound);
	}
	public static void PlayPainSound () {
		staticRef.source.PlayOneShot (staticRef.painSound);
	}
	public static void PlayDeathSound () {
		staticRef.source.PlayOneShot (staticRef.deathSound);
	}
	public static void PlayGenerationSound () {
		staticRef.source.PlayOneShot (staticRef.generationSound, 0.25f);
	}

	public static void SetNoiseVolume (float volume) {
		staticRef.noiseSource.volume = volume;
	}
}
