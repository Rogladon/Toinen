using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toinen {
	[DisallowMultipleComponent]
	public sealed class LevelChoosingMenu : MonoBehaviour {
		public TextAsset[] builtInLevels;

		[Header("Prefabs")]
		public Button buttonPrefab;

		[Header("Components")]
		public RectTransform levelButtonsDomain;

		void Start() {
			foreach (TextAsset ta in builtInLevels) {
				var btn = Instantiate(buttonPrefab, levelButtonsDomain.transform);
				btn.onClick.AddListener(() => {
					SceneLoader.LoadScene(SceneLoader.SCENE_PLAY, new Dictionary<string, object>() { { LevelLoader.PARAM_LEVEL_JSON, ta.text} });
				});
				btn.GetComponentInChildren<Text>().text = $"ta.name [BUILT-IN]";
			}
			StartCoroutine(nameof(UpdateLevels));
		}

		public void Close() {
			gameObject.SetActive(false);
		}

		private int targetLevelId;

		public IEnumerator UpdateLevels() {
			yield return Api.UpdateSummary();
			while (Api.summary == null)
				yield return new WaitForEndOfFrame();
			foreach (var info in Api.summary.levels) {
				var btn = Instantiate(buttonPrefab, levelButtonsDomain.transform);
				btn.onClick.AddListener(() => {
					targetLevelId = info.id;
					Debug.Log($"Target level set to {info.id}");
					StartCoroutine(Api.GetLevelJson(info.id, (string json) => {
						if (targetLevelId == info.id) {
							SceneLoader.LoadScene(SceneLoader.SCENE_PLAY, new Dictionary<string, object>() { { LevelLoader.PARAM_LEVEL_JSON, json } });
						}
					}));
				});
				btn.GetComponentInChildren<Text>().text = $"{info.name} [REMOTE]\nauthor:{info.author} id:{info.id}";
			}
			yield return null;
		}
	}
}
