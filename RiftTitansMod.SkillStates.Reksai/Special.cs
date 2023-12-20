using EntityStates;
using RiftTitansMod.Modules;
using RiftTitansMod.Modules.Components.Reksai;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Reksai {

	public class Special : BaseSkillState
	{
		public static float damageCoefficient = 16f;

		public static float procCoefficient = 1f;

		public static float baseDuration = 6f;

		public static float throwForce = 80f;

		public static float speedCoefficient = 4f;

		public static float forceExitDistance = 35f;

		public static float effectRadius = 10f;

		private ReksaiSpecialTracker tracker;

		private float duration;

		private float burrowTime;

		private bool playSound;

		private Animator animator;

		private HurtBox target;

		private Vector3 previousTargetPosition;

		private bool targetIsValid;

		private bool targetIsLost;

		private Transform muzzleTransform;

		private bool burrowEffect;

		private bool b;

		private float y = 0.18f;

		private float du = 1.1f;

		public override void OnEnter()
		{
			base.OnEnter();
			if (!GetComponent<ReksaiBurrowController>().burrowed)
			{
				outer.SetNextStateToMain();
				base.skillLocator.special.AddOneStock();
				return;
			}
			duration = baseDuration;
			burrowTime = 0.35f * duration;
			animator = GetModelAnimator();
			tracker = GetComponent<ReksaiSpecialTracker>();
			target = tracker.GetTrackingTarget();
			base.characterBody.SetAimTimer(2f);
			StartAimMode();
			if ((bool)GetComponent<ReksaiBurrowController>())
			{
				GetComponent<ReksaiBurrowController>().SetBurrowFalse();
			}
			if ((bool)(Object)(object)base.characterBody)
			{
				base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
			}
			if ((bool)target && (bool)(Object)(object)target.healthComponent && target.healthComponent.alive)
			{
				targetIsValid = true;
				base.characterDirection.forward = target.transform.position - base.transform.position;
			}
			if ((bool)base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild("Mouth");
				}
			}
			animator.SetLayerWeight(animator.GetLayerIndex("Body, Burrowed"), 0f);
			Util.PlaySound("RekUltVoice", base.gameObject);
			PlayAnimation("Body", "Special", "ThrowBomb.playbackRate", duration);
		}

		public override void OnExit()
		{
			base.OnExit();
			base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!targetIsValid)
			{
				outer.SetNextStateToMain();
				return;
			}
			if (base.fixedAge >= y && !b)
			{
				b = true;
				GameObject gameObject = Object.Instantiate(Assets.reksaiYellEffect, muzzleTransform);
				gameObject.AddComponent<DestroyOnParticleEnd>();
				ShakeEmitter shakeEmitter = gameObject.AddComponent<ShakeEmitter>();
				shakeEmitter.duration = du;
				shakeEmitter.radius = 200f;
				shakeEmitter.wave.amplitude = 1.3f;
				shakeEmitter.wave.frequency = 360f;
				ParticleSystem.MainModule main = gameObject.gameObject.transform.Find("Breath").gameObject.GetComponent<ParticleSystem>().main;
				main.duration = du;
				main.startDelay = 0f;
				main = gameObject.gameObject.transform.Find("Spit").gameObject.GetComponent<ParticleSystem>().main;
				main.duration = du;
				main.startDelay = 0f;
				main = gameObject.gameObject.transform.Find("SoundWaves").gameObject.GetComponent<ParticleSystem>().main;
				main.duration = du;
				main.startDelay = 0f;
			}
			if (!target || !(Object)(object)target.healthComponent || !target.healthComponent.alive)
			{
				targetIsLost = true;
			}
			Vector3 vector = (targetIsLost ? previousTargetPosition : target.transform.position);
			Vector3 vector2 = vector - base.transform.position;
			if (base.fixedAge >= burrowTime * 0.7f && !playSound)
			{
				Util.PlaySound("RekBurrow", base.gameObject);
				playSound = true;
			}
			if (base.fixedAge >= burrowTime)
			{
				if (!burrowEffect)
				{
					EffectManager.SimpleEffect(Assets.reksaiBurrowEffect, base.transform.position, Quaternion.identity, transmit: true);
					burrowEffect = true;
				}
				Vector3 normalized = vector2.normalized;
				normalized.y = 0f;
				base.characterMotor.rootMotion += moveSpeedStat * normalized * speedCoefficient * Time.fixedDeltaTime;
			}
			if (vector2.magnitude <= forceExitDistance && base.fixedAge >= burrowTime && base.isAuthority)
			{
				if (targetIsValid && !targetIsLost)
				{
					outer.SetNextState(new SpecialOut
					{
						target = target
					});
				}
				else if (targetIsLost)
				{
					outer.SetNextState(new SpecialOut
					{
						targetPosition = previousTargetPosition
					});
				}
			}
			else if (base.fixedAge >= duration && base.isAuthority)
			{
				outer.SetNextState(new SpecialOut());
			}
			else if (!targetIsLost)
			{
				previousTargetPosition = target.transform.position;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
