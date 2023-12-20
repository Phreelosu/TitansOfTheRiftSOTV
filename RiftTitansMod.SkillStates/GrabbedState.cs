using EntityStates;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.SkillStates {

	public class GrabbedState : BaseState
	{
		public Transform handTransform;

		private float duration = 1f;

		private Animator modelAnimator;

		public float freezeDuration = 1f;

		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = GetModelTransform();
			if ((bool)modelTransform)
			{
				CharacterModel component = modelTransform.GetComponent<CharacterModel>();
				if (!component)
				{
				}
			}
			modelAnimator = GetModelAnimator();
			if ((bool)modelAnimator)
			{
				duration = freezeDuration;
			}
			GetModelBaseTransform().parent = handTransform;
			base.transform.parent = handTransform;
		}

		public override void OnExit()
		{
			if ((bool)modelAnimator)
			{
				modelAnimator.enabled = true;
			}
			GetModelBaseTransform().parent = null;
			base.transform.parent = null;
			base.healthComponent.isInFrozenState = false;
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= duration)
			{
				outer.SetInterruptState(new ThrownState(), InterruptPriority.Frozen);
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}
	}
}
