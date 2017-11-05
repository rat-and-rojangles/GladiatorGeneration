using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	private static LayerMask mask {
		get { return -257; }
	}

	private enum ControlType { Human, Random, ML }
	[SerializeField]
	private ControlType controlType;


	private ControlCharacter controller;
	private new Rigidbody2D rigidbody;
	private new BoxCollider2D collider;

	public float timeSinceLastJump = 0f;

	private Vector2 groundCheckPointLeftLocal;
	private Vector2 groundCheckPointRightLocal;
	public bool grounded {
		get {
			Collider2D leftCollision = Physics2D.OverlapPoint (transform.TransformPoint (groundCheckPointLeftLocal), mask);
			Collider2D rightCollision = Physics2D.OverlapPoint (transform.TransformPoint (groundCheckPointRightLocal), mask);
			return leftCollision != null | rightCollision != null;
		}
	}

	private SpriteRenderer spriteRenderer;
	public Color color {
		get { return spriteRenderer.color; }
		set { spriteRenderer.color = value; }
	}

	public float runSpeed = 1f;
	public float jumpHeight = 4f;
	public int numberOfJumps = 2;
	private int remainingJumps = 0;

	private float jumpVelocity {
		get { return Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y * rigidbody.gravityScale); }
	}

	public Vector2 velocity {
		get { return rigidbody.velocity; }
	}

	private FrameAction actions;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (controlType) {
			case ControlType.Human:
				controller = new ControlCharacterHuman ();
				try {
					NeuralNetController.staticRef.RegisterPlayer (this);
				}
				catch {

				}
				break;
			case ControlType.Random:
				controller = new ControlCharacterRandom ();
				break;
			default:
				controller = new ControlCharacterML (this);
				break;
		}
		collider = GetComponent<BoxCollider2D> ();
		rigidbody = GetComponent<Rigidbody2D> ();
		groundCheckPointLeftLocal = collider.offset + Vector2.down * collider.size.y * 0.55f + Vector2.left * collider.size.x * 0.5f;
		groundCheckPointRightLocal = collider.offset + Vector2.down * collider.size.y * 0.55f + Vector2.right * collider.size.x * 0.5f;
	}

	void Update () {
		timeSinceLastJump += Time.deltaTime;
		actions = actions.Combined (controller.GetActions ());
	}

	void FixedUpdate () {
		if (grounded) {
			remainingJumps = numberOfJumps - 1;
		}
		Vector2 velocity = rigidbody.velocity;
		velocity.x = runSpeed * actions.moveDirection;
		if (actions.jump) {
			if (grounded) {
				velocity.y = jumpVelocity;
				timeSinceLastJump = 0f;
			}
			else if (remainingJumps > 0) {
				remainingJumps--;
				velocity.y = jumpVelocity;
				timeSinceLastJump = 0f;
			}
		}
		rigidbody.velocity = velocity;
		actions = FrameAction.NEUTRAL;
	}
}
