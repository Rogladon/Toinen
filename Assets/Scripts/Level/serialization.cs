using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toinen {
	public static class Serialized {
		[Serializable]
		public class Level {
			public string name;
			public int x, y, width, height;

			public Rect rect {
				get => new Rect(x, y, width, height);
				set {
					x = (int)value.x;
					y = (int)value.y;
					width = (int)value.width;
					height = (int)value.height;
				}
			}

			public EnvironmentObject[] objects;

			public static explicit operator Level(Toinen.Level l) {
				Level sLevel = new Level();
				sLevel.name = $"toinen-level-{DateTime.Now.ToString("yyyyMMdd-HHmm")}";
				sLevel.rect = l.rect;

				// Обход объектов окружения
				List<EnvironmentObject> sEnvironment = new List<EnvironmentObject>();
				foreach (var eo in l.objectsDomain.GetComponentsInChildren<Toinen.EnvironmentObject>()) {
					sEnvironment.Add((EnvironmentObject)eo);
				}
				sLevel.objects = sEnvironment.ToArray();
				return sLevel;
			}
		}

		[Serializable]
		public class EnvironmentObject {
			public int x, y;
			public string code;
			public string color;

			public static explicit operator EnvironmentObject(Toinen.EnvironmentObject l) {
				EnvironmentObject sObject = new EnvironmentObject();
				sObject.x = (int)l.position.x;
				sObject.y = (int)l.position.y;
				sObject.code = l.paletteCode;
				sObject.color = '#' + ColorUtility.ToHtmlStringRGB(l.color);
				return sObject;
			}
		}
	}
}
