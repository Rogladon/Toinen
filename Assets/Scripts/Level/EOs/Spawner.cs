using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Environment Objects/Spawner"),
	DisallowMultipleComponent,
	PlayModeOnly,
	RequireComponent(typeof(EnvironmentObject))]
	public class Spawner : MonoBehaviour {
		public Entity spawnablePrefab;

		public bool spawnOnStart = false;

		public enum OffsetModes {
			NONE,
			FROM_RECT_TRANSFORM_PIVOT,
			CUSTOM,
		}

		public OffsetModes offsetMode = OffsetModes.FROM_RECT_TRANSFORM_PIVOT;
		[Tooltip("Используется при " + nameof(offsetMode) + " == " + nameof(OffsetModes.CUSTOM))]
		public Vector3 customOffset = Vector3.zero;

		void Start() {
			if (spawnOnStart) {
				Spawn();
			}
		}

		public void Spawn() {
			Level level = GetComponentInParent<Level>();
			Entity s = Instantiate(spawnablePrefab, level.transform);
			s.transform.position = transform.position;
			switch (offsetMode) {
			case OffsetModes.FROM_RECT_TRANSFORM_PIVOT:
				RectTransform rt = s.GetComponent<RectTransform>();
				s.transform.position += new Vector3(rt.pivot.x, 0, 0);
				break;
			case OffsetModes.CUSTOM:
				s.transform.position += customOffset;
				break;
			case OffsetModes.NONE:
			default:
				break;
			}
			level.onEntitySpawn.Invoke(s);
			Destroy(gameObject);
		}
	}
}
