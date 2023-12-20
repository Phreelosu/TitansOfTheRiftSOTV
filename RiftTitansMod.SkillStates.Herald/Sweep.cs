using System;
using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Herald {

	public class Sweep : BaseSkillState
	{
		public int swingIndex;

		public string hitboxName;

		public float radius = 13f;

		protected DamageType damageType = DamageType.Generic;

		protected float damageCoefficient = 6f;

		protected float procCoefficient = 1f;

		protected float pushForce = 3000f;

		protected Vector3 bonusForce = Vector3.up * 2000f;

		protected float baseDuration = 3f;

		private float slamTime = 0.6f;

		protected float attackStartTime = 0.6f;

		protected float attackEndTime = 0.7f;

		protected bool cancelled = false;

		protected float animDuration;

		protected string swingSoundString = "";

		protected string hitSoundString = "";

		private string muzzleString;

		private GameObject swingEffectPrefab;

		private GameObject hitEffectPrefab;

		protected NetworkSoundEventIndex impactSound;

		private Transform muzzleTransform;

		public float duration;

		private bool hasFired;

		private OverlapAttack attack;

		protected float stopwatch;

		protected Animator animator;

		private bool blast;

		private bool a;

		private float s1 = 0.6f;

		private bool b;

		private float s2 = 0.8f;

		public override void OnEnter()
		{
			base.OnEnter();
			hasFired = false;
			animator = GetModelAnimator();
			StartAimMode(0.5f + duration);
			animator.SetBool("attacking", value: true);
			duration = baseDuration / attackSpeedStat;
			swingEffectPrefab = Assets.reksaiAttackEffect;
			System.Random random = new System.Random();
			muzzleString = "Sweep";
			PlayCrossfade("Body", "SwingPunch", "Slash.playbackRate", 0.8f, 0.05f);
			hitboxName = "Sweep";
			Util.PlaySound("HeraldSweep", base.gameObject);
			swingSoundString = "RekSwing";
			hitSoundString = "RekHit";
			if ((bool)base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild("Slam");
				}
			}
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

		private void Fire()
		{
			if (!blast)
			{
				blast = true;
				EffectManager.SimpleEffect(Assets.heraldSlamEffect, muzzleTransform.position, Quaternion.identity, transmit: true);
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.attacker = base.gameObject;
				blastAttack.procChainMask = default(ProcChainMask);
				blastAttack.impactEffect = EffectIndex.Invalid;
				blastAttack.losType = BlastAttack.LoSType.NearestHit;
				blastAttack.damageColorIndex = DamageColorIndex.Default;
				blastAttack.damageType = DamageType.Generic;
				blastAttack.procCoefficient = procCoefficient;
				blastAttack.bonusForce = Vector3.up * 1500f;
				blastAttack.baseForce = 1500f;
				blastAttack.baseDamage = damageCoefficient * damageStat;
				blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
				blastAttack.radius = radius;
				blastAttack.position = muzzleTransform.position;
				blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
				blastAttack.teamIndex = GetTeam();
				blastAttack.inflictor = base.gameObject;
				blastAttack.crit = RollCrit();
				blastAttack.Fire();
			}
		}

		private void FireAttack()
		{
			if (!hasFired)
			{
				hasFired = true;
				Util.PlaySound("HeraldSlam", base.gameObject);
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
			if (base.fixedAge >= s1 && a)
			{
				a = true;
				Util.PlaySound("HeraldSweep", base.gameObject);
			}
			if (base.fixedAge >= s2 && b)
			{
				b = true;
				Util.PlaySound("HeraldSwing", base.gameObject);
			}
			stopwatch += Time.fixedDeltaTime;
			if (stopwatch >= duration * attackStartTime && stopwatch <= duration * attackEndTime)
			{
				FireAttack();
			}
			if (stopwatch >= duration * slamTime)
			{
				Fire();
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
