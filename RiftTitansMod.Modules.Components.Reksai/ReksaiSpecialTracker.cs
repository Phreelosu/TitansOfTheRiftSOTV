using System.Linq;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace RiftTitansMod.Modules.Components.Reksai {

	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(InputBankTest))]
	[RequireComponent(typeof(TeamComponent))]
	public class ReksaiSpecialTracker : MonoBehaviour
	{
		private float trackingStopwatch;

		private float maxTrackingTime = 4f;

		private ReksaiBurrowController burrowController;

		public float combatDetectTime = 0.5f;

		public float maxTrackingDistance = 160f;

		public float maxTrackingAngle = 360f;

		public float trackerUpdateFrequency = 10f;

		public HurtBox trackingTarget;

		private CharacterBody characterBody;

		private TeamComponent teamComponent;

		private InputBankTest inputBank;

		private float trackerUpdateStopwatch;

		private Indicator indicator;

		private readonly BullseyeSearch search = new BullseyeSearch();

		private void Awake()
		{
			indicator = new Indicator(base.gameObject, Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
		}

		private void Start()
		{
			burrowController = GetComponent<ReksaiBurrowController>();
			characterBody = GetComponent<CharacterBody>();
			inputBank = GetComponent<InputBankTest>();
			teamComponent = GetComponent<TeamComponent>();
		}

		public HurtBox GetTrackingTarget()
		{
			return trackingTarget;
		}

		private void OnEnable()
		{
			indicator.active = true;
		}

		private void OnDisable()
		{
			indicator.active = false;
		}

		private void FixedUpdate()
		{
			trackerUpdateStopwatch += Time.fixedDeltaTime;
			if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
			{
				trackerUpdateStopwatch -= 1f / trackerUpdateFrequency;
				HurtBox hurtBox = trackingTarget;
				Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
				if (!trackingTarget && burrowController.burrowed)
				{
					SearchForTarget(aimRay);
				}
				if ((bool)trackingTarget || burrowController.burrowed)
				{
				}
				indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);
			}
		}

		private void SearchForTarget(Ray aimRay)
		{
			TeamMask enemyTeams = TeamMask.GetEnemyTeams(teamComponent.teamIndex);
			search.teamMaskFilter = enemyTeams;
			search.filterByLoS = false;
			search.searchOrigin = aimRay.origin;
			search.searchDirection = aimRay.direction;
			search.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			search.maxDistanceFilter = maxTrackingDistance;
			search.maxAngleFilter = maxTrackingAngle;
			search.RefreshCandidates();
			foreach (HurtBox result in search.GetResults())
			{
				bool flag = false;
				CharacterBody component = ((Component)(object)result.healthComponent).gameObject.GetComponent<CharacterBody>();
				if (component.isSprinting)
				{
					flag = true;
				}
				if ((bool)(Object)(object)component.characterMotor)
				{
					if (!component.characterMotor.isGrounded)
					{
						flag = false;
					}
					if (!(component.characterMotor.lastGroundedTime.timeSince < 0.5f))
					{
					}
				}
				if (!component.outOfCombat && component.outOfCombatStopwatch < 0.5f)
				{
					flag = true;
				}
				if (!burrowController || !burrowController.burrowed)
				{
					flag = false;
				}
				if (!flag)
				{
					search.FilterOutGameObject(((Component)(object)component).gameObject);
				}
			}
			search.FilterOutGameObject(base.gameObject);
			trackingTarget = search.GetResults().FirstOrDefault();
			if ((bool)characterBody.masterObject.GetComponent<BaseAI>() && (bool)trackingTarget)
			{
				characterBody.masterObject.GetComponent<BaseAI>().currentEnemy.gameObject = ((Component)(object)trackingTarget.healthComponent).gameObject;
			}
			else if (!trackingTarget && burrowController.burrowed && (bool)characterBody.masterObject.GetComponent<BaseAI>().currentEnemy.gameObject)
			{
				characterBody.masterObject.GetComponent<BaseAI>().currentEnemy.Reset();
			}
		}
	}
}
