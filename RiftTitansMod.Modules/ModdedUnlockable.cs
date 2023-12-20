using System;
using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RiftTitansMod.Modules {

	internal abstract class ModdedUnlockable : BaseAchievement, IModdedUnlockableDataProvider
	{
		public abstract string AchievementIdentifier { get; }

		public abstract string UnlockableIdentifier { get; }

		public abstract string AchievementNameToken { get; }

		public abstract string PrerequisiteUnlockableIdentifier { get; }

		public abstract string UnlockableNameToken { get; }

		public abstract string AchievementDescToken { get; }

		public abstract Sprite Sprite { get; }

		public abstract Func<string> GetHowToUnlock { get; }

		public abstract Func<string> GetUnlocked { get; }

        public override bool wantsBodyCallbacks => base.wantsBodyCallbacks;

        public void Revoke()
		{
			if (base.userProfile.HasAchievement(AchievementIdentifier))
			{
				base.userProfile.RevokeAchievement(AchievementIdentifier);
			}
			base.userProfile.RevokeUnlockable(UnlockableCatalog.GetUnlockableDef(UnlockableIdentifier));
		}

		public override void OnGranted()
		{
			base.OnGranted();
		}

		public override void OnInstall()
		{
			base.OnInstall();
		}

		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		public override float ProgressForAchievement()
		{
			return base.ProgressForAchievement();
		}

		public override BodyIndex LookUpRequiredBodyIndex()
		{
			return base.LookUpRequiredBodyIndex();
		}

		public override void OnBodyRequirementBroken()
		{
			base.OnBodyRequirementBroken();
		}

		public override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
		}
	}
}
