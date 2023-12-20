using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Herald {

	public class Charge : BaseSkillState
	{
		public float baseWindupTime = 2.2f;

		private float windupTime;

		private float stopwatch;

		public float turningSpeed = 0.3f;

		public float baseDuration = 1.2f;

		public static float chargeDamageCoefficient = 3f;

		public static float awayForceMagnitude = 3500f;

		public static float upwardForceMagnitude = 2500f;

		public static GameObject impactEffectPrefab;

		public static string impactSoundString;

		public static string startSoundString;

		public static string endSoundString;

		public static GameObject knockbackEffectPrefab;

		public static float knockbackDamageCoefficient;

		public static float knockbackForce;

		public GameObject startEffectPrefab;

		public GameObject endEffectPrefab;

		private float duration;

		private Vector3 targetMoveVector;

		private Vector3 targetMoveVectorVelocity;

		public static float turnSmoothTime = 0.01f;

		public static float turnSpeed = 60f;

		public static float chargeMovementSpeedCoefficient = 4f;

		private string baseFootstepString;

		private OverlapAttack attack;

		private List<HurtBox> victimsStruck = new List<HurtBox>();

		public override void OnEnter()
		{
			base.OnEnter();
			windupTime = baseWindupTime;
			duration = baseDuration;
			base.characterDirection.forward = base.inputBank.aimDirection;
			if ((bool)base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = true;
			}
			if (!startEffectPrefab || (bool)(UnityEngine.Object)(object)base.characterBody)
			{
			}
			Transform modelTransform = GetModelTransform();
			FootstepHandler component = modelTransform.gameObject.GetComponent<FootstepHandler>();
			if ((bool)component)
			{
				baseFootstepString = component.baseFootstepString;
				component.baseFootstepString = "Play_titanboss_step";
			}
			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform2 = GetModelTransform();
			if ((bool)modelTransform2)
			{
				hitBoxGroup = Array.Find(modelTransform2.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge");
			}
			attack = new OverlapAttack();
			attack.attacker = base.gameObject;
			attack.inflictor = base.gameObject;
			attack.teamIndex = GetTeam();
			attack.damage = chargeDamageCoefficient * damageStat;
			attack.hitEffectPrefab = impactEffectPrefab;
			attack.forceVector = Vector3.up * upwardForceMagnitude;
			attack.pushAwayForce = awayForceMagnitude;
			attack.hitBoxGroup = hitBoxGroup;
			attack.isCrit = RollCrit();
			PlayCrossfade("Body", "DashStart", 0.1f);
			Util.PlaySound("HeraldChargeStart", base.gameObject);
		}

		public override void OnExit()
		{
			if ((bool)(UnityEngine.Object)(object)base.characterBody)
			{
				if (outer.destroying || (bool)endEffectPrefab)
				{
				}
				base.characterBody.isSprinting = false;
			}
			Transform modelTransform = GetModelTransform();
			FootstepHandler component = modelTransform.gameObject.GetComponent<FootstepHandler>();
			if ((bool)component)
			{
				component.baseFootstepString = baseFootstepString;
			}
			if ((bool)(UnityEngine.Object)(object)base.characterMotor)
			{
				base.characterMotor.moveDirection /= 6f;
			}
			if ((bool)base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = false;
			}
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!(base.fixedAge > windupTime))
			{
				return;
			}
			stopwatch += Time.fixedDeltaTime;
			if (stopwatch >= duration)
			{
				outer.SetNextState(new ChargeCrash());
				base.PlayAnimation("Body", "DashHit");
			}
			else if (base.isAuthority)
			{
				if ((bool)(UnityEngine.Object)(object)base.characterBody)
				{
					base.characterBody.isSprinting = true;
				}
				targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(targetMoveVector, base.inputBank.aimDirection, ref targetMoveVectorVelocity, turnSmoothTime, turnSpeed), Vector3.up).normalized;
				base.characterDirection.moveVector = targetMoveVector;
				Vector3 forward = base.characterDirection.forward;
				base.characterMotor.moveDirection = forward * chargeMovementSpeedCoefficient;
				attack.damage = damageStat * chargeDamageCoefficient;
				if (attack.Fire(victimsStruck))
				{
					outer.SetNextState(new ChargeCrash());
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}
	}
}
