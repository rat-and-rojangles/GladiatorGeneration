using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All of the things a player can do per frame.
/// </summary>
public struct FrameAction {
	public int moveDirection;
	public bool jump;

	public FrameAction (int moveDirection, bool jump) {
		this.moveDirection = moveDirection;
		this.jump = jump;
	}

	/// <summary>
	/// Overwrites movements, ORs jump
	/// </summary>
	public FrameAction Combined (FrameAction other) {
		return new FrameAction (other.moveDirection, jump || other.jump);
	}

	private static FrameAction m_NEUTRAL = new FrameAction (0, false);
	public static FrameAction NEUTRAL {
		get { return m_NEUTRAL; }
	}

	// /// <summary>
	// /// Gun angle
	// /// </summary>
	// public float angle;
	// public bool shoot;
}
