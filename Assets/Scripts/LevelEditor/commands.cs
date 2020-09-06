using System.Collections.Generic;
using UnityEngine;

namespace Toinen.LevelEditor {
	public interface ICommand {
		void Redo(EditorController ec);
		void Undo(EditorController ec);

		string ToString();
	}

	public class AddLineCommand : ICommand {
		int sx;
		int sy;
		
		public AddLineCommand(int sx, int sy) {
			this.sx = sx;
			this.sy = sy;
		}

		public void Redo(EditorController ec) {
			ec.DoAddLine(sx, sy);
		}
		public void Undo(EditorController ec) {
			ec.DoRemoveLine(sx, sy);
		}
	}

	public class RemoveLineCommand : ICommand {
		int sx;
		int sy;
		Stack<ICommand> commands = new Stack<ICommand>();

		public RemoveLineCommand(int sx, int sy) {
			this.sx = sx;
			this.sy = sy;
		}

		public void Redo(EditorController ec) {
			ec.DoRemoveLine(sx, sy);
			List<GameObject> removedObject = new List<GameObject>();
			foreach(Transform eo in ec.level.objectsDomain) {
				if (!ec.level.IsValidCoord((int)eo.position.x,(int)eo.position.y)) {
					removedObject.Add(eo.gameObject);
				}
			}
			foreach(var eo in removedObject) {
				ICommand command = new RemoveObjectCommand(eo.gameObject);
				command.Redo(ec);
				commands.Push(command);
			}
		}
		public void Undo(EditorController ec) {
			ec.DoAddLine(sx, sy);
			foreach(var c in commands) {
				c.Undo(ec);
			}
		}
	}

	public class PlaceObjectCommand : ICommand {
		public int x;
		public int y;
		public string code;
		public Level level { get; private set; }

		public EnvironmentObject placed { get; private set; } = null;
		public Color? color { get; private set; } = null;

		public PlaceObjectCommand(int x, int y, string code) {
			this.x = x;
			this.y = y;
			this.code = code;
		}

		public void Redo(EditorController ec) {
			placed = ec.AddFromPalette(x, y, code);
			level = ec.level;
			if (color.HasValue) {
				placed.color = color.Value;
			}
		}

		public void Undo(EditorController ec) {
			color = placed.color;
			GameObject.Destroy(placed.gameObject);
		}
	}

	public class RemoveObjectCommand : ICommand {
		public GameObject eo;
		Transform _originParent;

		public RemoveObjectCommand(GameObject eo) {
			this.eo = eo;
		}

		public void Redo(EditorController ec) {
			_originParent = eo.transform.parent;
			eo.transform.SetParent(ec.removedDomain, true);
		}

		public void Undo(EditorController ec) {
			eo.transform.SetParent(_originParent, true);
		}
	}
}
