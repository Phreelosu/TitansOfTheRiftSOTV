using JetBrains.Annotations;
using RiftTitansMod.Modules.Components.Reksai;
using RoR2;
using RoR2.Skills;

namespace RiftTitansMod.Modules.Components {

	public class TrackingSkillDef : SkillDef
	{
		protected class InstanceData : BaseSkillInstanceData
		{
			public ReksaiSpecialTracker henryTracker;
		}

		public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new InstanceData
			{
				henryTracker = skillSlot.GetComponent<ReksaiSpecialTracker>()
			};
		}

		private static bool HasTarget([NotNull] GenericSkill skillSlot)
		{
			ReksaiSpecialTracker henryTracker = ((InstanceData)skillSlot.skillInstanceData).henryTracker;
			return (henryTracker != null) ? henryTracker.GetTrackingTarget() : null;
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return HasTarget(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && HasTarget(skillSlot);
		}
	}
}
