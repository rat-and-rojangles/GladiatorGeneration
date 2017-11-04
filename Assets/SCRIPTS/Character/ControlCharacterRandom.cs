using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacterRandom : ControlCharacter {
	private FrameAction action;
	public FrameAction GetActions () {
		if (Random.value < 0.1) {
			action = new FrameAction (UnityEngine.Random.Range (-1, 2), UnityEngine.Random.value < 0.1f);
		}
		return action;
	}
}
