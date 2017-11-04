using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directs a player
/// </summary>
public interface ControlCharacter {
	FrameAction GetActions ();
}
