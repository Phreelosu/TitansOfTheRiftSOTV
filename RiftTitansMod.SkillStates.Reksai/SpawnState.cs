using System;
using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Reksai {

	public class SpawnState : BaseSkillState
	{
		private Transform muzzleTransform;

		public GameObject yellEffect;

		private bool a;

		private bool b;

		public float yellEffectTime = 1.7f;

		public float yellEndTime = 3f;

		public static float effectRadius = 10f;

		public float duration = 5f;

		public float yellTime = 1f;

		public bool yell;

		public override void OnEnter()
		{
			base.OnEnter();
			duration = 5f;
			if ((bool)base.modelLocator)
			{
				ChildLocator component = base.modelLocator.modelTransform.GetComponent<ChildLocator>();
				if ((bool)component)
				{
					muzzleTransform = component.FindChild("Mouth");
				}
			}
			System.Random random = new System.Random();
			PlayAnimation("Body", "Spawn" + random.Next(1, 4), "Spawn.playbackRate", duration);
			Util.PlaySound("RekUnburrow", base.gameObject);
			Util.PlaySound("RekUnburrowVoice", base.gameObject);
			EffectManager.SpawnEffect(Assets.reksaiUnburrowEffect, new EffectData
			{
				origin = base.transform.position,
				scale = effectRadius
			}, transmit: true);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!yell && base.fixedAge >= yellTime)
			{
				Util.PlaySound("RekSpawnVoice", base.gameObject);
				yell = true;
			}
			if (!a && base.fixedAge >= yellEffectTime)
			{
				yellEffect = UnityEngine.Object.Instantiate(Assets.reksaiYellEffect, muzzleTransform);
				yellEffect.AddComponent<DestroyOnParticleEnd>();
				ShakeEmitter shakeEmitter = yellEffect.AddComponent<ShakeEmitter>();
				shakeEmitter.duration = yellEndTime - yellEffectTime;
				shakeEmitter.radius = 200f;
				shakeEmitter.wave.amplitude = 0.6f;
				shakeEmitter.wave.frequency = 200f;
				ParticleSystem.MainModule main = yellEffect.gameObject.transform.Find("Breath").gameObject.GetComponent<ParticleSystem>().main;
				main.duration = yellEndTime - yellEffectTime;
				main.startDelay = 0f;
				main = yellEffect.gameObject.transform.Find("Spit").gameObject.GetComponent<ParticleSystem>().main;
				main.duration = yellEndTime - yellEffectTime;
				main.startDelay = 0f;
				main = yellEffect.gameObject.transform.Find("SoundWaves").gameObject.GetComponent<ParticleSystem>().main;
				main.duration = yellEndTime - yellEffectTime;
				main.startDelay = 0f;
				a = true;
			}
			if (!b && base.fixedAge >= yellEndTime && (bool)yellEffect)
			{
				b = true;
			}
			if (base.fixedAge >= 6f && base.isAuthority)
			{
				outer.SetNextStateToMain();
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			if ((bool)yellEffect)
			{
				EntityState.Destroy(yellEffect);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}
