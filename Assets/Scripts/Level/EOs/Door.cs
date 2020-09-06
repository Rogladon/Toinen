using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Environment Objects/Door"),
	DisallowMultipleComponent,
	PlayModeOnly,
	RequireComponent(typeof(EnvironmentObject))]
	public class Door : MonoBehaviour {
		public float amplitude = 1f;
		public float speed = 0.05f;

		[Header("Components")]
		public List<Transform> doors;


		EnvironmentObject environmentObject;
		Level level;

		void Start() {
			environmentObject = GetComponent<EnvironmentObject>();
			level = GetComponentInParent<Level>();
		}

		void Update() {
			foreach (var door in doors) {
				if (environmentObject.isTriggered) {
					if (door.localPosition.y > -amplitude)
						door.localPosition -= Vector3.up * (speed * amplitude);
				} else {
					if (door.localPosition.y < 0)
						door.localPosition += Vector3.up * (speed * amplitude);
				}
				door.localPosition = new Vector3(0, Mathf.Max(-amplitude, Mathf.Min(0, door.localPosition.y)), 0);
			}
		}
	}
}