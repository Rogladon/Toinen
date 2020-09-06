using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Toinen {
	public class ApiEvent : UnityEvent<string> {
	}

	public static class Api {
		[Serializable]
		/// <summary>
		/// Сводка по уровням на сервере
		/// </summary>
		public class Summary {
			public LevelInfo[] levels;
		}

		[Serializable]
		public class LevelInfo {
			public int id;
			public string name;
			public string author;
			public string creationDate;
		}

		[Serializable]
		public class UploadInfo {
			public bool success;
			public int levelId;
		}

		[Serializable]
		public class Error {
			public bool success;
			public string error;
		}

		public static string host = "http://localhost:5000";
		public static string FormUrl(string uri) => host + (uri.StartsWith("/") ? uri : "/" + uri);

		public static Summary summary { get; private set; } = null;
		public static Dictionary<int, string> levelsJsons { get; private set; } = new Dictionary<int, string>();

		public static readonly ApiEvent onError = new ApiEvent();
		/// <summary>
		/// Количество выполняемых в данный момент запросов API
		/// </summary>
		public static int activeRequests { get; private set; } = 0;

		public static IEnumerator UpdateSummary() {
			Debug.Log("Updating summary");
			activeRequests++;
			yield return Net.Get(FormUrl("/summary"),
				(string text) => {
					activeRequests--;
					summary = JsonUtility.FromJson<Summary>(text);
				},
				(long code) => {
					activeRequests--;
					var errmsg = $"Failed to update summary: error {code}";
					Debug.LogError(errmsg);
					onError.Invoke(errmsg);
				}
			);
		}

		public delegate void HandleLevelJson(string msg);

		/// <summary>
		/// Загрузка уровня с сервера или из кэша, если уже был скачан.
		/// </summary>
		/// <param name="id">Индекс уровня из <see cref="Summary"></param>
		public static IEnumerator GetLevelJson(int id, HandleLevelJson callback, HandleApiError errorHandler = null) {
			if (levelsJsons.ContainsKey(id)) {
				callback(levelsJsons[id]);
			} else {
				Debug.Log($"Loading level {id}");
				activeRequests++;
				yield return Net.Get(FormUrl($"/l/{id}/json"),
					(string text) => {
						activeRequests--;
						levelsJsons[id] = text;
						callback(levelsJsons[id]);
						// TODO: Error handling
					},
					(long code) => {
						activeRequests--;
						var errmsg = $"Failed to load level {id}: error {code}";
						Debug.LogError(errmsg);
						onError.Invoke(errmsg);
						if (errorHandler != null) errorHandler($"Network error {code}");
					}
				);
			}
		}

		public delegate void HandleLevelId(int id);
		public delegate void HandleApiError(string msg);

		public static IEnumerator UploadLevel(string author, string json, HandleLevelId callback, HandleApiError errorHandler) {
			activeRequests++;
			yield return Net.Post(FormUrl($"/u/{author}/upload"), json, (string text) => {
				activeRequests--;
				var uploadInfo = JsonUtility.FromJson<UploadInfo>(text);
				if (uploadInfo.success) {
					callback(uploadInfo.levelId);
				} else {
					var err = JsonUtility.FromJson<Error>(text);
					var errmsg = $"api error: {err.error}";
					Debug.LogError(errmsg);
					onError.Invoke(errmsg);
					if (errorHandler != null) errorHandler(err.error);
				}
			}, (long code ) => {
				activeRequests--;
				var errmsg = $"Failed to upload level: error {code}";
				Debug.LogError(errmsg);
				onError.Invoke(errmsg);
				if (errorHandler != null) errorHandler($"Network error {code}");
			});
		}

		public static readonly Dictionary<string, string> ERROR_CODES = new Dictionary<string, string>() {
			{ "ERR_INVALID_JSON", "invalid JSON" },
			{ "ERR_INVALID_JSON_STRUCTURE", "invalid structure of JSON" },
		};

		/// <summary>
		/// Расшифровать значение ошибки API
		/// </summary>
		public static string ExpandError(string code) {
			if (ERROR_CODES.TryGetValue(code, out string s)) {
				return s;
			} else {
				return code;
			}
		}
	}
}
