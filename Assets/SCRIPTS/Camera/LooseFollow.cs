using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseFollow : MonoBehaviour {

	public Transform target;
	public float speed;
	public Vector2 offset;
	private Vector2 derivedOffset {
		get { return new Vector2 (offset.x, offset.y / 1.5f); }
	}

	void Update () {
		Vector3 position = Vector3.Lerp (transform.position, target.position + (Vector3)derivedOffset, speed * Time.deltaTime);
		position.z = transform.position.z;
		transform.position = position;
	}
}
