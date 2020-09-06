using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toinen {
	public sealed class ApiStatusBar : MonoBehaviour {
		[Header("Components")]
		public GameObject loadingIndicator;
		public GameObject errorIndicator;
		public GameObject log;
		public Text errorsText;

		int unreadMessages = 0;

		void Start() {
			loadingIndicator.SetActive(false);
			errorIndicator.SetActive(false);
			log.SetActive(false);
			errorsText.text = "";

			Api.onError.AddListener((string msg) => {
				errorsText.text = $"{msg}\n{errorsText.text}";
				unreadMessages++;
			});
		}

		void Update() {
			loadingIndicator.SetActive(Api.activeRequests > 0);
			if (log.activeInHierarchy) {
				unreadMessages = 0;
			}
			errorIndicator.SetActive(unreadMessages > 0);
		}

		public void ToggleLog() {
			log.SetActive(!log.activeSelf);
		}
	}
}
