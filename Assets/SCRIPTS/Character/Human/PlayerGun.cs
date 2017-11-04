using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

	public LooseFollow follow;
	public float leadMinDistance = 10f;
	public float leadMaxDistance = 20f;

	void Update () {
		Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 diff = mouseWorldPoint - transform.position;
		Vector3 normalDiff = diff.normalized;

		float rot_z = Mathf.Atan2 (normalDiff.y, normalDiff.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (0f, 0f, rot_z);

		// float leadFactor = Mathf.Lerp (leadMinDistance, leadMaxDistance, (diff.magnitude - leadMinDistance) / (leadMaxDistance - leadMinDistance)) - leadMinDistance;
		// follow.offset = transform.right * leadFactor;
		// print ((diff.magnitude - leadMinDistance) / (leadMaxDistance - leadMinDistance));
	}
}
