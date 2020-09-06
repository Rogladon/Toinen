using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Environment Objects/Floor Button"),
	DisallowMultipleComponent,
	PlayModeOnly,
	RequireComponent(typeof(EnvironmentObject))]
	public class FloorButton : MonoBehaviour {
		public float plateAmplitude = 0.1f;
		public float pressingSpeed = 0.5f;

		[Header("Components")]
		public Transform plate;

		int enters = 0;
		public bool isTriggered => enters > 0;

		public bool isPressed { get; private set; }

		EnvironmentObject environmentObject;
		Level level;

		void Start() {
			environmentObject = GetComponent<EnvironmentObject>();
			level = GetComponentInParent<Level>();
		}

		void Update() {
			if (enters < 0) {
				Debug.LogWarning("enters < 0! Reseting to 0");
				enters = 0;
			}
			if (isTriggered) {
				if (plate.localPosition.y > -plateAmplitude)
					plate.localPosition -= Vector3.up * (pressingSpeed * plateAmplitude);
				else
					level.colorTriggers.Trigger(environmentObject.color);
			} else {
				if (plate.localPosition.y < 0)
					plate.localPosition += Vector3.up * (pressingSpeed * plateAmplitude);
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
			if (rigid == null) {
				rigid = other.GetComponentInParent<Rigidbody2D>();
			}
			if (rigid != null) {
				enters++;
			}
		}

		void OnTriggerExit2D(Collider2D other) {
			Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
			if (rigid == null) {
				rigid = other.GetComponentInParent<Rigidbody2D>();
			}
			if (rigid != null) {
				enters--;
			}
		}
	}
}
