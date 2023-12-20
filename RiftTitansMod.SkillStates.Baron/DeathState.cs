using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiftTitansMod.SkillStates.Baron {

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
			Util.PlaySound("BaronDeath", base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && base.fixedAge > 2f)
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
