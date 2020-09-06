using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Toinen {
	public abstract class BaseDataBindingDrawer<T> : Editor where T : MonoBehaviour {
		protected void CustomizableObjectField<T>(string title, ref bool trigger, ref T field, T stubValue = default(T)) where T : UnityEngine.Object {
			GUILayout.BeginHorizontal();
			GUILayout.Label(title, new[] { GUILayout.Width(EditorGUIUtility.labelWidth) });
			if (field != null || field != default(T) || trigger) {
				field = (T)EditorGUILayout.ObjectField(field, typeof(T), true);
				if (GUILayout.Button("Use default")) {
					trigger = false;
					field = default(T);
				}
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.ObjectField(stubValue, typeof(T), true);
				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button("Customize")) {
					trigger = true;
				}
			}
			GUILayout.EndHorizontal();
		}

		Dictionary<string, bool> fieldNameTriggers = new Dictionary<string, bool>();

		protected void FieldNameField(string title, ref bool trigger, ref string fieldName) {
			GUILayout.BeginHorizontal();
			bool isNullFieldName = fieldName == null || fieldName.Trim().Length < 1;
			GUILayout.Label($"Toggle {title.Replace("Field", "")}", new[] { GUILayout.Width(EditorGUIUtility.labelWidth) });
			if (!isNullFieldName || trigger) {
				fieldName = EditorGUILayout.TextField(fieldName);
				if (GUILayout.Button("Reset")) {
					trigger = false;
					fieldName = default(string);
				}
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.TextField("<i>[NOT SET]</i>", new GUIStyle(GUI.skin.textField) { richText = true });
				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button("Set", new[] { GUILayout.Width(EditorGUIUtility.labelWidth / 2) })) {
					trigger = true;
				}
			}
			GUILayout.EndHorizontal();
		}

		GameObject _defaultViewModelSource;
		bool _defaultViewModelSource_lookedUp = false;
		public GameObject defaultViewModelSource { get {
			if (!_defaultViewModelSource_lookedUp) {
				_defaultViewModelSource_lookedUp = true;
				var par = ((T)target).gameObject;
				while (par != null) {
					if (par.GetComponent<IViewModelProvider>() != null) {
						_defaultViewModelSource = par;
						break;
					}
					par = par.transform.parent.gameObject;
				}
			}
			return _defaultViewModelSource;
		} }
	}

	[CustomEditor(typeof(ButtonDataBinding))]
	public class ButtonDataBindingDrawer : BaseDataBindingDrawer<ButtonDataBinding> {
		bool useCustom_viewModelSource;
		bool useCustom_button;

		bool trigger_interactableField = false;
		bool trigger_disableField = false;

		public override void OnInspectorGUI() {
			ButtonDataBinding db = (ButtonDataBinding)target;

			FieldNameField(nameof(ButtonDataBinding.interactableField), ref trigger_interactableField, ref db.interactableField);
			FieldNameField(nameof(ButtonDataBinding.disableField), ref trigger_disableField, ref db.disableField);

			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ButtonDataBinding.refreshEveryFrame)), new GUIContent("Refresh every frame"));

			EditorGUILayout.Separator();
			CustomizableObjectField("Button", ref useCustom_button, ref db.button, db.GetComponent<Button>());
			CustomizableObjectField("ViewModel Source", ref useCustom_viewModelSource, ref db.viewModelSource, defaultViewModelSource);
		}
	}

	[CustomEditor(typeof(GameObjectDataBinding))]
	public class GameObjectDataBindingDrawer : BaseDataBindingDrawer<GameObjectDataBinding> {
		bool useCustom_viewModelSource;
		bool useCustom_gameObject;
		bool trigger_isActiveField;

		public override void OnInspectorGUI() {
			GameObjectDataBinding db = (GameObjectDataBinding)target;

			FieldNameField(nameof(GameObjectDataBinding.isActiveField), ref trigger_isActiveField, ref db.isActiveField);

			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ButtonDataBinding.refreshEveryFrame)), new GUIContent("Refresh every frame"));

			EditorGUILayout.Separator();
			CustomizableObjectField("Game Object", ref useCustom_gameObject, ref db.targetGameObject);
			CustomizableObjectField("ViewModel Source", ref useCustom_viewModelSource, ref db.viewModelSource, defaultViewModelSource);
		}
	}
}