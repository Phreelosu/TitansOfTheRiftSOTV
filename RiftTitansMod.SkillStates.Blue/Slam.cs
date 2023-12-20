using System;
using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Blue {

	public class Slam : BaseSkillState
	{
		public int swingIndex;

		public string hitboxName;

		protected DamageType damageType = DamageType.Generic;

		public static float damageCoefficient = 5f;

		public static float procCoefficient = 1f;

		public static float blastRadius = 9f;

		protected float pushForce = 2000f;

		protected Vector3 bonusForce = Vector3.up * 2000f;

		protected float baseDuration = 1.5f;

		protected float attackStartTime = 0.4f;

		private float radius;

		protected string swingSoundString = "";

		protected string hitSoundString = "";

		private string muzzleString;

		private Transform muzzleTransform;

		private GameObject swingEffectPrefab;

		private GameObject hitEffectPrefab;

		public float duration;

		private bool hasFired;

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
			swingEffectPrefab = Assets.blueSlamEffect;
			radius = blastRadius;
			duration = baseDuration / attackSpeedStat;
			System.Random random = new System.Random();
			int num = random.Next(1, 4);
			if (num == 3)
			{
				bonusForce *= 2f;
				pushForce *= 2f;
				radius *= 1.5f;
			}
			muzzleString = "Slam" + num;
			if ((bool)base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild(muzzleString);
				}
			}
			Util.PlaySound("BlueSwing", base.gameObject);
			PlayCrossfade("Body", "Attack" + num, "Slash.playbackRate", 0.8f, 0.05f);
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

		private void FireAttack()
		{
			if (!hasFired)
			{
				hasFired = true;
				if (base.isAuthority)
				{
					PlaySwingEffect();
					Util.PlaySound("BlueHit", base.gameObject);
					BlastAttack blastAttack = new BlastAttack();
					blastAttack.attacker = base.gameObject;
					blastAttack.procChainMask = default(ProcChainMask);
					blastAttack.impactEffect = EffectIndex.Invalid;
					blastAttack.losType = BlastAttack.LoSType.NearestHit;
					blastAttack.damageColorIndex = DamageColorIndex.Default;
					blastAttack.damageType = DamageType.Generic;
					blastAttack.procCoefficient = procCoefficient;
					blastAttack.bonusForce = Vector3.up * pushForce;
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
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			stopwatch += Time.fixedDeltaTime;
			if (stopwatch >= duration * attackStartTime)
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
