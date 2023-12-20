using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Herald {

	public class ChargeCrash : BaseSkillState
	{
		public int swingIndex;

		public string hitboxName;

		protected DamageType damageType = DamageType.Generic;

		public static float damageCoefficient = 6f;

		public static float procCoefficient = 1f;

		public static float blastRadius = 15f;

		protected float pushForce = 2000f;

		protected Vector3 bonusForce = Vector3.up * 2000f;

		protected float baseDuration = 2f;

		protected float attackStartTime = 0.3f;

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
			swingEffectPrefab = Assets.heraldCrashEffect;
			radius = blastRadius;
			duration = baseDuration / attackSpeedStat;
			if ((bool)base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild("Crash");
				}
			}
			PlayCrossfade("Body", "DashHit", "Slash.playbackRate", 0.8f, 0.05f);
			Util.PlaySound("HeraldChargeEnd", base.gameObject);
		}

		protected virtual void PlayAttackAnimation()
		{
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected virtual void PlaySwingEffect()
		{
			EffectManager.SimpleEffect(Assets.heraldSlamEffect, base.transform.position, Quaternion.identity, transmit: true);
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
					BlastAttack blastAttack = new BlastAttack();
					blastAttack.attacker = base.gameObject;
					blastAttack.procChainMask = default(ProcChainMask);
					blastAttack.impactEffect = EffectIndex.Invalid;
					blastAttack.losType = BlastAttack.LoSType.NearestHit;
					blastAttack.damageColorIndex = DamageColorIndex.Default;
					blastAttack.damageType = DamageType.Generic;
					blastAttack.procCoefficient = procCoefficient;
					blastAttack.bonusForce = Vector3.up * 3000f;
					blastAttack.baseForce = 300f;
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
				base.characterMotor.moveDirection = Vector3.zero;
				base.characterMotor.velocity /= 5f;
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
