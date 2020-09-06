using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toinen {
	[AddComponentMenu("DataBinding/Image Data Binding")]
	[DisallowMultipleComponent]
	public class ImageDataBinding : MonoBehaviour, IDataBinding {
		[SerializeField]
		private GameObject viewModelSource = null;
		public IViewModelProvider viewModelProvider { get; private set; }
		public ViewModel viewModel => viewModelProvider.viewModel;

		public Image image;

		public string tintField;

		public bool refreshEveryFrame = true;

		void Start() {
			if (image == null) {
				image = GetComponent<Image>();
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
			if (tintField != null && tintField.Length > 0) {
				image.color = viewModel.GetField<Color>(tintField);
			}
		}
	}
}

