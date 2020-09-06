using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour {
	public bool spinning = true;
	public RectTransform rt;
	public float degreesPerSecond = 180;

	void Start() {
		if (rt == null) {
			TryGetComponent(out rt);
		}
	}

	void Update() {
		if (rt != null && spinning) {
			rt.Rotate(new Vector3(0, 0, degreesPerSecond * Time.deltaTime));
		}
		// SPINNING OF COMMONT TRANSFORM
	}
}
