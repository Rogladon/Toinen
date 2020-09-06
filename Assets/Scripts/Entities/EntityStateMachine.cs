using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toinen {
	[AddComponentMenu("Toinen/Entities/Entity StateMachine"),
	RequireComponent(typeof(Entity),typeof(Rigidbody2D)),
	DisallowMultipleComponent]
	public class EntityStateMachine : StateMachine {
		abstract class EntityState : MachineSpecificState<EntityStateMachine> {
			public Entity entity => mch.entity;
			public CapsuleCollider2D collider => mch.collider;
			public Rigidbody2D rigid => mch.rigid;
		}
		abstract class EntityIdleState : MachineSpecificIdleState<EntityStateMachine> {
			public Entity entity => mch.entity;
			public Rigidbody2D rigid => mch.rigid;
		}
		abstract class EntityOverseer : MachineSpecificStateOverseer<EntityStateMachine> {
			public Entity entity => mch.entity;
			
			public Rigidbody2D rigid => mch.rigid;
		}

		class StateIdle : EntityIdleState {
			public override void OnStateEnter() {
				if (mch.armature == null) return;
				mch.armature.animation.FadeIn(mch.animationIdle);
			}
		}

		class StateWalking : EntityState {
			public override void OnStateEnter() {
				if (mch.armature == null) return;
				mch.armature.animation.FadeIn(mch.animationRunning);
				//rigid.velocity += new Vector2(entity.speed * entity.direction.x, 0);
			}

			public override void FixedTick() {
				RaycastHit2D hit;
				var layerMask = 1 << 8;
				Debug.DrawRay(entity.footPoint.position + new Vector3(
					collider.size.x * collider.transform.localScale.x / 2 * entity.direction.x,
					0.1f,
					0), entity.direction * 0.1f, Color.red);
				Debug.DrawRay(entity.footPoint.position + new Vector3(
					collider.size.x * collider.transform.localScale.x / 2 * entity.direction.x,
					collider.size.y * collider.transform.localScale.y * 0.7f,
					0), entity.direction * 0.1f, Color.red);
				if ((hit = Physics2D.Raycast(entity.footPoint.position + new Vector3(
					collider.size.x * collider.transform.localScale.x / 2 * entity.direction.x, 
					0.1f, 
					0), 
					entity.direction, 0.1f,layerMask)) || (hit = Physics2D.Raycast(entity.footPoint.position + new Vector3(
					collider.size.x * collider.transform.localScale.x / 2 * entity.direction.x,
					collider.size.y * collider.transform.localScale.y * 0.9f,
					0),
					entity.direction, 0.1f, layerMask))) {
					return;
				}
				rigid.velocity = new Vector2(entity.speed * entity.direction.x, rigid.velocity.y);
				
			}

			public override void OnStateExit() {
				rigid.velocity = new Vector2(0, rigid.velocity.y);
			}
		}

		class AppearanceOverseer : EntityOverseer {
			public override void FixedPostTick() {
				if (entity.isRight)
					transform.localScale = new Vector3(1, 1, 1);
				else
					transform.localScale = new Vector3(-1, 1, 1);
			}
		}

		public DragonBones.UnityArmatureComponent armature;
		public string animationIdle;
		public string animationRunning;
		public Entity entity { get; private set; }
		public Rigidbody2D rigid { get; private set; }
		public new CapsuleCollider2D collider;

		StateIdle stateIdle;
		StateWalking stateWalking;

		public bool canJump => entity.isOnGround;

		public float jumpDelay;

		public override void Configure() {
			entity = GetComponent<Entity>();
			rigid = GetComponent<Rigidbody2D>();
			collider = GetComponentInChildren<CapsuleCollider2D>();

			ChangeState(stateIdle = new StateIdle());
			stateWalking = new StateWalking();

			AddOverseer(new AppearanceOverseer());
		}

		public void Go() {
			ChangeState(stateWalking);
		}

		public void GoLeft() {
			entity.isRight = false;
			ChangeState(stateWalking);
		}

		public void GoRight() {
			entity.isRight = true;
			ChangeState(stateWalking);
		}

		public void Idle() {
			ChangeState(stateIdle);
		}

		public void Jump() {
			if (canJump) {
				rigid.AddForce(Vector2.up * entity.jumpForce);
				entity._jumpDelay = 0;
			}
		}
	}
}

