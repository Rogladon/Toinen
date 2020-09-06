using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Toinen.LevelEditor {
	[AddComponentMenu("Toinen/LevelEditor/Navigation")]
	[DisallowMultipleComponent]
	public class EditorNavigation : MonoBehaviour {
		public float speedScale = 2f;
		public float speedMove = 1f;
		public CameraController cameraController;
		private Transform cam { get { return cameraController.transform; } }
		public Level level;

		public bool isLocked = false;

		void Start() {
			cameraController = GetComponent<CameraController>();
			if (level == null) {
				level = GameObject.FindGameObjectWithTag(Level.TAG).GetComponent<Level>();
				if (level == null) {
					Debug.Log($"Failed to find {Level.TAG}");
				}
			}
		}

		void Update() {
			if (EventSystem.current.currentSelectedGameObject != null/* || palette.selected != null*/) {
				return;
			}
			if (isLocked) return;
			if (InputHelper.isDoubleMoved) {
				Debug.Log(InputHelper.doubleDeltaPos);
				cam.position -= InputHelper.doubleDeltaPos* speedMove * Time.deltaTime;
			}
			if (InputHelper.isScaler) {
				cameraController.zoom += InputHelper.deltaScaler * speedScale * Time.deltaTime;
			}
		}
	}
}