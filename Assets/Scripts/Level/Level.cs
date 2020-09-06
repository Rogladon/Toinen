using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toinen {
	public class LevelEvent : UnityEvent<Level> {}

	[AddComponentMenu("Toinen/Level/Level")]
	[DisallowMultipleComponent]
	public sealed class Level : MonoBehaviour {
		public static readonly string TAG = "Level";

		public class EntityEvent : UnityEvent<Entity> {}

		public LevelEvent onLevelComplete = new LevelEvent();
		public EntityEvent onEntitySpawn = new EntityEvent();

		public ColorTriggers colorTriggers = new ColorTriggers();

		public Rect rect = new Rect(0, 0, 24, 13);

		public bool isCompleted { get; private set; } = false;
		public bool isEditing = false;

		[Header("Components")]
		public Transform objectsDomain;

		/// <summary>
		/// Перебор объектов окружения
		/// </summary>
		public IEnumerable<EnvironmentObject> objects { get {
			foreach (Transform t in objectsDomain) {
				EnvironmentObject eo;
				if (t.TryGetComponent(out eo)) {
					yield return eo;
				}
			}
		} }

		/// <summary>
		/// Проверить во всех ли выходах есть персонажи. Если да, вызывает <see cref="onLevelComplete.Invoke">
		/// </summary>
		public void CheckExits() {
			if (isCompleted) {
				return;
			}
			foreach (var e in transform.GetComponentsInChildren<Exit>()) {
				if (!e.isOk) {
					return;
				}
			}
			isCompleted = true;
			Debug.Log("Level completed");
			onLevelComplete.Invoke(this);
		}

		public EnvironmentObject GetAt(int gx, int gy) {
			int x = gx;
			int y = gy;
			foreach (var eo in objects) {
				for (int dy = 0; dy < eo.height; dy++) {
					for (int dx = 0; dx < eo.width; dx++) {
						if (Mathf.Abs((eo.transform.localPosition.x + dx) - x) < 0.01f && Mathf.Abs((eo.transform.localPosition.y + dy) - y) < 0.01f) {
							return eo;
						}
					}
				}
			}
			return null;
		}

		public bool IsValidCoord(int gx, int gy) => rect.Contains(new Vector2(gx, gy));

		public static void FindInstanceIfNull(ref Level level) {
			if (level == null) {
				level = GameObject.FindGameObjectWithTag(Level.TAG).GetComponent<Level>();
				if (level == null) {
					Debug.Log($"Failed to find {Level.TAG}");
				}
			}
		}
	}
}
