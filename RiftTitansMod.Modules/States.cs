using System;
using System.Collections.Generic;
using RiftTitansMod.SkillStates;
using RiftTitansMod.SkillStates.BaseStates;
using RiftTitansMod.SkillStates.Baron;
using RiftTitansMod.SkillStates.Blue;
using RiftTitansMod.SkillStates.Chicken;
using RiftTitansMod.SkillStates.Herald;
using RiftTitansMod.SkillStates.Reksai;

namespace RiftTitansMod.Modules {

	public static class States
	{
		internal static List<Type> entityStates = new List<Type>();

		internal static void RegisterStates()
		{
			entityStates.Add(typeof(BaseMeleeAttack));
			entityStates.Add(typeof(GrabbedState));
			entityStates.Add(typeof(SkillStates.Baron.DeathState));
			entityStates.Add(typeof(PlaceTentacles));
			entityStates.Add(typeof(SkillStates.Baron.SpawnState));
			entityStates.Add(typeof(Spit));
			entityStates.Add(typeof(SpitCone));
			entityStates.Add(typeof(SkillStates.Blue.DeathState));
			entityStates.Add(typeof(Slam));
			entityStates.Add(typeof(SkillStates.Blue.SpawnState));
			entityStates.Add(typeof(SkillStates.Chicken.DeathState));
			entityStates.Add(typeof(Shoot));
			entityStates.Add(typeof(SkillStates.Chicken.SpawnState));
			entityStates.Add(typeof(Attack));
			entityStates.Add(typeof(Charge));
			entityStates.Add(typeof(ChargeCrash));
			entityStates.Add(typeof(SkillStates.Herald.DeathState));
			entityStates.Add(typeof(SkillStates.Herald.SpawnState));
			entityStates.Add(typeof(Sweep));
			entityStates.Add(typeof(SkillStates.Reksai.DeathState));
			entityStates.Add(typeof(FireSeeker));
			entityStates.Add(typeof(Punch));
			entityStates.Add(typeof(SkillStates.Reksai.SpawnState));
			entityStates.Add(typeof(Special));
			entityStates.Add(typeof(SpecialOut));
			entityStates.Add(typeof(Unburrow));
		}
	}
}
