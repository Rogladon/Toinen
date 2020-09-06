using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Entities/Player")]
	[DisallowMultipleComponent]
	public sealed class Player : MonoBehaviour {
		public enum Id {
			PEA,
			TOMATO
		}

		[SerializeField]
		private Id _id;
		public Id id => _id;

		public Entity entity { get; private set; }
		public EntityStateMachine stateMachine { get; private set; }

		void Start() {
			entity = GetComponent<Entity>();
			stateMachine = GetComponent<EntityStateMachine>();
		}
	}
}
