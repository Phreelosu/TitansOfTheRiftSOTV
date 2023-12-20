using RoR2;
using UnityEngine;

namespace EntityStates {

	public class ThrownState : BaseState
	{
		public Transform[] bones;

		private RagdollController ragdoll;

		public Vector3 force = 200f * Vector3.up;

		private bool noRagdoll;

		private float duration = 1.5f;

		private Animator modelAnimator;

		private TemporaryOverlay temporaryOverlay;

		public float freezeDuration = 0.35f;

		public static GameObject frozenEffectPrefab;

		public static GameObject executeEffectPrefab;

		public override void OnEnter()
		{
			base.OnEnter();
			modelAnimator = GetModelAnimator();
			ragdoll = base.modelLocator.modelTransform.GetComponent<RagdollController>();
			if (!ragdoll)
			{
				base.characterMotor.ApplyForce(force);
				outer.SetNextStateToMain();
				noRagdoll = true;
				return;
			}
			bones = ragdoll.bones;
			if ((bool)modelAnimator)
			{
				modelAnimator.enabled = false;
			}
			Transform[] array = bones;
			foreach (Transform transform in array)
			{
				if (transform.gameObject.layer == LayerIndex.ragdoll.intVal)
				{
					transform.parent = base.transform;
					Rigidbody component = transform.GetComponent<Rigidbody>();
					transform.GetComponent<Collider>().enabled = true;
					component.isKinematic = false;
					component.interpolation = RigidbodyInterpolation.Interpolate;
					component.collisionDetectionMode = CollisionDetectionMode.Continuous;
					component.AddForce(force * Random.Range(0.9f, 1.2f), ForceMode.VelocityChange);
				}
			}
			if ((bool)(Object)(object)base.characterDirection)
			{
				base.characterDirection.moveVector = base.characterDirection.forward;
			}
		}

		public override void OnExit()
		{
			if (!noRagdoll)
			{
				Transform[] array = bones;
				foreach (Transform transform in array)
				{
					if (transform.gameObject.layer == LayerIndex.ragdoll.intVal)
					{
						transform.parent = base.transform;
						Rigidbody component = transform.GetComponent<Rigidbody>();
						transform.GetComponent<Collider>().enabled = false;
						component.isKinematic = false;
						component.interpolation = RigidbodyInterpolation.Interpolate;
						component.collisionDetectionMode = CollisionDetectionMode.Continuous;
					}
				}
				if ((bool)modelAnimator)
				{
					modelAnimator.enabled = true;
				}
				if (!temporaryOverlay)
				{
				}
			}
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= duration)
			{
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}
	}
}
