using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentPrototype : MonoBehaviour {
	private static bool alreadyInstantiated = false;
	[SerializeField]
	private GameObject prefab;
	void Awake () {
		if (!alreadyInstantiated) {
			alreadyInstantiated = true;
			DontDestroyOnLoad (Instantiate (prefab));
			Destroy (gameObject);
		}
	}
}
