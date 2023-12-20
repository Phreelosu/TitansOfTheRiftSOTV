using EntityStates;
using EntityStates.LemurianMonster;
using RiftTitansMod.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Baron {

	public class SpitCone : BaseSkillState
	{
		public static float damageCoefficient = 1f;

		public static float numProjectiles = 2f;

		public static float force = 200f;

		private bool hasFired;

		private float duration;

		private float fireTime = 0.8f;

		private float fireDuration = 0.9f;

		private float fireInterval = 0.1f;

		private float fireStopwatch;

		private Transform muzzleTransform;

		public static float minSpread = 0f;

		public static float maxSpread = 30f;

		public static float baseDuration = 3f;

		public static GameObject chargeVfxPrefab;

		private GameObject chargeVfxInstance;

		public override void OnEnter()
		{
			base.OnEnter();
			fireInterval /= attackSpeedStat;
			duration = baseDuration;
			StartAimMode();
			GetModelAnimator();
			PlayCrossfade("Body", "ConeWindup", "Slash.playbackRate", 0.8f, 0.05f);
			Transform modelTransform = GetModelTransform();
			Util.PlayAttackSpeedSound(ChargeFireball.attackString, base.gameObject, attackSpeedStat);
			if ((bool)modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild("Mouth");
					chargeVfxInstance = Object.Instantiate(Assets.baronChargeEffect, muzzleTransform.position, muzzleTransform.rotation);
					chargeVfxInstance.transform.parent = muzzleTransform;
				}
			}
		}

		private void Fire()
		{
			if (!hasFired)
			{
				hasFired = true;
				EffectManager.SimpleMuzzleFlash(Assets.baronSpitConeEffect, base.gameObject, "Mouth", transmit: false);
				Util.PlaySound("BaronSpitCone", base.gameObject);
			}
			Ray aimRay = GetAimRay();
			float num = 40f;
			Ray ray = aimRay;
			ray.origin = aimRay.GetPoint(6f);
			if (!base.isAuthority)
			{
				return;
			}
			for (int i = 0; (float)i < numProjectiles; i++)
			{
				float value = Random.value;
				float num2 = (0.6f + value * 0.5f) * num;
				if (Util.CharacterRaycast(base.gameObject, ray, out var hitInfo, float.PositiveInfinity, (int)LayerIndex.world.mask | (int)LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
				{
					float num3 = num2;
					Vector3 vector = hitInfo.point - aimRay.origin;
					Vector2 vector2 = new Vector2(vector.x, vector.z);
					float magnitude = vector2.magnitude;
					float num4 = Trajectory.CalculateInitialYSpeed(magnitude / num3, vector.y);
					if (num4 >= 80f)
					{
						num4 = 80f;
					}
					Vector3 vector3 = new Vector3(vector2.x / magnitude * num3, num4, vector2.y / magnitude * num3);
					num2 = vector3.magnitude;
					aimRay.direction = vector3 / num2;
				}
				Vector3 forward = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0.1f, 0.1f);
				ProjectileManager.instance.FireProjectile(Projectiles.baronSpitPrefab, muzzleTransform.position, Util.QuaternionSafeLookRotation(forward), base.gameObject, damageStat * damageCoefficient, force, RollCrit(), DamageColorIndex.Default, null, num2);
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
				if ((bool)chargeVfxInstance)
				{
					EntityState.Destroy(chargeVfxInstance);
				}
				if (base.fixedAge < fireDuration + fireTime)
				{
					fireStopwatch += Time.fixedDeltaTime;
				}
				if (fireStopwatch >= fireInterval)
				{
					fireStopwatch = 0f;
					Fire();
				}
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
