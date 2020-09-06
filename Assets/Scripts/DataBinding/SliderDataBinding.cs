using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toinen {
	[AddComponentMenu("DataBinding/Slider Data Binding")]
	[DisallowMultipleComponent]
	public class SliderDataBinding : MonoBehaviour, IDataBinding {
		[SerializeField]
		private GameObject viewModelSource = null;
		public IViewModelProvider viewModelProvider { get; private set; }
		public ViewModel viewModel => viewModelProvider.viewModel;

		public Slider slider;

		public string valueField;
		public string minValueField;
		public string maxValueField;
		public string interactableField;

		public bool refreshEveryFrame = true;

		void Start() {
			if (slider == null) {
				slider = GetComponent<Slider>();
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
			if (interactableField != null && interactableField.Length > 0)
				slider.interactable = viewModel.GetBoolExpression(interactableField);
			if (minValueField != null && minValueField.Length > 0)
				slider.minValue = viewModel.GetField<float>(minValueField);
			if (maxValueField != null && maxValueField.Length > 0)
				slider.maxValue = viewModel.GetField<float>(maxValueField);
			if (valueField != null && valueField.Length > 0)
				slider.value = viewModel.GetField<float>(valueField);
		}
	}
}