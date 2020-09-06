using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Level/Environment Objects/Exit"),
	DisallowMultipleComponent,
	RequireComponent(typeof(EnvironmentObject))]
	public class Exit : MonoBehaviour {
		public Player.Id targetId;

		public bool isOk => enters > 0;

		int enters = 0;

		EnvironmentObject environmentObject;

		Player catchedPlayer;

		void Start() {
			environmentObject = GetComponent<EnvironmentObject>();
		}

		void Update() {
			if (catchedPlayer != null) {
				// Притягивание игрока к центру выхода по Oy
				catchedPlayer.transform.position = new Vector3(
					Mathf.Lerp(catchedPlayer.transform.position.x, transform.position.x + 0.5f, 2f * Time.deltaTime),
					catchedPlayer.transform.position.y,
					catchedPlayer.transform.position.z);
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			Player player = other.GetComponent<Player>();
			if (player == null) {
				player = other.GetComponentInParent<Player>();
			}
			// Проверка на завершение уровня
			if (player != null && player.id == targetId) {
				enters++;
				environmentObject.level.CheckExits();
				catchedPlayer = player;
			}
		}

		void OnTriggerExit2D(Collider2D other) {
			Player player = other.GetComponent<Player>();
			if (player == null) {
				player = other.GetComponentInParent<Player>();
			}
			if (player != null && player.id == targetId) {
				enters--;
				catchedPlayer = null;
			}
		}
	}
}
