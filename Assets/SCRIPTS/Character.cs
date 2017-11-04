using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	private enum ControlType { Human, AI }
	[SerializeField]
	private ControlType controlType;


	private ControlCharacter controller;
	private new Rigidbody2D rigidbody;
	private new BoxCollider2D collider;


	private Vector2 groundCheckPointLeftLocal;
	private Vector2 groundCheckPointRightLocal;
	public bool grounded {
		get {
			return Physics2D.OverlapPoint (transform.TransformPoint (groundCheckPointLeftLocal)) != null || Physics2D.OverlapPoint (transform.TransformPoint (groundCheckPointRightLocal)) != null;
		}
	}

	public float runSpeed = 1f;
	public float jumpHeight = 4f;

	private float jumpVelocity {
		get { return Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y); }
	}

	private FrameAction actions;

	void Start () {
		if (controlType == ControlType.Human) {
			controller = new ControlCharacterHuman ();
		}
		else {
			throw new System.NotImplementedException ();
		}
		collider = GetComponent<BoxCollider2D> ();
		rigidbody = GetComponent<Rigidbody2D> ();
		groundCheckPointLeftLocal = collider.offset + Vector2.down * collider.size.y * 0.55f + Vector2.left * collider.size.x * 0.25f;
		groundCheckPointRightLocal = collider.offset + Vector2.down * collider.size.y * 0.55f + Vector2.right * collider.size.x * 0.25f;
	}

	void Update () {
		actions = controller.GetActions ();
	}

	void FixedUpdate () {
		Vector2 velocity = rigidbody.velocity;
		velocity.x = runSpeed * actions.moveDirection;
		if (grounded && actions.jump) {
			velocity.y = jumpVelocity;
		}
		rigidbody.velocity = velocity;
	}
}
