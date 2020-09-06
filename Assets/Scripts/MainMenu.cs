using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toinen {
	[DisallowMultipleComponent]
	public sealed class MainMenu : MonoBehaviour {
		public RectTransform mainMenu;
		public LevelChoosingMenu levelChoosingMenu;

		void Start() {
			levelChoosingMenu.gameObject.SetActive(false);
		}

		void Update() {
			if (!levelChoosingMenu.gameObject.activeSelf)
				mainMenu.gameObject.SetActive(true);
		}

		public void GoPlay() {
			mainMenu.gameObject.SetActive(false);
			levelChoosingMenu.gameObject.SetActive(true);
		}

		public void GoEditor() {
			SceneLoader.LoadScene(SceneLoader.SCENE_EDITOR);
		}
	}
}