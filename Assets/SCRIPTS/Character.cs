using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	[SerializeField]
	private ControlCharacter controller;
	[SerializeField]
	private new Rigidbody2D rigidbody;


	public bool grounded { get; }

	public float runSpeed = 1f;
	public float jumpHeight = 4f;

	private float jumpVelocity {
		get { return Mathf.Sqrt (2f * jumpHeight * Physics2D.gravity.y); }
	}

	void Update () {
		FrameAction actions = controller.GetActions ();
		Vector2 velocityChange = Vector2.zero;
		velocityChange.x += actions.moveDirection * runSpeed * Time.deltaTime;
		if (grounded && actions.jump) {
			velocityChange.y = jumpVelocity;
		}
	}
}
