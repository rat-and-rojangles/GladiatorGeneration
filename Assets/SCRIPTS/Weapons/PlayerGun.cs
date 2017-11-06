using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

	public LooseFollow follow;
	public float leadMinDistance = 10f;
	public float leadMaxDistance = 20f;

	public float fullChargeDuration = 1f;
	private float chargeTimeElapsed = 0f;

	private float chargeRatio {
		get { return Mathf.Clamp01 (chargeTimeElapsed / fullChargeDuration); }
	}

	public float bulletsPerFullCharge = 6f;
	private float bulletChargeDrain {
		get { return fullChargeDuration / bulletsPerFullCharge; }
	}

	[SerializeField]
	private Transform chargeMeterY;

	[SerializeField]
	private MegaLaserFlare flare;

	[SerializeField]
	private GameObject bulletPrefab;


	private Vector2 camLead = Vector2.zero;

	[SerializeField]
	private Character player;

	private float timeSinceMaxCharge = -1f;
	public float flareChargeTime = 1f;

	void Start () {
		chargeTimeElapsed = fullChargeDuration;
	}

	void Update () {
		Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 diff = mouseWorldPoint - transform.position;

		Vector3 normalDiff = diff.normalized;
		float rot_z = Mathf.Atan2 (normalDiff.y, normalDiff.x) * Mathf.Rad2Deg;

		if (!flare.firing) {
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z);
		}
		else {
			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0f, 0f, rot_z), 0.5f * Time.deltaTime);
		}

		if (!Input.GetMouseButton (1) || Input.GetKey (KeyCode.LeftShift)) {
			camLead = Vector2.zero;
		}
		else {
			float leadFactor = Mathf.Lerp (leadMinDistance, leadMaxDistance, (diff.magnitude - leadMinDistance) / (leadMaxDistance - leadMinDistance)) - leadMinDistance;
			camLead = transform.right * leadFactor;
		}

		follow.offset = camLead;
		if (!flare.firing) {
			player.weakened = Input.GetMouseButton (0);
			float increment = Input.GetMouseButton (0) ? Time.deltaTime * 4f : Time.deltaTime;
			chargeTimeElapsed = Mathf.Clamp (chargeTimeElapsed + increment, 0f, fullChargeDuration);
			if (timeSinceMaxCharge >= 0f) {
				timeSinceMaxCharge += Time.deltaTime;
			}
			else if (Input.GetMouseButton (0) && chargeRatio > 0.99f && timeSinceMaxCharge < 0f) {
				timeSinceMaxCharge = 0f;
			}

			if (Input.GetMouseButton (0) && chargeRatio > 0.99f && timeSinceMaxCharge > flareChargeTime) {
				CamShake.Shake (0.1f, Mathf.Clamp ((timeSinceMaxCharge - flareChargeTime) * 0.25f, 0f, 0.25f));
				SoundCatalog.SetNoiseVolume (Mathf.Clamp01 (timeSinceMaxCharge - flareChargeTime));
			}
			else {
				SoundCatalog.SetNoiseVolume (0f);
			}

			if (Input.GetMouseButtonUp (0)) {
				if (chargeRatio > 0.99f && timeSinceMaxCharge > flareChargeTime) {
					flare.Fire ();
				}
				else if (chargeTimeElapsed > bulletChargeDrain) {
					SoundCatalog.PlayGunshotSound ();
					GameObject bullet = Instantiate (bulletPrefab, transform.position, transform.rotation);
					bullet.name = "Bullet";
					bullet.GetComponent<Rigidbody2D> ().velocity = transform.right * 50f;
					Destroy (bullet, 5f);
					CamShake.Shake (0.1f, 0.21f);
					chargeTimeElapsed = Mathf.Clamp (chargeTimeElapsed - bulletChargeDrain, 0f, fullChargeDuration);
				}
				timeSinceMaxCharge = -1f;
			}
		}
		else {
			chargeTimeElapsed = Mathf.Clamp (chargeTimeElapsed - Time.deltaTime * 12f, 0f, fullChargeDuration);
		}

		// if (!flare.firing && Input.GetMouseButton (0) && (chargeTimeElapsed < fullChargeDuration)) {
		// 	chargeTimeElapsed += Time.deltaTime;
		// 	player.weakened = true;
		// }
		// else if (Input.GetMouseButtonUp (0)) {
		// 	player.weakened = false;
		// 	if (chargeRatio > 0.99f) {
		// 		flare.Fire ();
		// 	}
		// 	else if (chargeRatio > 0.25f) {
		// 		GameObject bullet = Instantiate (bulletPrefab, transform.position, transform.rotation);
		// 		bullet.name = "Bullet";
		// 		bullet.GetComponent<Rigidbody2D> ().velocity = transform.right * 50f;
		// 		Destroy (bullet, 5f);
		// 		CamShake.Shake (0.1f, 0.1f);
		// 	}
		// 	chargeTimeElapsed = 0f;
		// }
		chargeMeterY.transform.localScale = new Vector3 (1f, Mathf.Lerp (0f, 1f, chargeRatio), 1f);
	}
}
