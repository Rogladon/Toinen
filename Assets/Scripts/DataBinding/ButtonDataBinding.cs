using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toinen {
	[AddComponentMenu("DataBinding/Button Data Binding")]
	[DisallowMultipleComponent]
	public class ButtonDataBinding : MonoBehaviour, IDataBinding {
		[SerializeField]
		public GameObject viewModelSource = null;
		public IViewModelProvider viewModelProvider { get; private set; }
		public ViewModel viewModel => viewModelProvider.viewModel;

		public Button button;

		public string disableField;
		public string interactableField;

		public bool refreshEveryFrame = true;

		void Start() {
			if (button == null) {
				button = GetComponent<Button>();
			}

			if (viewModelSource == null) {
				viewModelProvider = GetComponentInParent<IViewModelProvider>();
			}

			if (viewModelProvider == null && (viewModelProvider = viewModelSource.GetComponent<IViewModelProvider>()) == null) {
				Debug.LogError($"IViewModelProvider has not been found in {viewModelSource}");
			}
		}

		void FixedUpdate() {
			if (refreshEveryFrame) {
				Refresh();
			}
		}

		public void Refresh() {
			button.interactable = viewModel.GetBoolExpression(interactableField);
			if (disableField != null && disableField.Length > 0) {
				if (viewModel.GetBoolExpression(disableField)) {
					gameObject.SetActive(false);
				}
			}
		}
	}
}
