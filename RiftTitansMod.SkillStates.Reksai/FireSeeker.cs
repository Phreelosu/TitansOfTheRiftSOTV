using EntityStates;
using RiftTitansMod.Modules;
using RiftTitansMod.Modules.Components.Reksai;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Reksai {

	public class FireSeeker : BaseSkillState
	{
		public static float damageCoefficient = 3f;

		public static float procCoefficient = 1f;

		public static float baseDuration = 0.65f;

		public static float throwForce = 80f;

		private float duration;

		private float fireTime;

		private bool hasFired;

		private Animator animator;

		public override void OnEnter()
		{
			base.OnEnter();
			if (!GetComponent<ReksaiBurrowController>() || !GetComponent<ReksaiBurrowController>().burrowed)
			{
				outer.SetNextStateToMain();
				base.skillLocator.secondary.AddOneStock();
				return;
			}
			duration = baseDuration / attackSpeedStat;
			fireTime = 0.35f * duration;
			animator = GetModelAnimator();
			PlayAnimation("Body, Burrowed", "Seeker", "ThrowBomb.playbackRate", duration);
			base.characterBody.SetAimTimer(2f);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		private void Fire()
		{
			if (!hasFired)
			{
				hasFired = true;
				Util.PlaySound("RekSeeker", base.gameObject);
				if (base.isAuthority)
				{
					Ray aimRay = GetAimRay();
					ProjectileManager.instance.FireProjectile(Projectiles.seekerPrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageCoefficient * damageStat, 1000f, RollCrit());
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= fireTime)
			{
				Fire();
			}
			if (base.fixedAge >= duration && base.isAuthority)
			{
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
