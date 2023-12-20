using System;
using EntityStates;
using EntityStates.LemurianMonster;
using RiftTitansMod.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Chicken {

	public class Shoot : BaseSkillState
	{
		public static float damageCoefficient = 1f;

		public static float force = 200f;

		private bool hasFired;

		private float duration;

		private float fireTime = 0.5f;

		public static float baseDuration = 0.67f;

		public static GameObject chargeVfxPrefab;

		private GameObject chargeVfxInstance;

		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			fireTime = duration * 0.75f;
			StartAimMode(fireTime);
			GetModelAnimator();
			System.Random random = new System.Random();
			PlayCrossfade("Body", "Attack" + random.Next(1, 4), "Slash.playbackRate", 0.8f, 0.05f);
			Transform modelTransform = GetModelTransform();
			Util.PlayAttackSpeedSound(ChargeFireball.attackString, base.gameObject, attackSpeedStat);
			if (!modelTransform)
			{
				return;
			}
			ChildLocator component = modelTransform.GetComponent<ChildLocator>();
			if ((bool)component)
			{
				Transform transform = component.FindChild("Mouth");
				if ((bool)transform && (bool)ChargeFireball.chargeVfxPrefab)
				{
					chargeVfxInstance = UnityEngine.Object.Instantiate(ChargeFireball.chargeVfxPrefab, transform.position, transform.rotation);
					chargeVfxInstance.transform.parent = transform;
				}
			}
		}

		private void Fire()
		{
			if (!hasFired)
			{
				hasFired = true;
				Ray aimRay = GetAimRay();
				Util.PlaySound("ChickenShoot", base.gameObject);
				if ((bool)FireFireball.effectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(FireFireball.effectPrefab, base.gameObject, "Mouth", transmit: false);
				}
				if (base.isAuthority)
				{
					ProjectileManager.instance.FireProjectile(Projectiles.chickenProjectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageStat * damageCoefficient, force, RollCrit());
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			if ((bool)chargeVfxInstance)
			{
				EntityState.Destroy(chargeVfxInstance);
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
			return InterruptPriority.Skill;
		}
	}
}
