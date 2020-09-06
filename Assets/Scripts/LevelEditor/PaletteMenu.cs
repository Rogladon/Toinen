using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Toinen.LevelEditor {
	[AddComponentMenu("Toinen/LevelEditor/PaletteMenu")]
	[DisallowMultipleComponent]
	public sealed class PaletteMenu : MonoBehaviour {
		[Header("Prefabs")]
		public RectTransform itemPrefab;

		[Header("Components")]
		public Transform categoriesBar;
		public Transform content;

		public string selected { get; private set; }

		public RoomElementsPalette palette { get; private set; }
		public EnvironmentObject.Categories selectedCategory { get; private set; } = EnvironmentObject.Categories.PLATFORM;

		List<RectTransform> items = new List<RectTransform>();
		Dictionary<string, EnvironmentObject.Categories> itemsCategories = new Dictionary<string, EnvironmentObject.Categories>();

		public class SelectedEvent : UnityEvent<string, RoomElementsPalette> {}

		public class Events {
			public SelectedEvent selected = new SelectedEvent(); 
		}
		public Events events = new Events();

		public void SetPalette(RoomElementsPalette rep) {
			palette = rep;
			foreach (var kv in rep.environmentObjects) {
				var item = Instantiate(itemPrefab, content);
				item.name = kv.Key;
				itemsCategories.Add(kv.Key, kv.Value.category);
				item.GetComponentInChildren<Text>().text = kv.Value.paletteCode;
				if (kv.Value.paletteIcon != null) {
					foreach (Transform c in item) {
						if (c.name == "Icon") {
							Image i = c.GetComponent<Image>();
							i.sprite = kv.Value.paletteIcon;
							i.color = kv.Value.paletteIconColor;
						}
					}
				}
				item.GetComponent<Button>().onClick.AddListener(() => {
					foreach (RectTransform rt in items) {
						if (rt.name == kv.Key) {
							rt.localScale = Vector3.one * 0.8f;
						} else {
							rt.localScale = Vector3.one;
						}
					}
					if (kv.Key == selected) {
						selected = null;
					} else {
						selected = kv.Key;
					}
					Close();
					events.selected.Invoke(selected, palette);
				});
				items.Add(item);
				item.gameObject.SetActive(kv.Value.category == selectedCategory);
			}
		}

		public void SelectCategory(EnvironmentObject.Categories category) {
			selectedCategory = category;
			foreach (var item in items) {
				item.gameObject.SetActive(selectedCategory == itemsCategories[item.name]);
			}
		}

		public void DoSelectCategory(string categoryName) {
			EnvironmentObject.Categories category;
			try {
				category = (EnvironmentObject.Categories)Enum.Parse(typeof(EnvironmentObject.Categories), categoryName);
			} catch (ArgumentException e) {
				Debug.LogError($"Failed to select category {categoryName}. There is no such element in enum:\n{e.ToString()}");
				return;
			}
			SelectCategory(category);
			foreach (Transform btn in categoriesBar.transform) {
				btn.GetComponent<Button>().interactable = !btn.name.Contains(categoryName);
			}
		}

		public void Open() {
			gameObject.SetActive(true);
		}

		public void Close() {
			gameObject.SetActive(false);
		}
	}
}
