using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Herald {

	public class SpawnState : BaseSkillState
	{
		private bool a;

		protected float sound = 0.9f;

		private bool b;

		protected float effect = 0.4f;

		protected GameObject spawnEffect = Assets.heraldCrashEffect;

		public static float duration = 3f;

		public static string spawnSoundString;

		public override void OnEnter()
		{
			base.OnEnter();
			PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= sound && !a)
			{
				a = true;
				Util.PlaySound("HeraldSpawn", base.gameObject);
			}
			if (base.fixedAge >= effect && !b)
			{
				b = true;
				Util.PlaySound("RekUnburrow", base.gameObject);
				EffectManager.SimpleEffect(Assets.heraldCrashEffect, base.transform.position, Quaternion.identity, transmit: true);
			}
			if (base.fixedAge >= duration && base.isAuthority)
			{
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}
