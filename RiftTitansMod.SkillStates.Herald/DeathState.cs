using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Herald {

	public class DeathState : GenericCharacterDeath
	{
		private bool a;

		private float s = 0.7f;

		public static GameObject initialEffect;

		public static float initialEffectScale;

		public static float velocityMagnitude;

		public static float explosionForce;

		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = GetModelTransform();
			PlayAnimation("Death", "Death", "Spawn.playbackRate", SpawnState.duration);
			Util.PlaySound("HeraldDeathStart", base.gameObject);
		}

		public override void PlayDeathSound()
		{
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!a && base.fixedAge >= s)
			{
				a = true;
				Util.PlaySound("HeraldDeath", base.gameObject);
			}
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
