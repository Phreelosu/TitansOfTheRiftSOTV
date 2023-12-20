using System.Collections.Generic;
using System.Linq;
using EntityStates;
using EntityStates.LemurianMonster;
using RiftTitansMod.Modules;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Baron {

	public class PlaceTentacles : BaseSkillState
	{
		public static float damageCoefficient = 1f;

		public static float tentaclesPerSecond = 20f;

		public static float placementDuration = 3f;

		public static float radius = 125f;

		public static float radiusAroundPlayer = 18f;

		public static float tentaclesPerPlayerPerSecond = 2f;

		public static float force = 200f;

		private bool hasFired;

		private float duration;

		private float fireTime = 0.5f;

		private float fireDuration;

		private float fireInterval;

		private float playerFireInterval;

		private float fireStopwatch;

		private float playerFireStopwatch;

		private int index;

		private List<HurtBox> enemies;

		private Transform muzzleTransform;

		public static float minSpread = 0f;

		public static float maxSpread = 30f;

		private bool b;

		private GameObject yellEffect;

		public static float baseDuration = 7f;

		public static GameObject chargeVfxPrefab;

		private GameObject chargeVfxInstance;

		public override void OnEnter()
		{
			base.OnEnter();
			GetModelAnimator();
			PlayCrossfade("Body", "TentacleWindup", "Slash.playbackRate", 0.8f, 0.05f);
			enemies = new List<HurtBox>();
			AimAnimator component = base.modelLocator.modelTransform.GetComponent<AimAnimator>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			if (NetworkServer.active)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				if ((bool)(Object)(object)base.teamComponent)
				{
					bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
				}
				bullseyeSearch.maxDistanceFilter = radius * 2f;
				bullseyeSearch.maxAngleFilter = 360f;
				Ray aimRay = GetAimRay();
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.None;
				bullseyeSearch.RefreshCandidates();
				enemies = bullseyeSearch.GetResults().ToList();
			}
			duration = baseDuration + fireTime;
			fireDuration = placementDuration;
			fireInterval = 1f / tentaclesPerSecond;
			playerFireInterval = 1f / tentaclesPerPlayerPerSecond;
			Transform modelTransform = GetModelTransform();
			Util.PlayAttackSpeedSound(ChargeFireball.attackString, base.gameObject, attackSpeedStat);
			chargeVfxInstance = Object.Instantiate(Assets.baronGroundChargeEffect, base.transform.position, base.transform.rotation);
			chargeVfxInstance.transform.parent = muzzleTransform;
		}

		private void Fire()
		{
			if (base.isAuthority)
			{
				Vector2 insideUnitCircle = Random.insideUnitCircle;
				Vector3 vector = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
				vector *= radius;
				vector += base.transform.position;
				if (Physics.Raycast(new Ray(vector + Vector3.up * 1f, Vector3.down), out var hitInfo, 200f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					vector = hitInfo.point;
				}
				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.projectilePrefab = Projectiles.baronTentaclePrefab;
				fireProjectileInfo.position = vector;
				fireProjectileInfo.rotation = Quaternion.identity;
				fireProjectileInfo.owner = base.gameObject;
				fireProjectileInfo.damage = damageStat * damageCoefficient;
				fireProjectileInfo.force = force;
				fireProjectileInfo.crit = base.characterBody.RollCrit();
				fireProjectileInfo.fuseOverride = placementDuration;
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

		private void FireOnPlayer(HurtBox h)
		{
			if (base.isAuthority)
			{
				Vector2 insideUnitCircle = Random.insideUnitCircle;
				Vector3 vector = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
				vector *= radiusAroundPlayer;
				vector += h.transform.position;
				if (Physics.Raycast(new Ray(vector + Vector3.up * 4f, Vector3.down), out var hitInfo, 200f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
				{
					vector = hitInfo.point;
					FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
					fireProjectileInfo.projectilePrefab = Projectiles.baronTentaclePrefab;
					fireProjectileInfo.position = vector;
					fireProjectileInfo.rotation = Quaternion.identity;
					fireProjectileInfo.owner = base.gameObject;
					fireProjectileInfo.damage = damageStat * damageCoefficient;
					fireProjectileInfo.force = force;
					fireProjectileInfo.crit = base.characterBody.RollCrit();
					fireProjectileInfo.fuseOverride = placementDuration;
					ProjectileManager.instance.FireProjectile(fireProjectileInfo);
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			AimAnimator component = base.modelLocator.modelTransform.GetComponent<AimAnimator>();
			if ((bool)component)
			{
				component.enabled = true;
			}
			if ((bool)yellEffect)
			{
				EntityState.Destroy(yellEffect);
			}
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
				if (base.fixedAge < fireDuration + fireTime)
				{
					fireStopwatch += Time.fixedDeltaTime;
					playerFireStopwatch += Time.fixedDeltaTime;
				}
				if (playerFireStopwatch >= playerFireInterval)
				{
					foreach (HurtBox enemy in enemies)
					{
						if ((bool)(Object)(object)enemy.healthComponent)
						{
							FireOnPlayer(enemy);
						}
					}
					playerFireStopwatch = 0f;
				}
				if (fireStopwatch >= fireInterval)
				{
					fireStopwatch = 0f;
					Fire();
				}
			}
			if (base.fixedAge >= fireTime + placementDuration && !b)
			{
				b = true;
				PlayCrossfade("Body", "Tentacles", "Slash.playbackRate", 1f, 0.05f);
				Util.PlaySound("BaronTentacleYell", base.gameObject);
				Transform modelTransform = GetModelTransform();
				if ((bool)modelTransform)
				{
					ChildLocator component = modelTransform.GetComponent<ChildLocator>();
					if ((bool)component)
					{
						muzzleTransform = component.FindChild("Mouth");
						yellEffect = Object.Instantiate(Assets.baronYellEffect, muzzleTransform.position, muzzleTransform.rotation);
						yellEffect.transform.parent = muzzleTransform;
					}
				}
			}
			if (base.fixedAge >= duration - 0.67f && (bool)yellEffect)
			{
				EntityState.Destroy(yellEffect);
			}
			if (base.fixedAge >= duration && base.isAuthority)
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
