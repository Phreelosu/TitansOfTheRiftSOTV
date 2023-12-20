using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Baron {

	public class Spit : BaseSkillState
	{
		public static float damageCoefficient = 1f;

		public static float force = 200f;

		private bool hasFired;

		private float duration;

		private float fireTime = 0.3f;

		private Transform muzzleTransform;

		public static float minSpread = 0f;

		public static float maxSpread = 5f;

		private bool a;

		public static float baseDuration = 1.5f;

		public static GameObject chargeVfxPrefab;

		private GameObject chargeVfxInstance;

		public override void OnEnter()
		{
			base.OnEnter();
			duration = baseDuration / attackSpeedStat;
			fireTime = duration * 0.3f;
			StartAimMode();
			GetModelAnimator();
			PlayCrossfade("Body", "Spit", "Slash.playbackRate", 0.8f, 0.05f);
			Transform modelTransform = GetModelTransform();
			if ((bool)modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild("Mouth");
				}
			}
		}

		private void Fire()
		{
			if (hasFired)
			{
				return;
			}
			hasFired = true;
			Ray aimRay = GetAimRay();
			EffectManager.SimpleMuzzleFlash(Assets.baronSpitEffect, base.gameObject, "Mouth", transmit: false);
			float num = 50f;
			Ray ray = aimRay;
			ray.origin = aimRay.GetPoint(6f);
			if (Util.CharacterRaycast(base.gameObject, ray, out var hitInfo, float.PositiveInfinity, (int)LayerIndex.world.mask | (int)LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
			{
				float num2 = num;
				Vector3 vector = hitInfo.point - aimRay.origin;
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				float magnitude = vector2.magnitude;
				float num3 = Trajectory.CalculateInitialYSpeed(magnitude / num2, vector.y);
				if (num3 >= 65f)
				{
					num3 = 65f;
				}
				Vector3 vector3 = new Vector3(vector2.x / magnitude * num2, num3, vector2.y / magnitude * num2);
				num = vector3.magnitude;
				aimRay.direction = vector3 / num;
			}
			if (base.isAuthority)
			{
				Vector3 forward = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f);
				ProjectileManager.instance.FireProjectile(Projectiles.baronSpitPrefab, muzzleTransform.position, Util.QuaternionSafeLookRotation(forward), base.gameObject, damageStat * damageCoefficient, force, RollCrit(), DamageColorIndex.Default, null, num);
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
			if (base.fixedAge >= fireTime - 0.12f && !a)
			{
				a = true;
				Util.PlaySound("BaronSpit", base.gameObject);
			}
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
