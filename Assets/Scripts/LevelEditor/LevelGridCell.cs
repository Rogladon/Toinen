using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Toinen.LevelEditor {
	[AddComponentMenu("Toinen/LevelEditor/Grid Cell")]
	[DisallowMultipleComponent]
	public sealed class LevelGridCell : MonoBehaviour {
		public int posX;
		public int posY;

		RectTransform rectTransform;
		LevelGrid grid;
		[HideInInspector]
		public EditorController editorController;

		Image image;
		float originalAlpha;

		void Start() {
			rectTransform = GetComponent<RectTransform>();
			grid = GetComponentInParent<LevelGrid>();
			image = GetComponent<Image>();
			originalAlpha = image.color.a;
			posX = (int)rectTransform.position.x;
			posY = (int)rectTransform.position.y;
	}

		public void ToggleVisibility() {
			if (image.color.a < 0.1f) {
				image.color = new Color(image.color.r, image.color.g, image.color.b, originalAlpha);
			} else {
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
			}
		}
	}
}

