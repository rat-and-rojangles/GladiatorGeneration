using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour {
	public AudioSource musicA;
	public AudioSource musicB;

	/// <summary>
	/// 0 is a, 1 is b
	/// </summary>
	[Range (0f, 1f)]
	public float crossAmount = 1f;

	public static Crossfade staticRef;

	public static float fadeAmount {
		get { return staticRef.crossAmount; }
		set { staticRef.crossAmount = value; }
	}

	void Start () {
		staticRef = this;
		crossAmount = 1f;
	}

	void Update () {
		musicA.volume = crossAmount;
		musicB.volume = 1f - crossAmount;
	}
}
