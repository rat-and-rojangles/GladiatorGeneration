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

	/// <summary>
	/// Cuts run speed and jump height.
	/// </summary>
	public bool weakened = false;

	private float derivedRunSpeed {
		get {
			return weakened ? runSpeed * 0.5f : runSpeed;
		}
	}
	private float derivedJumpVelocity {
		get {
			float maxJumpVel = Mathf.Sqrt (2f * jumpHeight * -Physics2D.gravity.y * rigidbody.gravityScale);
			if (weakened) {
				maxJumpVel *= 0.75f;
			}
			return maxJumpVel;
		}
	}

	public Vector2 velocity {
		get { return rigidbody.velocity; }
	}

	public bool dead {
		get { return !enabled; }
	}

	private FrameAction actions;

	void Start () {
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
		spriteRenderer = GetComponent<SpriteRenderer> ();
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

		if (actions.moveDirection == 0) {
			// rigidbody.AddForce (Vector2.right * derivedRunSpeed * 0.1f * -rigidbody.velocity.x.Sign (), ForceMode2D.Impulse);
			rigidbody.SetVelocity (new Vector2 (Mathf.Lerp (velocity.x, 0f, 5f * Time.fixedDeltaTime), velocity.y));
		}
		else {
			// rigidbody.AddForce (Vector2.right * derivedRunSpeed * actions.moveDirection, ForceMode2D.Impulse);
			rigidbody.SetVelocity (new Vector2 (derivedRunSpeed * actions.moveDirection, velocity.y));
		}

		if (actions.jump) {
			if (grounded) {
				Jump ();
			}
			else if (remainingJumps > 0) {
				remainingJumps--;
				Jump ();
			}
		}
		actions = FrameAction.NEUTRAL;
	}

	private void Jump () {
		rigidbody.SetVelocity (new Vector2 (velocity.x, derivedJumpVelocity));
		timeSinceLastJump = 0f;
	}

	public void OnTriggerStay2D (Collider2D other) {
		if (other.gameObject.activeSelf && controlType != ControlType.Human && enabled && !other.name.Equals ("Hurtbox")) {
			if (!other.CompareTag ("Laser")) {
				other.gameObject.SetActive (false);
			}
			Kill ();
			if (other.name.Equals ("Bullet")) {
				Destroy (other.gameObject);
			}
		}
	}
	public void Kill () {
		gameObject.SetActive (false);
		DeathParticle.PlayEffect (transform.position, color);
		Scoreboard.AddScore (1);
	}

	/// <summary>
	/// For ML enemies.
	/// </summary>
	public void RespawnAtPosition (Vector3 position) {
		gameObject.SetActive (true);
		StartCoroutine (RespawnAtPositionHelper (position));
	}
	private IEnumerator RespawnAtPositionHelper (Vector3 position) {
		enabled = false;
		rigidbody.isKinematic = true;
		Vector3 initialPosition = transform.position;
		float respawnTimeElapsed = 0f;
		transform.localScale = Vector3.one * 0.5f;
		while (respawnTimeElapsed < NeuralNetController.staticRef.respawnTime) {
			respawnTimeElapsed += Time.deltaTime;
			transform.position = Interpolation.Interpolate (initialPosition, position, respawnTimeElapsed / NeuralNetController.staticRef.respawnTime, InterpolationMethod.Quadratic);
			Crossfade.fadeAmount = respawnTimeElapsed / NeuralNetController.staticRef.respawnTime;
			yield return null;
		}
		transform.localScale = Vector3.one;
		enabled = true;
		rigidbody.isKinematic = false;
	}
}
