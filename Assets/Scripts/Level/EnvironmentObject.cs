using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/EnvironmentObject")]
	[DisallowMultipleComponent]
	public sealed class EnvironmentObject : MonoBehaviour {
		public enum Categories {
			PLATFORM = 0,
			ACTING,
			SPAWNER,
			TRIGGER,
			INTERACTABLE,
		}

		public string paletteCode;
		public Sprite paletteIcon = null;
		public Color paletteIconColor = Color.white;

		public Categories category = Categories.PLATFORM;

		[Tooltip("Используется для связи между SPAWNER- и ACTING-объектами")]
		public Color color = Color.white;
		public bool usesColors = true;
		public bool isTriggered => level.colorTriggers[color];

		[SerializeField]
		private SpriteRenderer[] tintedComponents;

		public Vector2 position {
			get => new Vector2(transform.localPosition.x, transform.localPosition.y);
			set {
				transform.localPosition = new Vector3((int)value.x, (int)value.y, 0);
				if (_rect != null) {
					_rect.x = (int)value.x;
					_rect.y = (int)value.y;
				}
			}
		}

		[SerializeField]
		private RectTransform _shape;

		public int width => _shape == null ? 1 : (int)_shape.rect.width;
		public int height => _shape == null ? 1 : (int)_shape.rect.height;

		private Rect _rect;
		public Rect rect { get {
			if (_rect == null) {
				var pos = position;
				_rect = new Rect(pos.x, pos.y, width, height);
			}
			return _rect;
		} }

		[Tooltip("Объекты, которые будут деактивированы в режиме редактора")]
		public GameObject[] playModeOnlyObjects;
		[Tooltip("Объекты, в которых в режиме редактора будет производиться поиск компонентов с аттрибутом PlayModeOnly")]
		public GameObject[] objectsWithPlayModeOnlyAttribute;

		private Level _level;
		public Level level { get {
			if (_level == null) {
				_level = GetComponentInParent<Level>();
			}
			return _level;
		} }

		void Start() {
			if (level == null || level.isEditing) {
				DisablePlayModeOnlies();
			}
		}

		void FixedUpdate() {
			if (tintedComponents != null) {
				foreach (var t in tintedComponents) {
					t.color = color;
				}
			}
		}

		public void DisablePlayModeOnlies() {
			Rigidbody2D[] rigids = GetComponentsInChildren<Rigidbody2D>();
			foreach(var i in rigids) {
				i.simulated = false;
			}
			Rigidbody2D selfRigid = GetComponent<Rigidbody2D>();
			if (selfRigid) selfRigid.simulated = false;
			if (playModeOnlyObjects != null) {
				foreach (var o in playModeOnlyObjects) {
					o.SetActive(false);
				}
			}
			foreach (var mb in GetPotentiallyDisablableMonoBehaviours()) {
				if (Attribute.GetCustomAttribute(mb.GetType(), typeof(PlayModeOnlyAttribute)) != null) {
					mb.enabled = false;
				}
			}
		}

		public void EnablePlayModeOnlies() {
			foreach (var mb in GetPotentiallyDisablableMonoBehaviours()) {
				if (Attribute.GetCustomAttribute(mb.GetType(), typeof(PlayModeOnlyAttribute)) != null) {
					mb.enabled = true;
				}
			}
			if (playModeOnlyObjects != null) {
				foreach (var o in playModeOnlyObjects) {
					o.SetActive(true);
				}
			}
		}

		/// <returns>Список компонентов на самом объекте и в потомаках, которые могут быть отключены в режиме редактора</returns>
		public IEnumerable<MonoBehaviour> GetPotentiallyDisablableMonoBehaviours() {
			foreach (var mb in gameObject.GetComponents<MonoBehaviour>()) {
				yield return mb;
			}
			if (objectsWithPlayModeOnlyAttribute != null) {
				foreach (var o in objectsWithPlayModeOnlyAttribute) {
					foreach (var mb in o.GetComponents<MonoBehaviour>()) {
						yield return mb;
					}
				}
			}
		}
	}
}
