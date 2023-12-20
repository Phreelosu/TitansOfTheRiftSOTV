using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Blue {

	public class DeathState : GenericCharacterDeath
	{
		public static float dropTime = 0.15f;

		public static GameObject initialEffect;

		public static float initialEffectScale;

		public static float velocityMagnitude;

		public static float explosionForce;

		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = GetModelTransform();
			PlayAnimation("Death", "Death", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound("BlueDeath", base.gameObject);
		}

		public override void PlayDeathSound()
		{
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > 1f)
			{
				DestroyBodyAsapServer();
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}
}
