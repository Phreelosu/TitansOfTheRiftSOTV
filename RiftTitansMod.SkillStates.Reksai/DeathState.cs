using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Reksai {

	public class DeathState : GenericCharacterDeath
	{
		private float duration = 4f;

		public static GameObject initialEffect;

		public static float initialEffectScale;

		public static float velocityMagnitude;

		public static float explosionForce;

		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = GetModelTransform();
			PlayAnimation("Body", "Death", "Spawn.playbackRate", duration);
			PlayAnimation("Body, Burrowed", "Death", "Spawn.playbackRate", duration);
			Util.PlaySound("RekDeath", base.gameObject);
		}

		public override void PlayDeathSound()
		{
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > 0.5f)
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
