using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toinen {
	[CustomEditor(typeof(RoomElementsPalette))]
	public class RoomElementsPaletteDrawer : Editor {
		public override void OnInspectorGUI() {
			GUILayout.Label("");
			if (GUILayout.Button("Validate")) {
				RoomElementsPalette palette = (RoomElementsPalette)serializedObject.targetObject;
				palette.RebuildCache();
			}
			GUILayout.Label("");

			DrawDefaultInspector();
		}
	}
}
