using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Toinen.LevelEditor {
	[AddComponentMenu("Toinen/LevelEditor/Grid")]
	[DisallowMultipleComponent]
	public sealed class LevelGrid : MonoBehaviour {
		public class CellClickedEvent : UnityEvent<int, int> {}

		public class Events {
			public CellClickedEvent cellClicked = new CellClickedEvent();
		}
		public Events events = new Events();

		[Header("Prefabs")]
		public GameObject linePrefab;

		Level level;

		List<GameObject> lines = null;

		bool isTapped;

		public EditorController editorController;

		float levelHeight;
		float levelWidth;

		void Start() {
			Level.FindInstanceIfNull(ref level);
		}

		float delayTapped = 0.1f;
		bool isEditing = false;

		void Update() {
			transform.position = level.rect.position;
			if (lines == null || (levelHeight != level.rect.size.y || levelWidth != level.rect.size.x)) {
				InstantiateCells();
			}

			if (InputHelper.isTapped) {
				Invoke("StartEdit", delayTapped);
				isTapped = true;
			}

			if (InputHelper.isTapEnded || InputHelper.isDoubleMoved) {
				isEditing = false;
				isTapped = false;
				CancelInvoke("StartEdit");
			}

			if (!isEditing) return;

			if (EventSystem.current.currentSelectedGameObject == null) {
				if (editorController.paletteMenu.selectedCategory == EnvironmentObject.Categories.PLATFORM || editorController.isDeletingMode) {
					if ((InputHelper.isMoved || InputHelper.isStationary || isTapped) && !InputHelper.isDoubleMoved) {
						
						Clicked();
					}
				} else {
					Debug.Log("1");
					if(isTapped) {
						Debug.Log("Clicked");
						Clicked();
					}
				}
			}
		}

		void Clicked() {
			isTapped = false;
			RaycastHit2D hit;
			Vector2 curMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (hit = Physics2D.Raycast(curMousePos, Vector2.zero)) {
				Vector2 point = hit.point;
				point = GetCell(point);
				Debug.Log($"Cell {(int)point.x}:{(int)point.y} clicked");
				events.cellClicked.Invoke((int)point.x, (int)point.y);

			}
		}

		void StartEdit() {
			isEditing = true;
		}

		Vector2 GetCell(Vector2 point) {
			Vector2 newPoint;
			newPoint.x = Mathf.Round(point.x-0.5f);
			newPoint.y = Mathf.Round(point.y-0.5f);
			return newPoint;
		}

		void InstantiateCells() {
			levelHeight = level.rect.size.y;
			levelWidth = level.rect.size.x;
			if(lines != null) {
				for(int i = 0; i < lines.Count; i++) {
					Destroy(lines[i]);
				}
			}
			lines = new List<GameObject>();
			for (int y = 0; y < levelHeight+1; y++) {
				Transform t = Instantiate(linePrefab, transform).transform;
				t.SetParent(transform);
				t.localPosition = new Vector3(levelWidth/2, y, 0);
				t.localScale = new Vector3(levelWidth, 0.1f, 1);
				lines.Add(t.gameObject);
			}
			for (int x = 0; x < levelWidth+1; x++) {
				Transform t = Instantiate(linePrefab, transform).transform;
				t.SetParent(transform);
				t.localPosition = new Vector3(x,levelHeight/2, 0);
				t.localScale = new Vector3(0.1f, levelHeight, 1);
				lines.Add(t.gameObject);
			}
		}

			public void ToggleVisibility() {
			foreach (GameObject cell in lines) {
				cell.SetActive(!cell.activeSelf);
			}
		}
	}
}
