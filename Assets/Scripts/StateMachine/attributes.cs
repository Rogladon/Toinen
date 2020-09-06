using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toinen {
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class StateAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class StateAnimation : StateAttribute {
		public string animationName { get; private set; }
		public float fadeInTime { get; private set; }

		public StateAnimation(string animationName, float fadeInTime = -1) {
			this.animationName = animationName;
			this.fadeInTime = fadeInTime;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public abstract class StateOverseerAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OffOnState: Attribute {
		public Type type { get; private set; }

		public OffOnState(Type t) {
			this.type = t;
		}
	}
}
