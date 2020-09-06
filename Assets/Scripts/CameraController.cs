using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Camera Controller")]
	[DisallowMultipleComponent]
	public sealed class CameraController : MonoBehaviour {
		float zOffset;

		[SerializeField]
		Level level;
		new Camera camera;

		public float zoom = 1f;

		public Entity entity;
		public bool entityPriority = true;

		public bool centrizeOnStart = true;

		void Start() {
			zOffset = transform.position.z;

			camera = GetComponent<Camera>();
			if (level == null) {
				level = GameObject.FindGameObjectWithTag(Level.TAG).GetComponent<Level>();
				if (level == null) {
					Debug.Log($"Failed to find {Level.TAG}");
				}
			}
			if (centrizeOnStart) {
				Centrize();
			}
		}

		void FixedUpdate() {
			if (entity != null) {
				float dx = (entity.eyesPoint.position.x - transform.position.x) * 0.5f;
				float dy = (entity.eyesPoint.position.y - transform.position.y);
				if (!entity.isNowOnGround) {
					dy *= 0.01f;
				} else {
					dy *= 0.25f;
				}
				transform.position += new Vector3(dx, dy);
			}
			camera.orthographicSize = (level.rect.width) / camera.aspect / 2 * zoom;
		}

		public void Centrize(float widthOffset = 0) {
			camera.orthographicSize = (level.rect.width + widthOffset) / camera.aspect / 2 * zoom;
			transform.localScale = new Vector3(zoom, zoom, 1);
			transform.position = new Vector3(0, 0, transform.position.z) + (Vector3)level.rect.center;
		}
	}
}

