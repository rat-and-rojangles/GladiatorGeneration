using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeCue : MonoBehaviour {

	[SerializeField]
	private AudioClip newSong;

	void Start () {
		if (MusicMaster.musicSource.clip != newSong) {
			MusicMaster.musicSource.clip = newSong;
		}
	}

}
