using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toinen {
	[AddComponentMenu("DataBinding/Text Data Binding")]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	public class TextDataBinding : MonoBehaviour, IDataBinding {
		public static readonly string[] SEPARATORS = new string[] { "@!" };

		[SerializeField]
		private GameObject viewModelSource = null;
		public IViewModelProvider viewModelProvider { get; private set; }
		public ViewModel viewModel => viewModelProvider.viewModel;

		public Text textField;


		public bool refreshEveryFrame = true;
		public bool singleFieldMode = false;

		public string textTemplate;
		public string[] fields;

		public string fadeToAlphaField = null;
		public float fadeToAlphaFactor = 0.8f;

		void Start() {
			if (textField == null) {
				textField = GetComponent<Text>();
			}

			if (textTemplate == null || textTemplate.Length == 0) {
				textTemplate = textField.text;
			}

			if (!Application.isPlaying) {
				return;
			}

			if (viewModelSource == null) {
				viewModelProvider = GetComponentInParent<IViewModelProvider>();
			}
			if (viewModelProvider == null && (viewModelProvider = viewModelSource.GetComponent<IViewModelProvider>()) == null) {
				Debug.LogError($"IViewModelProvider has not been found in {viewModelSource}");
			}
		}

		void Update() {
			if (Application.isPlaying) {
				if (refreshEveryFrame) {
					Refresh();
				}
			} else {
				textField.text = textTemplate;
			}
		}

		public void Refresh() {
			if (singleFieldMode) {
				textField.text = viewModel.GetStringField(textTemplate);
			} else {
				object[] fieldsValues = new object[fields.Length];
				for (int i = 0; i < fields.Length; i++) {
					fieldsValues[i] = viewModel.GetStringField(fields[i]);
				}
				textField.text = String.Format(textTemplate, fieldsValues);
			}

			if (fadeToAlphaField != null && fadeToAlphaField.Length > 0) {
				if (viewModel.GetBoolExpression(fadeToAlphaField)) {
					textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, textField.color.a * fadeToAlphaFactor);
				} else {
					textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, 1);
				}
			}
		}
	}
}
