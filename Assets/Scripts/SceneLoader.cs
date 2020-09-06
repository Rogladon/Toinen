using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	using Parameters = Dictionary<string, object>;
	[DisallowMultipleComponent]
	public class SceneLoader : MonoBehaviour {
		public static readonly int SCENE_LOADER_SCENE_ID = 0;

		public static string targetScene { get; private set; } = null;
		public static Parameters loadParameters { get; private set; } = null;

		public static readonly string SCENE_MAIN_MENU = "main-menu";
		public static readonly string SCENE_PLAY = "play";
		public static readonly string SCENE_EDITOR = "editor";

		[Tooltip("Сцена загружаемая по умолчанию")]
		public string targetSceneNullFallback = SCENE_MAIN_MENU;
		[Tooltip("Сцена загружаемая по умолчанию в STANDALONE-версии")]
		public string targetSceneStandaloneNullFallback = SCENE_EDITOR;

		public static bool editing = false;

		[SerializeField]
		private StringSceneReferenceDictionary scenesStore = StringSceneReferenceDictionary.New<StringSceneReferenceDictionary>();
		public Dictionary<string, SceneReference> scenes { get { return scenesStore.dictionary; } }

		void Start() {
			CheckSceneReferences();
		}

		Coroutine loadingCoroutine;
		void Update() {
			if (loadingCoroutine == null) {
				string scene = targetScene;
				if (targetScene == null) {
#if UNITY_STANDALONE
					scene = targetSceneStandaloneNullFallback;
#else
					scene = targetSceneNullFallback;
#endif
				}
				loadingCoroutine = StartCoroutine(LoadAsync(scene));
			}
			if(targetScene == SCENE_MAIN_MENU) {
				editing = false;
			}
		}

		void Load(string sceneName) {
			if (scenes.TryGetValue(sceneName, out SceneReference scene)) {
				SceneManager.LoadScene(scene);
			} else {
				Debug.LogError($"NO SCENE \"{sceneName}\")!!!!!");
			}
		}

		IEnumerator LoadAsync(string sceneName) {
			if (scenes.TryGetValue(sceneName, out SceneReference scene)) {
				AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
				while (!ao.isDone) {
					yield return null;
				}
			} else {
				Debug.LogError($"NO SCENE \"{sceneName}\")!!!!!");
			}
		}

		public void CheckSceneReferences() {
			int cnt = 0;
			if (!CheckDefaultSceneReference(SCENE_MAIN_MENU)) cnt++;
			if (!CheckDefaultSceneReference(SCENE_PLAY)) cnt++;
			if (!CheckDefaultSceneReference(SCENE_EDITOR)) cnt++;
			if (cnt > 0)
				Debug.LogWarning($"Scenes check failed with {cnt} errors");
		}

		bool CheckDefaultSceneReference(string sceneName) {
			if (!scenes.ContainsKey(sceneName)) {
				Debug.LogError($"No scene reference for default scene {sceneName}");
				return false;
			}
			return true;
		}

		public static void LoadScene(string name, Parameters parameters = null) {
			targetScene = name;
			loadParameters = parameters;
			SceneManager.LoadScene(SCENE_LOADER_SCENE_ID);
		}

		public static IEnumerator LoadSceneAsync(string name, Parameters parameters = null) {
			targetScene = name;
			loadParameters = parameters;
			var ao = SceneManager.LoadSceneAsync(SCENE_LOADER_SCENE_ID);
			while (!ao.isDone) {
				yield return ao;
			}
		}

		public static T GetParameter<T>(string paramName) {
			if (loadParameters != null && loadParameters.TryGetValue(paramName, out object o)) {
				if (o is T) {
					return (T)o;
				}
			}
			return default(T);
		}

		public static bool TryGetParameter<T>(string paramName, out T obj) {
			if (loadParameters != null && loadParameters.TryGetValue(paramName, out object o)) {
				if (o is T) {
					obj = (T)o;
					return true;
				}
			}
			obj = default(T);
			return false;
		}
	}
}
