using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputHelper {
	/// <summary>
	/// Нажат ли [где-либо] экран
	/// </summary>
	public static bool isTapped {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 1) {
				Touch[] touch = Input.touches;
				if (touch[0].phase == TouchPhase.Began) {
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl)) {
				return true;
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}

	public static bool isTapCanceled {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 1) {
				Touch[] touch = Input.touches;
				if (touch[0].phase == TouchPhase.Canceled) {
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonUp(0) && !Input.GetKey(KeyCode.LeftControl)) {
				return true;
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}

	public static bool isTapEnded {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 1) {
				Touch[] touch = Input.touches;
				if (touch[0].phase == TouchPhase.Ended) {
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonUp(0) && !Input.GetKey(KeyCode.LeftControl)) {
				return true;
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}
	public static Vector3 mousePos;
	public static Vector3 deltaPos;
	public static bool isStationary {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 1) {
				Touch[] touch = Input.touches;
				if (touch[0].phase == TouchPhase.Stationary) {
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl)) {
				mousePos = Input.mousePosition;
				return true;
			}
			if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl)) {
				if (Input.mousePosition == mousePos)
					return true;
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}

	public static Vector3 doubleDeltaPos;
	public static Vector3 doubleMiddleTouchPos;

	public static bool isDoubleMoved {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 2) {
				Touch[] touch = Input.touches;
				if (touch[0].phase == TouchPhase.Moved && touch[0].phase == TouchPhase.Moved) {
					Vector3 d1 = touch[0].deltaPosition;
					Vector3 d2 = touch[1].deltaPosition;
					doubleDeltaPos = (d1 + d2) / 2;
					doubleMiddleTouchPos = (touch[0].position + touch[1].position) / 2;
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonDown(0)) {
				doubleMiddleTouchPos = Input.mousePosition;
				return false;
			}
			if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)) {
				if (Input.mousePosition != doubleMiddleTouchPos) {
					doubleDeltaPos = doubleMiddleTouchPos - Input.mousePosition;
					doubleMiddleTouchPos = Input.mousePosition;
					return true;
				}
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}

	public static bool isMoved {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 1) {
				Touch[] touch = Input.touches;
				if (touch[0].phase == TouchPhase.Moved) {
					mousePos = touch[0].position;
					deltaPos = -touch[0].deltaPosition;
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl)) {
				mousePos = Input.mousePosition;
				return false;
			}
			if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl)) {
				if (Input.mousePosition != mousePos) {
					deltaPos = mousePos - Input.mousePosition;
					mousePos = Input.mousePosition;
					return true;
				}
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}
	private static float distanceTouches;
	public static float deltaScaler;
	public static bool isScaler {
		get {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (Input.touches.Length == 2) {
				Touch[] touch = Input.touches;
				if(touch[0].phase == TouchPhase.Began || touch[1].phase == TouchPhase.Began) {
					distanceTouches = Vector3.Distance(touch[0].position, touch[1].position);
				}
				if (touch[0].phase == TouchPhase.Moved || touch[1].phase == TouchPhase.Moved) {
					deltaScaler = (distanceTouches - Vector3.Distance(touch[0].position, touch[1].position))/(Screen.height/8);
					distanceTouches = Vector3.Distance(touch[0].position, touch[1].position);
					return true;
				}
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (Input.mouseScrollDelta != Vector2.zero) {
				deltaScaler = -Input.mouseScrollDelta.y;
				return true;
			}
#else
#error Not implemented for this platform
#endif
			return false;
		}
	}
}
