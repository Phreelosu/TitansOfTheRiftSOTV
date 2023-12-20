using JetBrains.Annotations;
using RiftTitansMod.Modules.Components.Reksai;
using RoR2;
using RoR2.Skills;

namespace RiftTitansMod.Modules.Components {

	public class ReksaiBurrowedSkillDef : SkillDef
	{
		protected class InstanceData : BaseSkillInstanceData
		{
			public ReksaiBurrowController RiftTitansTracker;
		}

		public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new InstanceData
			{
				RiftTitansTracker = skillSlot.GetComponent<ReksaiBurrowController>()
			};
		}

		private static bool IsBurrowed([NotNull] GenericSkill skillSlot)
		{
			ReksaiBurrowController riftTitansTracker = ((InstanceData)skillSlot.skillInstanceData).RiftTitansTracker;
			return riftTitansTracker != null && riftTitansTracker.burrowed;
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return IsBurrowed(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && IsBurrowed(skillSlot);
		}
	}
}
