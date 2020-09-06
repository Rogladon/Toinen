using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Environment Objects/ViscousMedium"),
	DisallowMultipleComponent,
	PlayModeOnly,
	RequireComponent(typeof(EnvironmentObject))]
	public class ViscousMedium : MonoBehaviour {
		public float viscosity = 2f;
		public float forceSinceTime = 1f;

		private void OnTriggerEnter2D(Collider2D other) {
			Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
			if (rigid == null) {
				rigid = other.GetComponentInParent<Rigidbody2D>();
			}

			// Замедление тела
			var vel = rigid.velocity;
			if(vel.y < -(0.2f*rigid.mass/Mathf.Pow(viscosity,2)))
				vel.y = -(0.2f * rigid.mass / Mathf.Pow(viscosity, 2));
			rigid.velocity = vel;
		}

		void OnTriggerStay2D(Collider2D other) {
			if (other.tag == "Player" || other.transform.parent.tag == "Player") {
				Entity entity = other.GetComponent<Entity>();
				if (entity == null) {
					entity = other.GetComponentInParent<Entity>();
				}
				entity.viscosityMedium = viscosity;
				return;
			}
			Rigidbody2D rigid = other.GetComponent<Rigidbody2D>();
			if (rigid == null) {
				rigid = other.GetComponentInParent<Rigidbody2D>();
			}
			rigid.velocity *= forceSinceTime * Time.deltaTime;
		}

		//private void OnTriggerExit2D(Collider2D other) {
		//	Entity entity = other.GetComponent<Entity>();
		//	if (entity == null) {
		//		entity = other.GetComponentInParent<Entity>();
		//	}
		//	entity.viscosity = 1;
		//}
	}
}
