using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace Toinen {
	[CreateAssetMenu(fileName="New REPalette", menuName = "Room Elements Palette", order=51)]
	[DisallowMultipleComponent]
	public sealed class RoomElementsPalette : ScriptableObject {
#pragma warning disable 0649
		[SerializeField]
		private EnvironmentObject[] _environment;
#pragma warning restore 0649

		private Dictionary<string, EnvironmentObject> _environmentObjects;
		public Dictionary<string, EnvironmentObject> environmentObjects {
			get {
				if (_environmentObjects == null) {
					RebuildCache();
				}
				return _environmentObjects;
			}
		}

		public void RebuildCache() {
			Debug.Log($"Rebuilding cache for palette {name}");
			_environmentObjects = new Dictionary<string, EnvironmentObject>();
			foreach (EnvironmentObject eo in _environment) {
				if (_environmentObjects.ContainsKey(eo.paletteCode)) {
					Debug.LogError($@"Duplicate palette code ""{eo.paletteCode}""");
				}
				_environmentObjects.Add(eo.paletteCode, eo);
			}
		}
	}
}

