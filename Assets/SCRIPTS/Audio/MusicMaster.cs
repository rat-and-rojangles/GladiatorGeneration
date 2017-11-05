using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMaster : MonoBehaviour {

	public static AudioSource musicSource;

	void Start () {
		musicSource = GetComponent<AudioSource> ();
	}
}
