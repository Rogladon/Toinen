using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toinen {
	[DisallowMultipleComponent]
	public sealed class BoilingPot : MonoBehaviour {
		public GameObject waterTop;
		public GameObject waterFull;
		public GameObject wallLeft;
		public GameObject wallRight;
		public GameObject wallBottom;
		public GameObject cornerBottomLeft;
		public GameObject cornerBottomRight;
		public GameObject bubbles;
		public GameObject waterTopEditing;

		int waterHeight;
		int waterTopWidth = 0;

		ParticleSystem bubblesSystem;

		Water _water;
		Water water {
			get {
				if(_water == null) {
					_water = GetComponentInChildren<Water>();
				}
				return _water;
			}
		}

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
			bubblesSystem = bubbles.GetComponent<ParticleSystem>();
			UpdateConnection();
			WaterSize();
			WaterBubblesHeight();
			if (environmentObject.level.isEditing) {
				if (water)
					water.RemoveWater();
				bubblesSystem.Stop();
				waterTop.SetActive(false);
				waterTop = waterTopEditing;
			}
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
				WaterSize();
				WaterBubblesHeight();
			}
		}

		/// <summary>
		/// Обновить соединение с другими объектами
		/// </summary>
		/// <param name="recursive">Происходить ли сейчас рекурсивное обновление (например, после добавления нового блока). Если true, то UpdateConnection не будет вызван у соседей.</param>
		public void UpdateConnection(bool recursive = false) {
			bool hasTop = CheckAt(0, 1, recursive);
			bool hasBottom = CheckAt(0, -1, recursive);
			bool hasLeft = CheckAt(-1, 0, recursive);
			bool hasRight = CheckAt(1, 0, recursive);
			bool hasBottomLeft = CheckAt(-1, -1, recursive);
			bool hasBottomRight = CheckAt(1, -1, recursive);
			// appearance.Choose(hasTop, hasBottom, hasLeft, hasRight);

			bubbles.SetActive(!hasBottom);
			waterTop.SetActive(!hasTop);
			waterFull.SetActive(hasTop);
			wallBottom.SetActive(!hasBottom);
			wallLeft.SetActive(!hasLeft);
			wallRight.SetActive(!hasRight);
			cornerBottomLeft.SetActive(!hasBottomLeft || !hasLeft || !hasBottom);
			cornerBottomRight.SetActive(!hasBottomRight || !hasRight || !hasBottom);
		}

		void WaterSize() {
			if (CheckAt(-1, 0, false,true) || !waterTop.activeSelf) {
				if(water)
					water.RemoveWater();
				waterTopWidth = 0;
				return;
			}
			int width = 0;
			int dx = 1;
			bool hasRight = true;
			while (hasRight) {
				hasRight = CheckAt(dx, 0, false, true);
				dx++;
				width++;
			}
			if (width != waterTopWidth) {
				waterTopWidth = width;
				water.widhtWater = width;
				water.ReSpawn();
			}
		}

		void WaterBubblesHeight() {
			if (CheckAt(0, -1, false)) {
				waterHeight = 0;
				return;
			}
			int height = 0;
			int dy = 1;
			bool hasBotoom = true;
			while (hasBotoom) {
				hasBotoom = CheckAt(0, dy, false);
				dy++;
				height++;
			}
			if (height != waterHeight) {
				waterHeight = height;
				bubblesSystem.startSpeed = 5 + (height * 0.3f);
				bubblesSystem.startLifetime = 1 / bubblesSystem.startSpeed * height-0.05f;
			}
		}

		public bool CheckAt(int dx, int dy, bool recursive) {
			return CheckAt(dx, dy, recursive, false);
		}

		public bool CheckAt(int dx, int dy, bool recursive, bool topWater) {
			if (environmentObject.level == null)
				return false;
			var eo = environmentObject.level.GetAt((int)environmentObject.position.x + dx, (int)environmentObject.position.y + dy);
			if (eo != null && eo.paletteCode == environmentObject.paletteCode) {
				if (!recursive)
					eo.GetComponent<BoilingPot>().UpdateConnection(true);
				if (topWater) {
					if (eo.GetComponent<BoilingPot>().waterTop.activeSelf)
						return true;
				} else {
					return true;
				}
			}
			return false;
		}
	}
}
