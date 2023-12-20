using System;
using System.Collections.Generic;
using System.Reflection;
using IL.RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RiftTitansMod.Modules {

	internal static class Unlockables
	{
		internal struct UnlockableInfo
		{
			internal string Name;

			internal Func<string> HowToUnlockString;

			internal Func<string> UnlockedString;

			internal int SortScore;
		}

		private static readonly HashSet<string> usedRewardIds = new HashSet<string>();

		internal static List<RoR2.AchievementDef> achievementDefs = new List<RoR2.AchievementDef>();

		internal static List<RoR2.UnlockableDef> unlockableDefs = new List<RoR2.UnlockableDef>();

		private static readonly List<(RoR2.AchievementDef achDef, RoR2.UnlockableDef unlockableDef, string unlockableName)> moddedUnlocks = new List<(RoR2.AchievementDef, RoR2.UnlockableDef, string)>();

		private static bool addingUnlockables;

		public static bool ableToAdd { get; private set; } = false;


		internal static RoR2.UnlockableDef CreateNewUnlockable(UnlockableInfo unlockableInfo)
		{
			RoR2.UnlockableDef unlockableDef = ScriptableObject.CreateInstance<RoR2.UnlockableDef>();
			unlockableDef.nameToken = unlockableInfo.Name;
			unlockableDef.cachedName = unlockableInfo.Name;
			unlockableDef.getHowToUnlockString = unlockableInfo.HowToUnlockString;
			unlockableDef.getUnlockedString = unlockableInfo.UnlockedString;
			unlockableDef.sortScore = unlockableInfo.SortScore;
			return unlockableDef;
		}

		public static RoR2.UnlockableDef AddUnlockable<TUnlockable>(bool serverTracked) where TUnlockable : BaseAchievement, IModdedUnlockableDataProvider, new()
		{
			TUnlockable val = new TUnlockable();
			string unlockableIdentifier = val.UnlockableIdentifier;
			if (!usedRewardIds.Add(unlockableIdentifier))
			{
				throw new InvalidOperationException("The unlockable identifier '" + unlockableIdentifier + "' is already used by another mod or the base game.");
			}
			RoR2.AchievementDef achievementDef = new RoR2.AchievementDef
			{
				identifier = val.AchievementIdentifier,
				unlockableRewardIdentifier = val.UnlockableIdentifier,
				prerequisiteAchievementIdentifier = val.PrerequisiteUnlockableIdentifier,
				nameToken = val.AchievementNameToken,
				descriptionToken = val.AchievementDescToken,
				achievedIcon = val.Sprite,
				type = val.GetType(),
				serverTrackerType = (serverTracked ? val.GetType() : null)
			};
			UnlockableInfo unlockableInfo = default(UnlockableInfo);
			unlockableInfo.Name = val.UnlockableIdentifier;
			unlockableInfo.HowToUnlockString = val.GetHowToUnlock;
			unlockableInfo.UnlockedString = val.GetUnlocked;
			unlockableInfo.SortScore = 200;
			RoR2.UnlockableDef unlockableDef = CreateNewUnlockable(unlockableInfo);
			unlockableDefs.Add(unlockableDef);
			achievementDefs.Add(achievementDef);
			moddedUnlocks.Add((achievementDef, unlockableDef, val.UnlockableIdentifier));
			if (!addingUnlockables)
			{
				addingUnlockables = true;
				IL.RoR2.AchievementManager.CollectAchievementDefs += new ILContext.Manipulator(CollectAchievementDefs);
				IL.RoR2.UnlockableCatalog.Init += new ILContext.Manipulator(Init_Il);
			}
			return unlockableDef;
		}

		public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target, out int index) where TDelegate : Delegate
		{
			index = cursor.EmitDelegate<TDelegate>(target);
			return cursor;
		}

		public static ILCursor CallDel_<TDelegate>(this ILCursor cursor, TDelegate target) where TDelegate : Delegate
		{
			int index;
			return cursor.CallDel_(target, out index);
		}

		private static void Init_Il(ILContext il)
		{
			new ILCursor(il).GotoNext((MoveType)1, new Func<Instruction, bool>[1]
			{
			(Instruction x) => ILPatternMatchingExt.MatchCallOrCallvirt(x, typeof(RoR2.UnlockableCatalog), "SetUnlockableDefs")
			}).CallDel_(ArrayHelper.AppendDel(unlockableDefs));
		}

		private static void CollectAchievementDefs(ILContext il)
		{
			FieldInfo field = typeof(RoR2.AchievementManager).GetField("achievementIdentifiers", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if ((object)field == null)
			{
				throw new NullReferenceException("Could not find field in AchievementManager");
			}
			ILCursor val = new ILCursor(il);
			val.GotoNext((MoveType)2, new Func<Instruction, bool>[2]
			{
			(Instruction x) => ILPatternMatchingExt.MatchEndfinally(x),
			(Instruction x) => ILPatternMatchingExt.MatchLdloc(x, 1)
			});
			val.Emit(OpCodes.Ldarg_0);
			val.Emit(OpCodes.Ldsfld, field);
			val.EmitDelegate<Action<List<RoR2.AchievementDef>, Dictionary<string, RoR2.AchievementDef>, List<string>>>((Action<List<RoR2.AchievementDef>, Dictionary<string, RoR2.AchievementDef>, List<string>>)EmittedDelegate);
			val.Emit(OpCodes.Ldloc_1);
			static void EmittedDelegate(List<RoR2.AchievementDef> list, Dictionary<string, RoR2.AchievementDef> map, List<string> identifiers)
			{
				ableToAdd = false;
				for (int i = 0; i < moddedUnlocks.Count; i++)
				{
					var (achievementDef, unlockableDef, text) = moddedUnlocks[i];
					if (achievementDef != null)
					{
						identifiers.Add(achievementDef.identifier);
						list.Add(achievementDef);
						map.Add(achievementDef.identifier, achievementDef);
					}
				}
			}
		}
	}
}
