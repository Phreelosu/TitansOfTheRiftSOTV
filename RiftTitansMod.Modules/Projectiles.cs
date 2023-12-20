using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.Modules {

	internal static class Projectiles
	{
		internal static GameObject baronSpitPrefab;

		internal static GameObject baronTentaclePrefab;

		internal static GameObject seekerPrefab;

		internal static GameObject chickenProjectilePrefab;

		internal static void RegisterProjectiles()
		{
			CreateSeekerProjectile();
			CreateChickenGun();
			CreateBaronTentacle();
			CreateBaronSpit();
			AddProjectile(baronSpitPrefab);
			AddProjectile(baronTentaclePrefab);
			AddProjectile(chickenProjectilePrefab);
			AddProjectile(seekerPrefab);
		}

		internal static void AddProjectile(GameObject projectileToAdd)
		{
			Prefabs.projectilePrefabs.Add(projectileToAdd);
		}

		private static void CreateBaronSpit()
		{
			baronSpitPrefab = CloneProjectilePrefab("BeetleQueenSpit", "BaronSpit");
			ProjectileImpactExplosion component = baronSpitPrefab.GetComponent<ProjectileImpactExplosion>();
			component.lifetimeExpiredSound = Assets.baronSpitImpactSound;
		}

		private static void CreateBaronTentacle()
		{
			baronTentaclePrefab = CloneProjectilePrefab("TitanPreFistProjectile", "BaronPreTentacleProjectile");
			ProjectileImpactExplosion component = baronTentaclePrefab.GetComponent<ProjectileImpactExplosion>();
			component.impactEffect = Assets.baronTentacleEffect;
			component.lifetimeExpiredSound = Assets.baronTentacleSound;
		}

		private static void CreateChickenGun()
		{
			chickenProjectilePrefab = CloneProjectilePrefab("Fireball", "ChickenFireball");
			ProjectileSimple component = chickenProjectilePrefab.GetComponent<ProjectileSimple>();
			component.desiredForwardSpeed = 60f;
			ProjectileSingleTargetImpact component2 = chickenProjectilePrefab.GetComponent<ProjectileSingleTargetImpact>();
			component2.enemyHitSoundString = "ChickenHit";
		}

		private static void CreateSeekerProjectile()
		{
			seekerPrefab = CloneProjectilePrefab("Sunder", "Seeker");
			ProjectileCharacterController component = seekerPrefab.GetComponent<ProjectileCharacterController>();
			component.velocity = 90f;
			CharacterController component2 = seekerPrefab.GetComponent<CharacterController>();
			component2.slopeLimit = 150f;
			component2.stepOffset = 0.01f;
		}

		private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
		{
			projectileImpactExplosion.blastDamageCoefficient = 1f;
			projectileImpactExplosion.blastProcCoefficient = 1f;
			projectileImpactExplosion.blastRadius = 1f;
			projectileImpactExplosion.bonusBlastForce = Vector3.zero;
			projectileImpactExplosion.childrenCount = 0;
			projectileImpactExplosion.childrenDamageCoefficient = 0f;
			projectileImpactExplosion.childrenProjectilePrefab = null;
			projectileImpactExplosion.destroyOnEnemy = false;
			projectileImpactExplosion.destroyOnWorld = false;
			projectileImpactExplosion.explosionSoundString = "";
			projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
			projectileImpactExplosion.fireChildren = false;
			projectileImpactExplosion.impactEffect = null;
			projectileImpactExplosion.lifetime = 0f;
			projectileImpactExplosion.lifetimeAfterImpact = 0f;
			projectileImpactExplosion.lifetimeExpiredSoundString = "";
			projectileImpactExplosion.lifetimeRandomOffset = 0f;
			projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
			projectileImpactExplosion.timerAfterImpact = false;
			projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
		}

		private static GameObject CreateGhostPrefab(string ghostName)
		{
			GameObject gameObject = Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
			if (!(Object)(object)gameObject.GetComponent<NetworkIdentity>())
			{
				gameObject.AddComponent<NetworkIdentity>();
			}
			if (!gameObject.GetComponent<ProjectileGhostController>())
			{
				gameObject.AddComponent<ProjectileGhostController>();
			}
			Assets.ConvertAllRenderersToHopooShader(gameObject);
			return gameObject;
		}

		private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
		{
			return PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName, true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Projectiles.cs", "CloneProjectilePrefab", 124);
		}
	}
}
