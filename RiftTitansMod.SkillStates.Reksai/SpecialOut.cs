using System;
using EntityStates;
using RiftTitansMod.Modules;
using RiftTitansMod.Modules.Components.Reksai;
using RoR2;
using RoR2.Audio;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Reksai {

	public class SpecialOut : BaseSkillState
	{
		public static float damageCoefficient = 16f;

		public static float procCoefficient = 1f;

		public static float baseDuration = 1f;

		public static float throwForce = 80f;

		public static float speedCoefficient = 3.35f;

		public float seekTime = 0.1f;

		private ReksaiSpecialTracker specialTracker;

		public string hitboxName = "Special";

		private OverlapAttack attack;

		protected DamageType damageType = DamageType.Generic;

		protected float pushForce = 2000f;

		protected Vector3 bonusForce = Vector3.zero;

		protected float attackStartTime = 0.625f;

		protected float attackEndTime = 0.75f;

		protected string swingSoundString = "";

		protected string hitSoundString = "";

		protected string muzzleString = "SwingCenter";

		protected GameObject swingEffectPrefab;

		protected GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		private float duration;

		private float fireTime;

		private bool hasFired;

		private bool hit;

		private Animator animator;

		private Vector3 lockedDirection;

		public HurtBox target;

		public Vector3 targetPosition;

		private bool locked;

		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration;
			fireTime = 0.5f * duration;
			animator = GetModelAnimator();
			PlayCrossfade("Body", "Emerge", "ThrowBomb.playbackRate", duration, 0.05f);
			base.characterBody.SetAimTimer(2f);
			specialTracker = GetComponent<ReksaiSpecialTracker>();
			Util.PlaySound("RekUnburrow", base.gameObject);
			Util.PlaySound("RekUnburrowVoice", base.gameObject);
			EffectManager.SimpleEffect(Assets.reksaiBurrowEffect, base.transform.position, Quaternion.identity, transmit: true);
			swingEffectPrefab = Assets.reksaiSpecialEffect;
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
			if ((bool)target)
			{
				base.characterDirection.forward = (target.transform.position - base.transform.position).normalized;
			}
		}

		public override void OnExit()
		{
			if ((bool)specialTracker && (bool)specialTracker.GetTrackingTarget())
			{
				specialTracker.trackingTarget = null;
			}
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if ((bool)target)
			{
				targetPosition = target.transform.position;
			}
			_ = targetPosition;
			Vector3 vector = ((!(targetPosition != Vector3.zero)) ? base.inputBank.aimDirection : (targetPosition - base.transform.position));
			if (base.fixedAge <= seekTime * duration)
			{
				Vector3 normalized = vector.normalized;
				base.characterMotor.rootMotion += moveSpeedStat * normalized * speedCoefficient * Time.fixedDeltaTime;
			}
			else
			{
				if (!locked)
				{
					lockedDirection = vector.normalized;
					locked = true;
				}
				float num = Mathf.Lerp(speedCoefficient, 1f, (base.fixedAge - fireTime) / (duration - fireTime));
				base.characterMotor.rootMotion += moveSpeedStat * lockedDirection * num * Time.fixedDeltaTime;
			}
			if (base.fixedAge >= fireTime && !hit)
			{
				hit = true;
				PlayCrossfade("Body", "Hit", "ThrowBomb.playbackRate", duration, 0.05f);
			}
			if (base.fixedAge >= duration * attackStartTime && base.fixedAge <= duration * attackEndTime)
			{
				Fire();
			}
			if (base.fixedAge >= duration + 0.5f && base.isAuthority)
			{
				outer.SetNextStateToMain();
			}
		}

		private void Fire()
		{
			if (!hasFired)
			{
				hasFired = true;
				Util.PlayAttackSpeedSound(swingSoundString, base.gameObject, attackSpeedStat);
				if (base.isAuthority)
				{
					EffectManager.SimpleMuzzleFlash(swingEffectPrefab, base.gameObject, "SpecialRight", transmit: true);
					EffectManager.SimpleMuzzleFlash(swingEffectPrefab, base.gameObject, "SpecialLeft", transmit: true);
				}
			}
			if (base.isAuthority && attack.Fire())
			{
				Util.PlaySound(hitSoundString, base.gameObject);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
