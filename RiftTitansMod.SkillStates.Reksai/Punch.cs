using System;
using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Reksai {

	public class Punch : BaseSkillState
	{
		public int swingIndex;

		public string hitboxName;

		protected DamageType damageType = DamageType.Generic;

		protected float damageCoefficient = 4f;

		protected float procCoefficient = 1f;

		protected float pushForce = 2000f;

		protected Vector3 bonusForce = Vector3.zero;

		protected float baseDuration = 1f;

		protected float attackStartTime = 0.32f;

		protected float attackEndTime = 0.8f;

		protected float baseEarlyExitTime = 0f;

		protected float hitStopDuration = 0.012f;

		protected float attackRecoil = 0.75f;

		protected float hitHopVelocity = 4f;

		protected bool cancelled = false;

		protected float animDuration;

		protected string swingSoundString = "";

		protected string hitSoundString = "";

		private string muzzleString;

		private GameObject swingEffectPrefab;

		private GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		public static float punchDuration = 1.5f;

		public static float punchStart = 0.25f;

		public static float punchEnd = 0.4f;

		public static float punch3Duration = 1.5f;

		public static float punch3Start = 0.25f;

		public static float punch3End = 0.4f;

		public static Vector3 punch3Force = Vector3.up * 2000f;

		public float duration;

		private bool hasFired;

		private OverlapAttack attack;

		protected bool inHitPause;

		protected float stopwatch;

		protected Animator animator;

		public override void OnEnter()
		{
			base.OnEnter();
			hasFired = false;
			animator = GetModelAnimator();
			StartAimMode(0.5f + duration);
			base.characterBody.outOfCombatStopwatch = 0f;
			animator.SetBool("attacking", value: true);
			swingEffectPrefab = Assets.reksaiAttackEffect;
			System.Random random = new System.Random();
			int num = random.Next(1, 5);
			switch (num)
			{
				case 1:
					duration = punchDuration;
					attackStartTime = punchStart;
					attackEndTime = punchEnd;
					hitboxName = "SwipeRight";
					break;
				case 2:
					duration = punch3Duration;
					attackStartTime = punch3Start;
					attackEndTime = punch3End;
					pushForce = 0f;
					bonusForce = punch3Force;
					hitboxName = "Uppercut";
					break;
				case 3:
					duration = punchDuration;
					attackStartTime = punchStart;
					attackEndTime = punchEnd;
					hitboxName = "SwipeLeft";
					break;
				case 4:
					duration = punch3Duration;
					attackStartTime = punch3Start;
					attackEndTime = punch3End;
					pushForce = 0f;
					bonusForce = punch3Force;
					hitboxName = "Uppercut";
					break;
				default:
					duration = punchDuration;
					attackStartTime = punchStart;
					attackEndTime = punchEnd;
					hitboxName = "SwipeRight";
					break;
			}
			muzzleString = "Attack" + num;
			PlayCrossfade("Body", "Attack" + num, "Slash.playbackRate", 0.8f, 0.05f);
			Util.PlaySound("RekAttackVoice", base.gameObject);
			swingSoundString = "RekSwing";
			hitSoundString = "RekHit";
			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = GetModelTransform();
			if ((bool)modelTransform)
			{
				hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxName);
			}
			attack = new OverlapAttack();
			attack.damageType = damageType;
			attack.attacker = base.gameObject;
			attack.inflictor = base.gameObject;
			attack.teamIndex = GetTeam();
			attack.damage = damageCoefficient * damageStat;
			attack.procCoefficient = procCoefficient;
			attack.hitEffectPrefab = hitEffectPrefab;
			attack.forceVector = bonusForce;
			attack.pushAwayForce = pushForce;
			attack.hitBoxGroup = hitBoxGroup;
			attack.isCrit = RollCrit();
			attack.impactSound = impactSound;
		}

		protected virtual void PlayAttackAnimation()
		{
		}

		public override void OnExit()
		{
			base.OnExit();
			if ((bool)animator)
			{
				animator.SetBool("attacking", value: false);
			}
		}

		protected virtual void PlaySwingEffect()
		{
			EffectManager.SimpleMuzzleFlash(swingEffectPrefab, base.gameObject, muzzleString, transmit: true);
		}

		protected virtual void OnHitEnemyAuthority()
		{
			Util.PlaySound(hitSoundString, base.gameObject);
		}

		private void FireAttack()
		{
			if (!hasFired)
			{
				hasFired = true;
				Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, attackSpeedStat);
				if (base.isAuthority)
				{
					PlaySwingEffect();
					AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
				}
			}
			if (base.isAuthority && attack.Fire())
			{
				OnHitEnemyAuthority();
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			stopwatch += Time.fixedDeltaTime;
			if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
			{
				FireAttack();
			}
			if (stopwatch >= duration && base.isAuthority)
			{
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(swingIndex);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			swingIndex = reader.ReadInt32();
		}
	}
}
