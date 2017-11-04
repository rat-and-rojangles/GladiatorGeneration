using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacterHuman : ControlCharacter {
	public FrameAction GetActions () {
		return new FrameAction (Mathf.RoundToInt (Input.GetAxis ("Horizontal")), Input.GetButtonDown ("Jump"));
	}
}
