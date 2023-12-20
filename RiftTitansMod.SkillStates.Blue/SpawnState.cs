using EntityStates;
using RiftTitansMod.Modules;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates.Blue {

	public class SpawnState : BaseSkillState
	{
		private bool b;

		protected float effect = 1f;

		protected GameObject spawnEffect = Assets.blueSlamEffect;

		public static float duration = 5f;

		public static string spawnSoundString;

		public override void OnEnter()
		{
			base.OnEnter();
			PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
			Util.PlaySound("BlueSpawn", base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= effect && !b)
			{
				b = true;
				if ((bool)spawnEffect)
				{
					EffectManager.SimpleEffect(spawnEffect, base.transform.position, Quaternion.identity, transmit: true);
				}
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
