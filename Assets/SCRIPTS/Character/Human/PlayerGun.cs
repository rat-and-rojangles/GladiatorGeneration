using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

	public LooseFollow follow;
	public float leadMinDistance = 10f;
	public float leadMaxDistance = 20f;

	public float fullChargeDuration = 1f;
	private float chargeTimeElapsed = 0f;

	public KeyCode lookKey;

	private float chargeRatio {
		get { return chargeTimeElapsed / fullChargeDuration; }
	}

	[SerializeField]
	private Transform chargeMeterY;

	[SerializeField]
	private MegaLaserFlare flare;

	private Vector2 camLead = Vector2.zero;

	[SerializeField]
	private Character player;

	void Update () {
		if (chargeTimeElapsed < fullChargeDuration) {
			chargeTimeElapsed += Time.deltaTime;
			chargeMeterY.transform.localScale = new Vector3 (1f, Mathf.Lerp (0f, 1f, chargeRatio), 1f);
		}

		player.blockInput = flare.firing;
		if (!flare.firing) {
			Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 diff = mouseWorldPoint - transform.position;
			Vector3 normalDiff = diff.normalized;
			float rot_z = Mathf.Atan2 (normalDiff.y, normalDiff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z);
			if (!Input.GetKey (lookKey)) {
				camLead = Vector2.zero;
			}
			else if (Input.GetKey (lookKey)) {
				float leadFactor = Mathf.Lerp (leadMinDistance, leadMaxDistance, (diff.magnitude - leadMinDistance) / (leadMaxDistance - leadMinDistance)) - leadMinDistance;
				camLead = transform.right * leadFactor;
			}
		}

		follow.offset = camLead;

		//

		if (Input.GetMouseButtonDown (0)) {
			//CamShake.Shake (0.1f, chargeRatio * 0.5f);
			flare.Fire ();
			chargeTimeElapsed = 0f;
		}
	}
}
