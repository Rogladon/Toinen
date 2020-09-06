using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Color Triggers")]
	[DisallowMultipleComponent]
	public sealed class ColorTriggers {
		public class TriggerEvent : UnityEvent<Color> {}

		public Dictionary<Color, float> sinceTriggeredTimes = new Dictionary<Color, float>();

		float triggeredSince = 0.1f;

		public bool this[Color color] => isTriggered(color);

		public bool isTriggered(Color color) {
			if (sinceTriggeredTimes.TryGetValue(color, out float since)) {
				return (Time.timeSinceLevelLoad - since) < triggeredSince;
			} else {
				return false;
			}
		}

		public void Trigger(Color c) {
			sinceTriggeredTimes[c] = Time.timeSinceLevelLoad;
		}
	}
}
