using EntityStates;
using RoR2;

namespace RiftTitansMod.SkillStates.Chicken {

	public class SpawnState : BaseSkillState
	{
		public static float duration = 3f;

		public static string spawnSoundString;

		public override void OnEnter()
		{
			base.OnEnter();
			PlayAnimation("Body", "Spawn", "Spawn.playbackRate", duration);
			Util.PlaySound("ChickenSpawn", base.gameObject);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
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
