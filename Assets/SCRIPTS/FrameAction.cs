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


	// /// <summary>
	// /// Gun angle
	// /// </summary>
	// public float angle;
	// public bool shoot;
}
