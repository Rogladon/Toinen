using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Toinen {
	public static class Net {
		public delegate void HandleTextData(string text);
		public delegate void HandleError(long responseCode);

		public static IEnumerator Get(string url, HandleTextData callback, HandleError error) {
			using (UnityWebRequest request = UnityWebRequest.Get(url)) {
				yield return request.SendWebRequest();
				if (request.isNetworkError) {
					Debug.Log($"Network error {request.responseCode}, while getting summary");
					if (error != null) error(request.responseCode);
				} else if (request.isHttpError) {
					Debug.Log($"HTTP error {request.responseCode}, while getting summary");
					if (error != null) error(request.responseCode);
				} else {
					if (callback == null) {
						Debug.LogWarning($"GET request with {nameof(callback)} == null");
					} else {
						callback(request.downloadHandler.text);
					}
				}
			}
		}

		public static IEnumerator Post(string url, string postData, HandleTextData callback, HandleError error) {
			using (UnityWebRequest request = UnityWebRequest.Post(url, postData)) {
				yield return request.SendWebRequest();
				if (request.isNetworkError) {
					Debug.Log($"Network error {request.responseCode}, while getting summary");
					if (error != null) error(request.responseCode);
				} else if (request.isHttpError) {
					Debug.Log($"HTTP error {request.responseCode}, while getting summary");
					if (error != null) error(request.responseCode);
				} else {
					if (callback == null) {
						Debug.LogWarning($"GET request with {nameof(callback)} == null");
					} else {
						callback(request.downloadHandler.text);
					}
				}
			}
		}
	}
}
