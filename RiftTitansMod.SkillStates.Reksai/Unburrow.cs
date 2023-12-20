using System;
using EntityStates;
using RiftTitansMod.Modules;
using RiftTitansMod.Modules.Components.Reksai;
using RoR2;
using RoR2.Audio;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Reksai {

	public class Unburrow : BaseSkillState
	{
		public int swingIndex;

		public string hitboxName = "Unburrow";

		public static float effectRadius = 13f;

		protected DamageType damageType = DamageType.Generic;

		protected float damageCoefficient = 4f;

		protected float procCoefficient = 1f;

		protected float pushForce = 1000f;

		protected Vector3 bonusForce = Vector3.up * 3750f;

		protected float baseDuration = 1f;

		protected float attackStartTime = 0.1f;

		protected float attackEndTime = 0.3f;

		protected bool cancelled = false;

		protected float animDuration;

		protected string swingSoundString = "";

		protected string hitSoundString = "";

		protected string muzzleString = "SwingCenter";

		protected GameObject swingEffectPrefab;

		protected GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		public float duration;

		private bool hasFired;

		private OverlapAttack attack;

		protected bool inHitPause;

		protected float stopwatch;

		protected Animator animator;

		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			hasFired = false;
			animator = GetModelAnimator();
			animator.SetBool("attacking", value: true);
			swingEffectPrefab = Assets.reksaiUnburrowEffect;
			PlayCrossfade("Body", "Unburrow", "Slash.playbackRate", 0.8f, 0.05f);
			PlayCrossfade("Body, Burrowed", "Unburrow", "Slash.playbackRate", 0.8f, 0.05f);
			Util.PlaySound("RekUnburrowVoice", base.gameObject);
			swingSoundString = "RekUnburrow";
			hitSoundString = "";
			muzzleString = "";
			hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleGuardSunderPop");
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
			ReksaiBurrowController component = GetComponent<ReksaiBurrowController>();
			if ((bool)component)
			{
				GetComponent<ReksaiBurrowController>().Unburrow();
			}
			animator.SetBool("attacking", value: false);
		}

		private void FireAttack()
		{
			if (!hasFired)
			{
				hasFired = true;
				Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, attackSpeedStat);
				if (base.isAuthority)
				{
					EffectManager.SimpleEffect(Assets.reksaiUnburrowEffect, base.transform.position, Quaternion.identity, transmit: true);
				}
			}
			if (base.isAuthority && attack.Fire())
			{
				Util.PlaySound(hitSoundString, base.gameObject);
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
	}
}
