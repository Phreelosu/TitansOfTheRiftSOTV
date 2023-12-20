using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Baron {

	public class SpawnState : BaseSkillState
	{
		private Transform muzzleTransform;

		public GameObject yellEffect;

		private bool a;

		private bool b;

		public float yellEffectTime = 0.8f;

		public float yellEndTime = 2.3f;

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
			PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
			Util.PlaySound("RekUnburrow", base.gameObject);
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
				yell = true;
			}
			if (!a && base.fixedAge >= yellEffectTime)
			{
				yellEffect = Object.Instantiate(Assets.baronYellEffect, muzzleTransform);
				Util.PlaySound("BaronSpawn", base.gameObject);
				yellEffect.AddComponent<DestroyOnParticleEnd>();
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
