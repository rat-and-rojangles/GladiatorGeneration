using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacterML : ControlCharacter {
	public static float RESPAWN_TIME {
		get { return 0.5f; }
	}
	private Character m_character;
	public Character character { get { return m_character; } }
	private FrameAction actions;
	public ControlCharacterML (Character character) {
		m_character = character;
		NeuralNetController.staticRef.RegisterEnemy (this);
	}

	public void UpdateActions (FrameAction actions) {
		this.actions = actions;
	}

	public FrameAction GetActions () {
		return actions;
	}
}
