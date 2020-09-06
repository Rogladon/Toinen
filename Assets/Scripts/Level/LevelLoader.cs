using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Level Loader")]
	[DisallowMultipleComponent]
	public sealed class LevelLoader : MonoBehaviour {
		public readonly static string PARAM_LEVEL_JSON = "level-json";

		public RoomElementsPalette palette;

		[Tooltip("If null, searching for source in SceneLoader.loadParameters")]
		public TextAsset sourceAsset;

		public string sourceJson { get; private set; } = null;

		bool isLoaded = false;

		Level level;

		void Start() {
			level = GetComponent<Level>();
			if (SceneLoader.TryGetParameter(PARAM_LEVEL_JSON, out string levelJson)) {
				sourceJson = levelJson;
				LoadFromText(levelJson);
			} else if (PlayerPrefs.HasKey("LevelCache") && SceneLoader.targetScene == SceneLoader.SCENE_EDITOR) {
				sourceJson = PlayerPrefs.GetString("LevelCache");
				LoadFromText(sourceJson);
			} 
			else if (sourceAsset != null) {
				sourceJson = levelJson;
				Debug.Log($@"Loading from TextAsset ""{sourceAsset.name}""");
				LoadFromText(sourceAsset.text);
			} else {
				Debug.LogError($@"Failed to load level: {nameof(sourceAsset)} is null, while ""{PARAM_LEVEL_JSON}"" format string in {nameof(SceneLoader)}.{nameof(SceneLoader.loadParameters)} is also NOT found.");
			}
		}

		public void LoadFromText(string src) {
			if (isLoaded) {
				Debug.LogError("Already loaded");
				return;
			}
			Serialized.Level sLevel = (Serialized.Level)JsonUtility.FromJson(src, typeof(Serialized.Level));
			UnpackLevel(ref sLevel);
			isLoaded = true;
		}

		public void UnpackLevel(ref Serialized.Level sLevel) {
			// TODO: handle SerializedLevel.name
			level.rect = sLevel.rect;

			foreach (Serialized.EnvironmentObject seo in sLevel.objects) {
				EnvironmentObject eo;
				UnpackEnvironmentObject(out eo, seo);
				eo.transform.SetParent(level.objectsDomain, false);
			}
		}

		public void UnpackEnvironmentObject(out EnvironmentObject eo, Serialized.EnvironmentObject seo) {
			eo = null;
			if (palette.environmentObjects.ContainsKey(seo.code)) {
				eo = Instantiate(palette.environmentObjects[seo.code]);
				eo.paletteCode = seo.code;
				eo.transform.position = new Vector3(seo.x, seo.y, 0);
				if (seo.color != null && seo.color.Length > 0) {
					if (!ColorUtility.TryParseHtmlString(seo.color, out eo.color)) {
						Debug.LogError($@"Failed parsing color ""{seo.color}""");
					}
				}
			} else {
				Debug.LogError($@"Failed to unpack environment object: No ""{seo.code}"" in palette.");
			}
		}
	}
}
