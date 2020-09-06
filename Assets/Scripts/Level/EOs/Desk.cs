using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toinen {
	[DisallowMultipleComponent]
	public sealed class Desk : MonoBehaviour {
		public GameObject wallLeft;
		public GameObject wallRight;
		public GameObject wallTop;

		EnvironmentObject _environmentObject;
		EnvironmentObject environmentObject {
			get {
				if (_environmentObject == null) {
					_environmentObject = GetComponent<EnvironmentObject>();
				}
				return _environmentObject;
			}
		}


		void Start() {
			UpdateConnection();
		}

		float sinceLastUpdate = 0;

		void Update() {
			if (environmentObject.level.isEditing) {
				sinceLastUpdate += Time.deltaTime;
				if (environmentObject.level.isEditing && sinceLastUpdate > 0.25f || sinceLastUpdate > 0.75f) {
					sinceLastUpdate = 0;
					UpdateConnection();
				}
			} else if (sinceLastUpdate < 5) {
				sinceLastUpdate++;
				UpdateConnection();
			}
		}

		public void UpdateConnection(bool recursive = false) {
			// bool hasTop = CheckAt(0, 1, recursive);
			bool hasLeft = CheckAt(-1, 0, recursive);
			bool hasRight = CheckAt(1, 0, recursive);

			wallTop.SetActive(true);
			wallLeft.SetActive(!hasLeft);
			wallRight.SetActive(!hasRight);
		}

		public bool CheckAt(int dx, int dy, bool recursive) {
			if (environmentObject.level == null)
				return false;
			var eo = environmentObject.level.GetAt((int)environmentObject.position.x + dx, (int)environmentObject.position.y + dy);
			if (eo != null && eo.paletteCode == environmentObject.paletteCode) {
				if (!recursive)
					eo.GetComponent<Desk>().UpdateConnection(true);
				return true;
			}
			return false;
		}
	}
}

