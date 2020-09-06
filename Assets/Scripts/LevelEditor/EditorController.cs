using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Toinen;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Toinen.LevelEditor {
	[AddComponentMenu("Toinen/LevelEditor/Editor Controller")]
	[DisallowMultipleComponent]
	public sealed class EditorController : MonoBehaviour, IViewModelProvider {
		public RoomElementsPalette palette;

		int minWidth = 24;
		int minHeight = 13;

		[Header("Components")]
		public LevelGrid grid;
		// public Palette paletteController;
		public PaletteMenu paletteMenu;
		public RectTransform colorChooser;
		public Text txtSaveDestination;
		public RectTransform editingDomain;
		public RectTransform navigationDomain;
		public RectTransform confirmationClearRoom;
		public Transform removedDomain;
		public Text lblBusy;
		float busyTimer = 1000f;

		public CameraController cameraController;
		public Level level { get; private set; }

		public bool isNavigating { get { return (cameraController.zoom >= 2); } private set { } }
		public bool isEditing {
			get => !isNavigating;
			private set => isNavigating = !value;
		}

		public bool isDeletingMode = false;

		int lastSelectedCellX, lastSelectedCellY;

		[SerializeField]
		private int spanAutoSave = 30;

		public ViewModel viewModel { get; } = new ViewModel();

		void Start() {
			StartCoroutine(nameof(AutoSave));
			SceneLoader.editing = true;

			if (level == null) {
				level = GameObject.FindGameObjectWithTag(Level.TAG).GetComponent<Level>();
				if (level == null) {
					Debug.Log($"Failed to find {Level.TAG}");
				}
			}
			// paletteController.SetPalette(palette);

			grid.events.cellClicked.AddListener((int x, int y) => {
				var rt = level.GetAt(x, y);
				if (isDeletingMode) {
					if (rt != null) {
						Debug.Log($"Deleting at {x}:{y}");
						Do(new RemoveObjectCommand(rt.gameObject));
					}
				} else {
					if (paletteMenu.selected != null) {
						if (CanAddAt(x, y, paletteMenu.selected)) {
							Do(new PlaceObjectCommand(x, y, paletteMenu.selected));
						} else {
							busyTimer = 0;
						}
					}
				}
			});
			cameraController.Centrize(14);
			cameraController.transform.position += Vector3.up * 2;

			colorChooser.gameObject.SetActive(false);
			paletteMenu.gameObject.SetActive(false);

			foreach (Transform c in colorChooser.transform) {
				Image i = c.GetComponent<Image>();
				Button b = c.GetComponent<Button>();
				if (i == null || b == null) {
					continue;
				}
				b.onClick.AddListener(() => {
					colorChooser.gameObject.SetActive(false);
					level.GetAt(lastSelectedCellX, lastSelectedCellY).GetComponent<EnvironmentObject>().color = i.color;
				});
			}

			grid.events.cellClicked.AddListener((int x, int y) => {
				lastSelectedCellX = x;
				lastSelectedCellY = y;
				EnvironmentObject eo;
				if ((eo = level.GetAt(x, y)) != null && !isDeletingMode) {
					if (eo.category == EnvironmentObject.Categories.ACTING || eo.category == EnvironmentObject.Categories.TRIGGER) {
						foreach (Transform c in colorChooser.transform) {
							Image i = c.GetComponent<Image>();
							Button b = c.GetComponent<Button>();
							i.GetComponent<Button>().interactable = !i.color.Equals(eo.color);
						}
						colorChooser.gameObject.SetActive(eo.usesColors);
					} else {
						colorChooser.gameObject.SetActive(false);
					}
				}
			});

			viewModel.Use(this, "");

			uploading.SetActive(false);
		}

		void Update() {
			if (paletteMenu.palette == null) {
				paletteMenu.SetPalette(palette);
			}
			busyTimer += Time.deltaTime;
		}

		Stack<ICommand> doneCommands = new Stack<ICommand>();
		Stack<ICommand> undoneCommands = new Stack<ICommand>();

		public bool canUndo => doneCommands.Count > 0;
		public bool canRedo => undoneCommands.Count > 0;
		public bool canOpenPalette => !isDeletingMode;
		public bool canPlay => true;
		public bool wasJustBusy => busyTimer < 1.75f;

		public void Do(ICommand command) {
			command.Redo(this);
			doneCommands.Push(command);
			undoneCommands.Clear();
		}

		public void TogglePaletteMenu() {
			paletteMenu.gameObject.SetActive(!paletteMenu.gameObject.activeSelf);
		}

		public void Redo() {
			if (undoneCommands.Count > 0) {
				ICommand c = undoneCommands.Peek();
				c.Redo(this);
				undoneCommands.Pop();
				doneCommands.Push(c);
			}
		}

		public void Undo() {
			if (doneCommands.Count > 0) {
				ICommand c = doneCommands.Peek();
				c.Undo(this);
				doneCommands.Pop();
				undoneCommands.Push(c);
			}
		}

		public EnvironmentObject AddFromPalette(int x, int y, string code) {
			var eoPrefab = palette.environmentObjects[code];
			RectTransform rt = eoPrefab.GetComponent<RectTransform>();
			if (!CanAddAt(x, y, code)) {
				return null;
			}
			var eo = Instantiate(eoPrefab);
			eo.paletteCode = code;
			eo.transform.position = new Vector3(x, y, 0);
			eo.transform.SetParent(level.objectsDomain, false);
			return eo;
		}

		public bool CanAddAt(int x, int y, string code) {
			var eoPrefab = palette.environmentObjects[code];
			RectTransform rt = eoPrefab.GetComponent<RectTransform>();
			if (rt != null) {
				for (int dy = 0; dy < rt.rect.height; dy++) {
					for (int dx = 0; dx < rt.rect.width; dx++) {
						if (!level.IsValidCoord(x + dx, y + dy) || level.GetAt(x + dx, y + dy) != null) {
							Debug.Log($"Busy at {x + dx}:{y + dy}");
							return false;
						}
					}
				}
			}
			return true;
		}

		public void DoAddLine(int sx, int sy) {
			if (sx < 0) {
				level.rect.xMin += sx;
			}
			if (sx > 0) {
				level.rect.xMax += sx;
			}
			if (sy < 0) {
				level.rect.yMin += sy;
			}
			if (sy > 0) {
				level.rect.yMax += sy;
			}
			Debug.Log(level.rect.position + " " + level.rect.size);
		}

		public void DoRemoveLine(int sx, int sy) {
			if (sx < 0) {
				level.rect.xMin -= sx;
			}
			if (sx > 0) {
				level.rect.xMax -= sx;
			}
			if (sy < 0) {
				level.rect.yMin -= sy;
			}
			if (sy > 0) {
				level.rect.yMax -= sy;
			}
			Debug.Log(level.rect.position + " " + level.rect.size);
		}

		public bool CanRemoveLine(int sx, int sy) {
			var rect = level.rect;
			if (sx < 0) {
				rect.xMin -= sx;
			}
			if (sx > 0) {
				rect.xMax -= sx;
			}
			if (sy < 0) {
				rect.yMin -= sy;
			}
			if (sy > 0) {
				rect.yMax -= sy;
			}
			return rect.width >= minWidth && rect.height >= minHeight;
		}

		public void RemoveLineIfPossible(int sx, int sy) {
			if (CanRemoveLine(sx, sy)) {
				Do(new RemoveLineCommand(sx, sy));
			}
		}


		public void AddLineUpper() => Do(new AddLineCommand(0, 1));
		public void AddLineLefter() => Do(new AddLineCommand(-1, 0));
		public void AddLineLower() => Do(new AddLineCommand(0, -1));
		public void AddLineRighter() => Do(new AddLineCommand(1, 0));

		public void RemoveLineUpper() => RemoveLineIfPossible(0, 1);
		public void RemoveLineLefter() => RemoveLineIfPossible(-1, 0);
		public void RemoveLineLower() => RemoveLineIfPossible(0, -1);
		public void RemoveLineRighter() => RemoveLineIfPossible(1, 0);

		public void Navigate() {
			// isNavigating = true;
			if (cameraController.zoom < 2) {
				cameraController.zoom = 2;
			} else {
				cameraController.zoom = 1.4f;
			}
		}

		public void Edit() {
			isEditing = true;
		}

		public void ToggleDeleting() {
			isDeletingMode = !isDeletingMode;
		}

		// public void DoSelectCategory(string categoryName) {
		// 	isDeletingMode = false;
		// 	EnvironmentObject.Categories category;
		// 	try {
		// 		category = (EnvironmentObject.Categories)Enum.Parse(typeof(EnvironmentObject.Categories), categoryName);
		// 	} catch (ArgumentException e) {
		// 		Debug.LogError($"Failed to select category {categoryName}. There is no such element in enum:\n{e.ToString()}");
		// 		return;
		// 	}
		// 	paletteController.SelectCategory(category);
		// 	foreach (Transform btn in categoriesBar.transform) {
		// 		btn.GetComponent<Button>().interactable = !btn.name.Contains(categoryName);
		// 	}
		// 	isDeletingMode = false;
		// }

		public void DoPlay() {
			var sLevel = (Serialized.Level)level;
			SceneLoader.LoadScene("play", new Dictionary<string, object>() { { LevelLoader.PARAM_LEVEL_JSON, JsonUtility.ToJson(sLevel) } });
		}

		public void DoExport() {
			// TODO: Экспорт для мобилок

			var sLevel = (Serialized.Level)level;
	
			string fileName = $"{sLevel.name}.json";

			if (fileName.Length > 0) {
				if (File.Exists(fileName)) {
					// TODO: проверка
				}

				var f = File.CreateText(fileName);
				f.Write(JsonUtility.ToJson(sLevel));
				f.Close();
				Debug.Log($@"Level has been successfully exported to ""{fileName}""");
				txtSaveDestination.text = $@"Saved to ""{fileName}""";
			}
		}

		public void ConfirmationClearRoom() {
			confirmationClearRoom.gameObject.SetActive(true);
		}

		public void ClearRoom() {
			foreach(var eo in level.objects) {
				Debug.Log(eo.name);
				Destroy(eo.gameObject);
			}
			confirmationClearRoom.gameObject.SetActive(false);
		}
		public void NoClearRoom() {
			confirmationClearRoom.gameObject.SetActive(false);
		}

		private IEnumerator AutoSave() {
			yield return new WaitForSecondsRealtime(spanAutoSave);
			SaveLevelCache();
			StartCoroutine(nameof(AutoSave));
		}

		public void SaveLevelCache() {
			var sLevel = (Serialized.Level)level;

			PlayerPrefs.SetString("LevelCache", JsonUtility.ToJson(sLevel));
		}

		public void Back() {
			SaveLevelCache();
			SceneLoader.LoadScene("main-menu");
		}

		[Header("Uploading")]
		public GameObject uploading;
		public Text txtStatus;
		public InputField inputName;
		public Button btnUpload;

		public void DoUpload() {
			if (inputName.text.Length < 4) {
				txtStatus.text = "Too short name";
				return;
			}
			txtStatus.text = "Uploading...";
			Debug.Log("Uploading level...");
			btnUpload.interactable = false;
			StartCoroutine(Api.UploadLevel("imjarek", JsonUtility.ToJson((Serialized.Level)level), (int id) => {
				Debug.Log($"Uploaded with id {id}");
				txtStatus.text = $"id: {id}";
				btnUpload.interactable = true;
			}, (string err) => {
				txtStatus.text = err;
				btnUpload.interactable = true;
			}));
		}
	}
}