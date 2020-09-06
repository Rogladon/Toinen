using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Toinen.LevelEditor {
	public sealed class L18n : MonoBehaviour {
		public static string currentLanguage { get; private set; } = null;
		public static Hashtable dictionary { get; private set; }

		public string targetLanguage = "ru";

		void Start() {
			if (dictionary == null || currentLanguage != targetLanguage) {
				Load(targetLanguage);
			}
		}

		public string GetText(string k) {
			if (dictionary != null && k != null && dictionary.ContainsKey(k)) {
				return (string)dictionary[k];
			} else {
				return k;
			}
		}

		public static void UpdateElements() {
			foreach (var t in Resources.FindObjectsOfTypeAll<UITextLocalizer>()) {
				t.UpdateTranslation();
			}
		}

		public static void Load(string lang) {
			currentLanguage = lang;
			if (lang == "ru") {
				UpdateElements();
			} else {
				string path = $"l18n/{lang}.po";

				TextAsset ta = (TextAsset)Resources.Load(path);
				if (ta == null) {
					Debug.LogError($@"""{path}"" not found");
					return ;
				} else {
					if (dictionary == null) {
						dictionary = new Hashtable();
					}

					dictionary.Clear();

					StringReader reader = new StringReader(ta.text);
					string key = null;
					string val = null;
					string line;
					while ((line = reader.ReadLine()) != null) {
						if (line.StartsWith("msgid \"")) {
							key = line.Substring(7, line.Length - 8).ToUpper();
						} else if (line.StartsWith("msgstr \"")) {
							val = line.Substring(8, line.Length - 9);
						} else {
							if (key != null && val != null) {
								// TODO: add error handling here in case of duplicate keys
								dictionary.Add(key, val);
								key = val = null;
							}
						}
					}

					reader.Close();
				}
			}
		}
	}
}
