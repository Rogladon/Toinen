using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toinen {
	public interface IDataBinding {
		void Refresh();
	}

	public interface IViewModelProvider {
		ViewModel viewModel { get; }
	}

	public class ViewModel {
		Dictionary<string, (MethodInfo getter, object obj)> db = new Dictionary<string, (MethodInfo getter, object obj)>();

		public void Use(object o, string prefix = "vmb_", string domainName = null) {
			foreach (var p in o.GetType().GetProperties()) {
				if (prefix.Length == 0 || p.Name.StartsWith(prefix)) {
					if (!p.GetMethod.IsPublic) {
						Debug.LogWarning($"Property {p.Name} (in {o} of type {o.GetType().Name}) got proper prefix, but no public get method");
						continue;
					}
					string fieldName = p.Name.Substring(prefix.Length);
					if (domainName != null && domainName.Length > 0) {
						fieldName = $"{domainName}:{fieldName}";
					}
					db.Add(fieldName, (p.GetMethod, o));
				}
			}
		}

		public string GetStringField(string fieldName) {
			if (db.ContainsKey(fieldName)) {
				(MethodInfo getter, object obj) = db[fieldName];
				return getter.Invoke(obj, new object[0]).ToString();
			} else {
				return $"{fieldName}???";
			}
		}

		public bool GetSimpleBoolField(string fieldName) {
			if (db.ContainsKey(fieldName)) {
				(MethodInfo getter, object obj) = db[fieldName];
				object r = getter.Invoke(obj, new object[0]);
				if (r is bool br) {
					return br;
				} else {
					return r != null;
				}
			} else {
				return false;
			}
		}

		/// <summary>
		/// <see cref="GetSimpleBoolField"> с поддержкой инвертирования
		/// </summary>
		public bool GetReversableBoolField(string fieldName) {
			bool reversed = fieldName.StartsWith("!");
			string key = reversed ? fieldName.Substring(1) : fieldName;
			bool value = GetSimpleBoolField(key);
			return reversed ? !value : value;
		}

		[System.Obsolete]
		public bool GetBoolField(string fieldName) => GetReversableBoolField(fieldName);

		public bool GetBoolExpression(string expression) {
			var processed = Regex.Replace(expression, @"\s+", "");
			foreach (string ors in processed.Split(new[] { "||" }, StringSplitOptions.None)) {
				bool sum = true;
				foreach (string ands in processed.Split(new[] { "&&" }, StringSplitOptions.None)) {
					if (ands.Length == 0) {
						Debug.LogWarning($@"Empty token in expression ""{expression}""");
						continue;
					}
					if (!GetReversableBoolField(ands)) {
						sum = false;
						break;
					}
				}
				if (sum) {
					return true;
				}
			}
			return false;
		}

		public T GetField<T>(string fieldName) {
			if (db.ContainsKey(fieldName)) {
				(MethodInfo getter, object obj) = db[fieldName];
				object r;
				try {
					r = getter.Invoke(obj, new object[0]);
				} catch (System.Exception e) {
					Debug.LogError(e);
					return default(T);
				}
				if (r is T tr) {
					return tr;
				} else {
					return default(T);
				}
			} else {
				return default(T);
			}
		}
	}
}