using EntityStates.BeetleGuardMonster;
using RiftTitansMod.Modules.Survivors;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.Modules.Components.Reksai {

	public class ReksaiBurrowController : MonoBehaviour
	{
		private CharacterBody characterBody;

		private ModelLocator model;

		private ChildLocator childLocator;

		private Animator modelAnimator;

		public bool burrowed;

		private float spawnWaitTime = 8f;

		private float lifetime;

		private string baseFootstepString;

		public static float burrowEffectTime = 1f;

		private float stopwatch;

		private bool burrowEffect;

		private void Awake()
		{
			characterBody = base.gameObject.GetComponent<CharacterBody>();
			childLocator = base.gameObject.GetComponentInChildren<ChildLocator>();
			model = base.gameObject.GetComponent<ModelLocator>();
			modelAnimator = base.gameObject.GetComponentInChildren<Animator>();
			baseFootstepString = model.modelTransform.gameObject.GetComponent<FootstepHandler>().baseFootstepString;
		}

		private void FixedUpdate()
		{
			model.normalizeToFloor = true;
			if (lifetime < spawnWaitTime)
			{
				lifetime += Time.fixedDeltaTime;
			}
			else
			{
				if (characterBody.outOfCombat && characterBody.outOfDanger && !burrowed)
				{
					model.modelTransform.gameObject.GetComponent<FootstepHandler>().baseFootstepString = "";
					modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("Body, Burrowed"), 1f);
					modelAnimator.PlayInFixedTime("Burrow", modelAnimator.GetLayerIndex("Body, Burrowed"));
					Util.PlaySound("RekBurrow", base.gameObject);
					SfxLocator component = ((Component)(object)characterBody).gameObject.GetComponent<SfxLocator>();
					component.barkSound = "RekBurrowIdle";
					component.aliveLoopStart = "RekIdleLoop";
					SkillLocator component2 = ((Component)(object)characterBody).gameObject.GetComponent<SkillLocator>();
					if ((bool)(Object)(object)component2)
					{
						component2.primary.SetSkillOverride(component2.primary, RiftTitansMod.Modules.Survivors.Reksai.unburrowSkillDef, GenericSkill.SkillOverridePriority.Contextual);
					}
					burrowed = true;
				}
				if (burrowed)
				{
					stopwatch += Time.fixedDeltaTime;
					if (!burrowEffect && stopwatch >= burrowEffectTime)
					{
						EffectManager.SimpleMuzzleFlash(GroundSlam.slamEffectPrefab, base.gameObject, "BurrowCenter", transmit: true);
						burrowEffect = true;
						stopwatch = 0f;
					}
				}
				else
				{
					burrowEffect = false;
				}
			}
			if (!burrowed)
			{
				SfxLocator component3 = ((Component)(object)characterBody).gameObject.GetComponent<SfxLocator>();
				component3.barkSound = "RekIdle";
				component3.aliveLoopStart = "";
			}
		}

		public void Unburrow()
		{
			model.modelTransform.gameObject.GetComponent<FootstepHandler>().baseFootstepString = baseFootstepString;
			modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("Body, Burrowed"), 0f);
			if (burrowed)
			{
				SkillLocator component = ((Component)(object)characterBody).gameObject.GetComponent<SkillLocator>();
				if ((bool)(Object)(object)component)
				{
					component.primary.UnsetSkillOverride(component.primary, RiftTitansMod.Modules.Survivors.Reksai.unburrowSkillDef, GenericSkill.SkillOverridePriority.Contextual);
					component.primary.stock = 0;
				}
			}
			burrowed = false;
		}

		public void SetBurrowFalse()
		{
			model.modelTransform.gameObject.GetComponent<FootstepHandler>().baseFootstepString = baseFootstepString;
			modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("Body, Burrowed"), 0f);
			SkillLocator component = ((Component)(object)characterBody).gameObject.GetComponent<SkillLocator>();
			if ((bool)(Object)(object)component)
			{
				component.primary.UnsetSkillOverride(component.primary, RiftTitansMod.Modules.Survivors.Reksai.unburrowSkillDef, GenericSkill.SkillOverridePriority.Contextual);
				component.primary.stock = 0;
			}
			burrowed = false;
		}
	}
}
