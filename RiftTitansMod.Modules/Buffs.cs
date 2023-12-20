using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.Modules {

	public static class Buffs
	{
		internal static List<BuffDef> buffDefs = new List<BuffDef>();

		internal static void RegisterBuffs()
		{
		}

		internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
		{
			BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
			buffDef.name = buffName;
			buffDef.buffColor = buffColor;
			buffDef.canStack = canStack;
			buffDef.isDebuff = isDebuff;
			buffDef.eliteDef = null;
			buffDef.iconSprite = buffIcon;
			buffDefs.Add(buffDef);
			return buffDef;
		}
	}
}
