using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Entities/Entity")]
	[DisallowMultipleComponent]
	public sealed class Entity : MonoBehaviour {
		[SerializeField]
		private float constSpeed = 3f;
		[SerializeField]
		private float constJumpForce = 7000f;
		[SerializeField]
		private float jumpDelay = 0.15f;

		public float speed { get; private set; }
		public float jumpForce { get; private set; }

		float timeSinceViscosity = 0f;

		private float _viscosityMedium;
		public float viscosityMedium {
			set {
				timeSinceViscosity = 0;
				if(_viscosityMedium != value) {
					Rigidbody2D rigid = GetComponent<Rigidbody2D>();
					if (rigid == null) {
						rigid = GetComponentInParent<Rigidbody2D>();
					}
					_viscosityMedium = value;
					rigid.gravityScale = 1/ value/value;
					speed = constSpeed/ value;
					jumpForce =constJumpForce/ value/value;
				}
			}
		}

		public Vector2 direction = Vector2.right;
		public bool isRight {
			get => direction.x > 0;
			set {
				if (value) {
					direction = Vector2.right;
				} else {
					direction = Vector2.left;
				}
			}
		}
		

		public float _jumpDelay { get; set; }

		public bool isOnGround { get { return _jumpDelay>=jumpDelay; } }

		[Header("Components")]
		public Transform eyesPoint;
		public Transform footPoint;

		[HideInInspector]
		public Level level;

		public Vector2 position {
			get => new Vector2(transform.position.x, transform.position.y);
			set {
				transform.position = new Vector3(value.x, value.y, transform.position.z);
			}
		}

		public bool isNowOnGround {
			get {
				RaycastHit2D hit;
				if (hit = Physics2D.Raycast(footPoint.position, Vector2.down, 0.1f)) {
					return !hit.transform.CompareTag("Player");
				}
				return false;
			}
		}

		void Start() {
			viscosityMedium = 1f;
			if (level == null) {
				level = GetComponentInParent<Level>();
			}
		}

		void FixedUpdate() {
			timeSinceViscosity += Time.fixedDeltaTime;
			if(timeSinceViscosity >= 0.15f) {
				viscosityMedium = 1f;
			}
			if (isNowOnGround) {
				_jumpDelay += Time.fixedDeltaTime;
			} 
		}
	}
}
