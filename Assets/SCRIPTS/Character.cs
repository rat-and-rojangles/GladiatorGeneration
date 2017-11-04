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
			Collider2D leftCollision = Physics2D.OverlapPoint (transform.TransformPoint (groundCheckPointLeftLocal));
			Collider2D rightCollision = Physics2D.OverlapPoint (transform.TransformPoint (groundCheckPointRightLocal));
			return (leftCollision != null && leftCollision != collider) || (rightCollision != null && rightCollision != collider);
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
		groundCheckPointLeftLocal = collider.offset + Vector2.down * collider.size.y * 0.55f + Vector2.left * collider.size.x * 0.5f;
		groundCheckPointRightLocal = collider.offset + Vector2.down * collider.size.y * 0.55f + Vector2.right * collider.size.x * 0.5f;
	}

	void Update () {
		actions = controller.GetActions ();
	}

	void OnDrawGizmos () {
		Gizmos.DrawSphere (transform.TransformPoint (groundCheckPointLeftLocal), 0.25f);
		Gizmos.DrawSphere (transform.TransformPoint (groundCheckPointRightLocal), 0.25f);
	}

	void FixedUpdate () {
		OnScreenConsole.Log (grounded);
		Vector2 velocity = rigidbody.velocity;
		velocity.x = runSpeed * actions.moveDirection;
		if (grounded && actions.jump) {
			velocity.y = jumpVelocity;
		}
		rigidbody.velocity = velocity;
	}
}
