using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Environment Objects/Trampoline"),
	DisallowMultipleComponent,
	PlayModeOnly,
	RequireComponent(typeof(EnvironmentObject))]
	public class Trampoline : MonoBehaviour {
		float bumpAmplitude = 0.5f;
		float bumpTime = 0.05f;

		float bumpAwait = 0.1f;
		float returnAwait = 0.25f;

		public float boostForce = 12500f;

		[Header("Components")]
		public Transform plate;

		EnvironmentObject environmentObject;
		Level level;

		float sinceBumpRequested = -100;

		void Start() {
			environmentObject = GetComponent<EnvironmentObject>();
			level = GetComponentInParent<Level>();
		}

		void Update() {
			if (sinceBumpRequested > -0.5) {
				sinceBumpRequested += Time.deltaTime;
				if (sinceBumpRequested > bumpAwait) {
					plate.localPosition += Vector3.up * (bumpAmplitude * (Time.deltaTime / bumpTime));
					if (plate.localPosition.y >= bumpAmplitude) {
						plate.localPosition = new Vector3(plate.localPosition.x, bumpAmplitude, plate.localPosition.z);
						sinceBumpRequested = -1;
					}
				}
			} else {
				sinceBumpRequested -= Time.deltaTime;
				if ((sinceBumpRequested + 1) < -returnAwait) {
					plate.localPosition = new Vector3(plate.localPosition.x, plate.localPosition.y * 0.5f, plate.localPosition.z);
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
			if (rigid == null) {
				rigid = other.GetComponentInParent<Rigidbody2D>();
			}
			if (rigid != null) {
				sinceBumpRequested = 0;
			}
			rigid.velocity = new Vector2(rigid.velocity.x, 0);
			rigid.AddForce(Vector2.up * boostForce);
		}
	}
}

