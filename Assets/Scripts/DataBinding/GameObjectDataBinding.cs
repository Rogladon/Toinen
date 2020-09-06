using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toinen {
	[AddComponentMenu("DataBinding/GameObject Data Binding")]
	public class GameObjectDataBinding : MonoBehaviour, IDataBinding {
		[SerializeField]
		public GameObject viewModelSource = null;
		public IViewModelProvider viewModelProvider { get; private set; }
		public ViewModel viewModel => viewModelProvider.viewModel;

		public GameObject targetGameObject;

		public string isActiveField;

		public bool refreshEveryFrame = true;

		void Start() {
			if (targetGameObject == null) {
				targetGameObject = gameObject;
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
			if (isActiveField != null && isActiveField.Length > 0) {
				targetGameObject.SetActive(viewModel.GetBoolExpression(isActiveField));
			}
		}
	}
}

