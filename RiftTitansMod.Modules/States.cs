using System;
using System.Collections.Generic;
using RiftTitansMod.SkillStates.BaseStates;

namespace RiftTitansMod.Modules {

	public static class States
	{
		internal static List<Type> entityStates = new List<Type>();

		internal static void RegisterStates()
		{
			entityStates.Add(typeof(BaseMeleeAttack));
		}
	}
}
