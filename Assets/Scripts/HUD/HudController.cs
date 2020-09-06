using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/HUD/HUD Controller")]
	[DisallowMultipleComponent]
	public sealed class HudController : MonoBehaviour, IViewModelProvider {
		[HideInInspector]
		public List<Player> players = new List<Player>();
		[HideInInspector]
		public Player currentPlayer;

		public Level level;

		public bool useKeyboard = false;

		[Header("Components")]
		public GameObject controlsDomain;
		public GameObject windowComplete;
		public GameObject windowMenu;

		public ViewModel viewModel { get; } = new ViewModel();

		public CameraController cameraController { get; private set; }

		void Start() {

			cameraController = GetComponentInParent<CameraController>();
			level.onEntitySpawn.AddListener(CatchPlaySpawn);
			level.onLevelComplete.AddListener(OnLevelComplete);

			viewModel.Use(this);
			windowMenu.SetActive(false);
			windowComplete.SetActive(false);
#if UNITY_EDITOR || UNITY_STANDALONE
			useKeyboard = true;
#endif
		}

		void FixedUpdate() {
			if (currentPlayer == null)
				return;
			cameraController.entity = currentPlayer.entity;
#if UNITY_EDITOR || UNITY_STANDALONE
			if (useKeyboard) {
				bool goLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
				bool goRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
				bool doJump = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);

				if (goLeftPressed) DoLeft();
				if (goRightPressed) DoRight();

				if (!(goLeftPressed || goRightPressed)) {
					DoStop();
				}

				if (doJump) DoJump();
			}
#endif
		}

		void Update() {
			windowComplete.SetActive(level.isCompleted);
			controlsDomain.SetActive(!level.isCompleted && !windowMenu.activeSelf);
		}

		public void CatchPlaySpawn(Entity entity) {
			if (entity.gameObject.CompareTag("Player")) {
				var p = entity.GetComponent<Player>();
				players.Add(p);
				Debug.Log($"Caught player {p}");
				if (currentPlayer == null) {
					currentPlayer = p;
				}
			}
		}

		public void Restart() {
			SceneLoader.LoadScene(SceneLoader.SCENE_PLAY, SceneLoader.loadParameters);
		}

		void OnLevelComplete(Level level) {
			// TODO:
		}

		public void DoLeft() {
			currentPlayer.stateMachine.GoLeft();
		}

		public void DoRight() {
			currentPlayer.stateMachine.GoRight();
		}

		public void DoStop() {
			currentPlayer.stateMachine.Idle();
		}

		public void DoJump() {
			currentPlayer.stateMachine.Jump();
		}

		public void DoBack() {
			if (SceneLoader.editing) {
				LevelLoader loader = level.GetComponent<LevelLoader>();
				SceneLoader.LoadScene("editor", new Dictionary<string, object>() { { LevelLoader.PARAM_LEVEL_JSON, loader.sourceJson } });
			} else {
				SceneLoader.LoadScene("main-menu");
			}
		}

		public void DoSelectNextPlayer() {
			if (players.Count < 1)
				return;
			Player newPlayer = players[0];
			bool takeNext = false;
			foreach (Player p in players) {
				if (takeNext) {
					newPlayer = p;
					break;
				} else if (p == currentPlayer) {
					takeNext = true;
				}
			}
			currentPlayer = newPlayer;
		}

		public void DoToggleKeyboardControls() {
			useKeyboard = !useKeyboard;
		}

		public void ToggleMenu() {
			windowMenu.SetActive(!windowMenu.activeSelf);
		}

		public bool vmb_canSelectNexPlayer => players.Count > 1;

		public bool vmb_disableTogglingKeyboardControls =>
#if UNITY_EDITOR || UNITY_STANDALONE
			false;
#else
			true;
#endif
	}
}