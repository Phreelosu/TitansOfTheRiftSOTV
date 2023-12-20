using System;
using EntityStates;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Herald {

	public class Attack : BaseSkillState
	{
		public int swingIndex;

		public string hitboxName;

		protected DamageType damageType = DamageType.Generic;

		protected float damageCoefficient = 2.5f;

		protected float procCoefficient = 1f;

		protected float pushForce = 3000f;

		protected Vector3 bonusForce = Vector3.zero;

		protected float baseDuration = 1f;

		protected float attackStartTime = 0.43f;

		protected float attackEndTime = 0.6f;

		protected bool cancelled = false;

		protected float animDuration;

		protected string swingSoundString = "";

		protected string hitSoundString = "";

		private string muzzleString;

		private GameObject swingEffectPrefab;

		private GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		public static Vector3 punch3Force = Vector3.up * 2000f;

		public float duration;

		private bool hasFired;

		private OverlapAttack attack;

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
			duration = baseDuration / attackSpeedStat;
			swingEffectPrefab = null;
			System.Random random = new System.Random();
			int num = random.Next(1, 3);
			muzzleString = "Attack" + num;
			PlayCrossfade("Body", "Attack" + num, "Slash.playbackRate", 0.8f, 0.05f);
			hitboxName = "Attack" + num;
			Util.PlaySound("HeraldSwing", base.gameObject);
			swingSoundString = "RekSwing";
			hitSoundString = "HeraldHit";
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
