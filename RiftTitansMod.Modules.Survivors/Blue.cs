using System.Collections.Generic;
using EntityStates;
using R2API;
using RiftTitansMod.SkillStates.Blue;
using RiftTitansMod.SkillStates.Herald;
using RiftTitansMod.SkillStates.Reksai;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Skills;
using UnityEngine;

namespace RiftTitansMod.Modules.Survivors {

	public static class Blue
	{
		internal static int spawnCost = 40;

		internal static GameObject characterPrefab;

		internal static GameObject displayPrefab;

		internal static GameObject enemyMaster;

		internal static UnlockableDef characterUnlockableDef;

		internal static SkillDef unburrowSkillDef;

		public static HitBoxGroup chargeHitbox;

		public const string bodyName = "BlueBody";

		public static int bodyRendererIndex;

		internal static ItemDisplayRuleSet itemDisplayRuleSet;

		internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

		internal static void CreateCharacter()
		{
			bool flag = true;
			characterPrefab = Prefabs.CreatePrefab("BlueBody", "mdlBlueBuff", new BodyInfo
			{
				moveSpeed = 11f,
				acceleration = 40f,
				armor = 8f,
				armorGrowth = 0f,
				bodyName = "BlueBody",
				bodyNameToken = "NDP_BLUE_BODY_NAME",
				bodyColor = Color.cyan,
				characterPortrait = Assets.LoadCharacterIcon("Blue"),
				crosshair = Assets.LoadCrosshair("Standard"),
				damage = 16f,
				healthGrowth = 130f,
				healthRegen = 0f,
				jumpCount = 1,
				maxHealth = 450f,
				subtitleNameToken = "NDP_BLUE_BODY_SUBTITLE",
				podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
				cameraPivotPosition = new Vector3(0f, 5f, 0f),
				aimOriginPosition = new Vector3(0f, 4f, 0f)
			});
			Material material = Assets.CreateMaterial("matBlueBuff", 4.25E-05f);
			bodyRendererIndex = 0;
			Prefabs.SetupCharacterModel(characterPrefab, new CustomRendererInfo[1]
			{
			new CustomRendererInfo
			{
				childName = "Body",
				material = material
			}
			}, bodyRendererIndex);
			CreateHitboxes();
			CreateSkills();
			//CreateSkins();
			InitializeItemDisplays();
			DeathRewards deathRewards = characterPrefab.AddComponent<DeathRewards>();
			SetStateOnHurt component = characterPrefab.GetComponent<SetStateOnHurt>();
			component.canBeStunned = true;
			component.canBeHitStunned = true;
			component.canBeFrozen = true;
			EntityStateMachine component2 = characterPrefab.GetComponent<EntityStateMachine>();
			component2.initialStateType = new SerializableEntityStateType(typeof(RiftTitansMod.SkillStates.Blue.SpawnState));
			CharacterDeathBehavior component3 = characterPrefab.GetComponent<CharacterDeathBehavior>();
			component3.deathState = new SerializableEntityStateType(typeof(RiftTitansMod.SkillStates.Blue.DeathState));
			CharacterBody component4 = characterPrefab.GetComponent<CharacterBody>();
			component4.moveSpeed = 9f;
			component4.bodyFlags = CharacterBody.BodyFlags.None;
			CharacterMotor component5 = characterPrefab.GetComponent<CharacterMotor>();
			component5.mass = 450f;
			characterPrefab.GetComponent<ModelLocator>().normalizeToFloor = false;
			CharacterDirection component6 = characterPrefab.GetComponent<CharacterDirection>();
			component6.turnSpeed = 240f;
			FootstepHandler component7 = characterPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<FootstepHandler>();
			component7.baseFootstepString = "Play_golem_step";
			component7.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericLargeFootstepDust");
			SfxLocator component8 = characterPrefab.GetComponent<SfxLocator>();
			component8.fallDamageSound = string.Empty;
			component8.landingSound = string.Empty;
			EntityLocator entityLocator = characterPrefab.AddComponent<EntityLocator>();
			entityLocator.entity = characterPrefab;
			enemyMaster = CreateMaster(characterPrefab, "BlueMaster");
			CreateSpawnCard();
		}

		private static GameObject CreateMaster(GameObject bodyPrefab, string masterName)
		{
			GameObject gameObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianMaster"), masterName, true, "CreateMaster", "CreateMaster", 140);
			gameObject.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
			AISkillDriver[] componentsInChildren = gameObject.GetComponentsInChildren<AISkillDriver>();
			foreach (AISkillDriver obj in componentsInChildren)
			{
				Object.DestroyImmediate(obj);
			}
			gameObject.GetComponent<BaseAI>().fullVision = false;
			gameObject.GetComponent<BaseAI>().aimVectorMaxSpeed = 360f;
			AISkillDriver aISkillDriver = gameObject.AddComponent<AISkillDriver>();
			aISkillDriver.customName = "Slam";
			aISkillDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aISkillDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aISkillDriver.activationRequiresAimConfirmation = false;
			aISkillDriver.activationRequiresTargetLoS = false;
			aISkillDriver.selectionRequiresTargetLoS = false;
			aISkillDriver.maxDistance = 15f;
			aISkillDriver.minDistance = 0f;
			aISkillDriver.requireSkillReady = false;
			aISkillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
			aISkillDriver.ignoreNodeGraph = false;
			aISkillDriver.moveInputScale = 0.8f;
			aISkillDriver.driverUpdateTimerOverride = 0.5f;
			aISkillDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aISkillDriver.minTargetHealthFraction = float.NegativeInfinity;
			aISkillDriver.maxTargetHealthFraction = float.PositiveInfinity;
			aISkillDriver.minUserHealthFraction = float.NegativeInfinity;
			aISkillDriver.maxUserHealthFraction = float.PositiveInfinity;
			aISkillDriver.skillSlot = SkillSlot.Primary;
			AISkillDriver aISkillDriver2 = gameObject.AddComponent<AISkillDriver>();
			aISkillDriver2.customName = "Chase";
			aISkillDriver2.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
			aISkillDriver2.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
			aISkillDriver2.activationRequiresAimConfirmation = false;
			aISkillDriver2.activationRequiresTargetLoS = false;
			aISkillDriver2.selectionRequiresTargetLoS = false;
			aISkillDriver2.maxDistance = float.PositiveInfinity;
			aISkillDriver2.minDistance = 0f;
			aISkillDriver2.requireSkillReady = false;
			aISkillDriver2.aimType = AISkillDriver.AimType.AtMoveTarget;
			aISkillDriver2.ignoreNodeGraph = false;
			aISkillDriver2.moveInputScale = 1f;
			aISkillDriver2.driverUpdateTimerOverride = -1f;
			aISkillDriver2.buttonPressType = AISkillDriver.ButtonPressType.Hold;
			aISkillDriver2.minTargetHealthFraction = float.NegativeInfinity;
			aISkillDriver2.maxTargetHealthFraction = float.PositiveInfinity;
			aISkillDriver2.minUserHealthFraction = float.NegativeInfinity;
			aISkillDriver2.maxUserHealthFraction = float.PositiveInfinity;
			aISkillDriver2.skillSlot = SkillSlot.None;
			aISkillDriver2.shouldSprint = false;
			Prefabs.masterPrefabs.Add(gameObject);
			return gameObject;
		}

		private static void CreateSpawnCard()
		{
			CharacterSpawnCard characterSpawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
			characterSpawnCard.name = "cscBlue";
			characterSpawnCard.prefab = enemyMaster;
			characterSpawnCard.sendOverNetwork = true;
			characterSpawnCard.hullSize = HullClassification.Golem;
			characterSpawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
			characterSpawnCard.requiredFlags = NodeFlags.None;
			characterSpawnCard.forbiddenFlags = NodeFlags.TeleporterOK;
			characterSpawnCard.directorCreditCost = spawnCost;
			characterSpawnCard.occupyPosition = false;
			characterSpawnCard.loadout = new SerializableLoadout();
			characterSpawnCard.noElites = false;
			characterSpawnCard.forbiddenAsBoss = false;
			DirectorCard card = new DirectorCard
			{
				spawnCard = characterSpawnCard,
				selectionWeight = 1,
				preventOverhead = false,
				minimumStageCompletions = 0,
				spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
			};
			DirectorAPI.DirectorCardHolder relicCard = new DirectorAPI.DirectorCardHolder
			{
				Card = card,
				MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
				InteractableCategory = (DirectorAPI.InteractableCategory)0
			};
			DirectorCard card2 = new DirectorCard
			{
				spawnCard = characterSpawnCard,
				selectionWeight = 1,
				preventOverhead = false,
				minimumStageCompletions = 5,
				spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
			};
			DirectorAPI.DirectorCardHolder blueLoopCard = new DirectorAPI.DirectorCardHolder
			{
				Card = card2,
				MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
			};
			RiftTitansPlugin.BlueCard = relicCard;
			RiftTitansPlugin.BlueLoopCard = blueLoopCard;

			foreach (RiftTitansPlugin.StageSpawnInfo ssi in RiftTitansPlugin.StageList)
			{
				DirectorAPI.DirectorCardHolder toAdd = ssi.GetMinStages() == 0 ? RiftTitansPlugin.BlueCard : RiftTitansPlugin.BlueLoopCard;

				SceneDef sd = ScriptableObject.CreateInstance<SceneDef>();
				sd.baseSceneNameOverride = ssi.GetStageName();

				DirectorAPI.Helpers.AddNewMonsterToStage(toAdd, false, DirectorAPI.GetStageEnumFromSceneDef(sd), ssi.GetStageName());
			}
		}

		private static void CreateHitboxes()
		{
			ChildLocator componentInChildren = characterPrefab.GetComponentInChildren<ChildLocator>();
			GameObject gameObject = componentInChildren.gameObject;
		}

		private static void CreateSkills()
		{
			Skills.CreateSkillFamilies(characterPrefab);
			string text = "NDP";
			SkillDefInfo skillDefInfo = new SkillDefInfo();
			skillDefInfo.skillName = text + "_RELIC_BODY_SECONDARY_BUFFDISC_NAME";
			skillDefInfo.skillNameToken = text + "_RELIC_BODY_SECONDARY_BUFFDISC_NAME";
			skillDefInfo.skillDescriptionToken = text + "_RELIC_BODY_SECONDARY_BUFFDISC_DESCRIPTION";
			skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarEncore1Icon");
			skillDefInfo.activationState = new SerializableEntityStateType(typeof(Slam));
			skillDefInfo.activationStateMachineName = "Body";
			skillDefInfo.baseMaxStock = 1;
			skillDefInfo.baseRechargeInterval = 1.3f;
			skillDefInfo.beginSkillCooldownOnSkillEnd = false;
			skillDefInfo.canceledFromSprinting = false;
			skillDefInfo.forceSprintDuringState = false;
			skillDefInfo.fullRestockOnAssign = true;
			skillDefInfo.interruptPriority = InterruptPriority.Skill;
			skillDefInfo.resetCooldownTimerOnUse = false;
			skillDefInfo.isCombatSkill = true;
			skillDefInfo.mustKeyPress = false;
			skillDefInfo.cancelSprintingOnActivation = false;
			skillDefInfo.rechargeStock = 1;
			skillDefInfo.requiredStock = 1;
			skillDefInfo.stockToConsume = 1;
			SkillDef skillDef = Skills.CreateSkillDef(skillDefInfo);
			Skills.AddPrimarySkill(characterPrefab, skillDef);
			skillDefInfo = new SkillDefInfo();
			skillDefInfo.skillName = text + "_RELIC_BODY_SECONDARY_BUFFDISC_NAME";
			skillDefInfo.skillNameToken = text + "_RELIC_BODY_SECONDARY_BUFFDISC_NAME";
			skillDefInfo.skillDescriptionToken = text + "_RELIC_BODY_SECONDARY_BUFFDISC_DESCRIPTION";
			skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarEncore1Icon");
			skillDefInfo.activationState = new SerializableEntityStateType(typeof(FireSeeker));
			skillDefInfo.activationStateMachineName = "Body";
			skillDefInfo.baseMaxStock = 2;
			skillDefInfo.baseRechargeInterval = 8f;
			skillDefInfo.beginSkillCooldownOnSkillEnd = false;
			skillDefInfo.canceledFromSprinting = false;
			skillDefInfo.forceSprintDuringState = false;
			skillDefInfo.fullRestockOnAssign = true;
			skillDefInfo.interruptPriority = InterruptPriority.Skill;
			skillDefInfo.resetCooldownTimerOnUse = false;
			skillDefInfo.isCombatSkill = false;
			skillDefInfo.mustKeyPress = false;
			skillDefInfo.cancelSprintingOnActivation = false;
			skillDefInfo.rechargeStock = 2;
			skillDefInfo.requiredStock = 1;
			skillDefInfo.stockToConsume = 1;
			SkillDef skillDef2 = Skills.CreateBurrowedSkillDef(skillDefInfo);
			Skills.AddSecondarySkills(characterPrefab, skillDef2);
			skillDefInfo = new SkillDefInfo();
			skillDefInfo.skillName = text + "_RELIC_BODY_UTILITY_ROLL_NAME";
			skillDefInfo.skillNameToken = text + "_RELIC_BODY_UTILITY_ROLL_NAME";
			skillDefInfo.skillDescriptionToken = text + "_RELIC_BODY_UTILITY_ROLL_DESCRIPTION";
			skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRepositionIcon");
			skillDefInfo.activationState = new SerializableEntityStateType(typeof(Charge));
			skillDefInfo.activationStateMachineName = "Body";
			skillDefInfo.baseMaxStock = 1;
			skillDefInfo.baseRechargeInterval = 5f;
			skillDefInfo.beginSkillCooldownOnSkillEnd = true;
			skillDefInfo.canceledFromSprinting = false;
			skillDefInfo.forceSprintDuringState = false;
			skillDefInfo.fullRestockOnAssign = true;
			skillDefInfo.interruptPriority = InterruptPriority.Skill;
			skillDefInfo.resetCooldownTimerOnUse = false;
			skillDefInfo.isCombatSkill = true;
			skillDefInfo.mustKeyPress = false;
			skillDefInfo.cancelSprintingOnActivation = true;
			skillDefInfo.rechargeStock = 1;
			skillDefInfo.requiredStock = 1;
			skillDefInfo.stockToConsume = 1;
			SkillDef skillDef3 = Skills.CreateSkillDef(skillDefInfo);
			Skills.AddUtilitySkills(characterPrefab, skillDef3);
			skillDefInfo = new SkillDefInfo();
			skillDefInfo.skillName = text + "_RELIC_BODY_SPECIAL_BOMB_NAME";
			skillDefInfo.skillNameToken = text + "_RELIC_BODY_SPECIAL_BOMB_NAME";
			skillDefInfo.skillDescriptionToken = text + "_RELIC_BODY_SPECIAL_BOMB_DESCRIPTION";
			skillDefInfo.skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texCrescentSlabsIcon");
			skillDefInfo.activationState = new SerializableEntityStateType(typeof(Special));
			skillDefInfo.activationStateMachineName = "Body";
			skillDefInfo.baseMaxStock = 1;
			skillDefInfo.baseRechargeInterval = 60f;
			skillDefInfo.beginSkillCooldownOnSkillEnd = false;
			skillDefInfo.canceledFromSprinting = false;
			skillDefInfo.forceSprintDuringState = false;
			skillDefInfo.fullRestockOnAssign = true;
			skillDefInfo.interruptPriority = InterruptPriority.PrioritySkill;
			skillDefInfo.resetCooldownTimerOnUse = false;
			skillDefInfo.isCombatSkill = true;
			skillDefInfo.mustKeyPress = false;
			skillDefInfo.cancelSprintingOnActivation = true;
			skillDefInfo.rechargeStock = 1;
			skillDefInfo.requiredStock = 1;
			skillDefInfo.stockToConsume = 1;
			SkillDef skillDef4 = Skills.CreateTrackingSkillDef(skillDefInfo);
			Skills.AddSpecialSkills(characterPrefab, skillDef4);
		}

		/*private static void CreateSkins()
		{
			GameObject gameObject = characterPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
			CharacterModel component = gameObject.GetComponent<CharacterModel>();
			ModelSkinController modelSkinController = gameObject.AddComponent<ModelSkinController>();
			ChildLocator component2 = gameObject.GetComponent<ChildLocator>();
			SkinnedMeshRenderer mainSkinnedMeshRenderer = component.mainSkinnedMeshRenderer;
			CharacterModel.RendererInfo[] baseRendererInfos = component.baseRendererInfos;
			List<SkinDef> list = new List<SkinDef>();
			SkinDef skinDef = Skins.CreateSkinDef("NDP_RELIC_BODY_DEFAULT_SKIN_NAME", Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"), baseRendererInfos, mainSkinnedMeshRenderer, gameObject);
			skinDef.meshReplacements = new SkinDef.MeshReplacement[0];
			list.Add(skinDef);
			modelSkinController.skins = list.ToArray();
		}*/

		private static void InitializeItemDisplays()
		{
			CharacterModel componentInChildren = characterPrefab.GetComponentInChildren<CharacterModel>();
			itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
			itemDisplayRuleSet.name = "idrsBlue";
			componentInChildren.itemDisplayRuleSet = itemDisplayRuleSet;
		}

		internal static void SetItemDisplays()
		{
			itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Jetpack,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
						childName = "Chest",
						localPos = new Vector3(0.0009f, 0.2767f, -0.1f),
						localAngles = new Vector3(21.4993f, 358.6616f, 358.3334f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.GoldGat,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
						childName = "Chest",
						localPos = new Vector3(0.1009f, 0.4728f, -0.1278f),
						localAngles = new Vector3(22.6043f, 114.6042f, 299.1935f),
						localScale = new Vector3(0.15f, 0.15f, 0.15f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.BFG,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
						childName = "Chest",
						localPos = new Vector3(0.0782f, 0.4078f, 0f),
						localAngles = new Vector3(0f, 0f, 313.6211f),
						localScale = new Vector3(0.3f, 0.3f, 0.3f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.CritGlasses,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
						childName = "Head",
						localPos = new Vector3(0f, 0.1687f, 0.1558f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.3215f, 0.3034f, 0.3034f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Syringe,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
						childName = "Chest",
						localPos = new Vector3(-0.0534f, 0.0352f, 0f),
						localAngles = new Vector3(0f, 0f, 83.2547f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Behemoth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
						childName = "Gun",
						localPos = new Vector3(0f, 0.2247f, -0.1174f),
						localAngles = new Vector3(6.223f, 180f, 0f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Missile,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
						childName = "Chest",
						localPos = new Vector3(-0.3075f, 0.5204f, -0.049f),
						localAngles = new Vector3(0f, 0f, 51.9225f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Dagger,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
						childName = "Chest",
						localPos = new Vector3(-0.0553f, 0.2856f, 0.0945f),
						localAngles = new Vector3(334.8839f, 31.5284f, 34.6784f),
						localScale = new Vector3(1.2428f, 1.2428f, 1.2299f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Hoof,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
						childName = "CalfL",
						localPos = new Vector3(-0.0071f, 0.3039f, -0.051f),
						localAngles = new Vector3(76.5928f, 0f, 0f),
						localScale = new Vector3(0.0846f, 0.0846f, 0.0758f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ChainLightning,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
						childName = "Chest",
						localPos = new Vector3(-0.0011f, 0.1031f, -0.0901f),
						localAngles = new Vector3(0f, 180f, 89.3997f),
						localScale = new Vector3(0.4749f, 0.4749f, 0.4749f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.GhostOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
						childName = "Head",
						localPos = new Vector3(0f, 0.1748f, 0.0937f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.6313f, 0.6313f, 0.6313f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Mushroom,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
						childName = "UpperArmR",
						localPos = new Vector3(-0.0139f, 0.1068f, 0f),
						localAngles = new Vector3(0f, 0f, 89.4525f),
						localScale = new Vector3(0.0501f, 0.0501f, 0.0501f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
						childName = "Head",
						localPos = new Vector3(0f, 0.2783f, -0.002f),
						localAngles = new Vector3(358.4554f, 0f, 0f),
						localScale = new Vector3(0.5666f, 0.5666f, 0.5666f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BleedOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
						childName = "Chest",
						localPos = new Vector3(-0.1247f, 0.416f, 0.1225f),
						localAngles = new Vector3(11.5211f, 128.5354f, 165.922f),
						localScale = new Vector3(0.2615f, 0.2615f, 0.2615f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.WardOnLevel,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
						childName = "Pelvis",
						localPos = new Vector3(0.0168f, 0.0817f, -0.0955f),
						localAngles = new Vector3(0f, 0f, 90f),
						localScale = new Vector3(0.3162f, 0.3162f, 0.3162f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.HealOnCrit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
						childName = "Chest",
						localPos = new Vector3(0.0278f, 0.2322f, -0.0877f),
						localAngles = new Vector3(323.9545f, 90f, 270f),
						localScale = new Vector3(0.1884f, 0.1884f, 0.1884f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.HealWhileSafe,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
						childName = "UpperArmL",
						localPos = new Vector3(0f, 0.3267f, 0.046f),
						localAngles = new Vector3(90f, 0f, 0f),
						localScale = new Vector3(0.0289f, 0.0289f, 0.0289f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Clover,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
						childName = "Gun",
						localPos = new Vector3(0.0004f, 0.1094f, -0.1329f),
						localAngles = new Vector3(85.6192f, 0.0001f, 179.4897f),
						localScale = new Vector3(0.2749f, 0.2749f, 0.2749f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BarrierOnOverHeal,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
						childName = "LowerArmL",
						localPos = new Vector3(0f, 0.1396f, 0f),
						localAngles = new Vector3(86.4709f, 180f, 180f),
						localScale = new Vector3(0.2849f, 0.2849f, 0.2849f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.GoldOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
						childName = "Head",
						localPos = new Vector3(0f, 0.1791f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(1.1754f, 1.1754f, 1.1754f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.WarCryOnMultiKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
						childName = "UpperArmL",
						localPos = new Vector3(0.0435f, 0.0905f, 0.0118f),
						localAngles = new Vector3(82.0839f, 160.9228f, 100.7722f),
						localScale = new Vector3(0.7094f, 0.7094f, 0.7094f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintArmor,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
						childName = "LowerArmR",
						localPos = new Vector3(-0.005f, 0.285f, 0.0074f),
						localAngles = new Vector3(358.4802f, 192.347f, 88.4811f),
						localScale = new Vector3(0.3351f, 0.3351f, 0.3351f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.IceRing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
						childName = "Gun",
						localPos = new Vector3(0.0334f, 0.2587f, -0.1223f),
						localAngles = new Vector3(274.3965f, 90f, 270f),
						localScale = new Vector3(0.3627f, 0.3627f, 0.3627f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FireRing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
						childName = "Gun",
						localPos = new Vector3(0.0352f, 0.282f, -0.1223f),
						localAngles = new Vector3(274.3965f, 90f, 270f),
						localScale = new Vector3(0.3627f, 0.3627f, 0.3627f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.UtilitySkillMagazine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
						childName = "UpperArmL",
						localPos = new Vector3(0f, 0f, -0.002f),
						localAngles = new Vector3(-90f, 0f, 0f),
						localScale = new Vector3(0.01f, 0.01f, 0.01f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
						childName = "UpperArmR",
						localPos = new Vector3(0f, 0f, -0.002f),
						localAngles = new Vector3(-90f, 0f, 0f),
						localScale = new Vector3(0.01f, 0.01f, 0.01f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.JumpBoost,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
						childName = "Head",
						localPos = new Vector3(0f, 0.0529f, -0.1242f),
						localAngles = new Vector3(24.419f, 0f, 0f),
						localScale = new Vector3(0.5253f, 0.5253f, 0.5253f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ArmorReductionOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
						childName = "Chest",
						localPos = new Vector3(0.0513f, 0.0652f, -0.0792f),
						localAngles = new Vector3(64.189f, 90f, 90f),
						localScale = new Vector3(0.1722f, 0.1722f, 0.1722f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.NearbyDamageBonus,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
						childName = "Sword",
						localPos = new Vector3(-0.002f, 0.1828f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1236f, 0.1236f, 0.1236f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ArmorPlate,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
						childName = "ThighL",
						localPos = new Vector3(0.0218f, 0.4276f, 0f),
						localAngles = new Vector3(90f, 180f, 0f),
						localScale = new Vector3(0.1971f, 0.1971f, 0.1971f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.CommandMissile,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.2985f, -0.0663f),
						localAngles = new Vector3(90f, 180f, 0f),
						localScale = new Vector3(0.3362f, 0.3362f, 0.3362f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Feather,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
						childName = "LowerArmL",
						localPos = new Vector3(0.001f, 0.2755f, 0.0454f),
						localAngles = new Vector3(270f, 91.2661f, 0f),
						localScale = new Vector3(0.0285f, 0.0285f, 0.0285f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Crowbar,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.1219f, -0.0764f),
						localAngles = new Vector3(90f, 90f, 0f),
						localScale = new Vector3(0.1936f, 0.1936f, 0.1936f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FallBoots,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
						childName = "CalfL",
						localPos = new Vector3(-0.0038f, 0.3729f, -0.0046f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1485f, 0.1485f, 0.1485f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
						childName = "CalfR",
						localPos = new Vector3(-0.0038f, 0.3729f, -0.0046f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1485f, 0.1485f, 0.1485f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
						childName = "ThighR",
						localPos = new Vector3(-0.0561f, 0.1607f, 0f),
						localAngles = new Vector3(90f, 90f, 0f),
						localScale = new Vector3(0.1843f, 0.1843f, 0.1843f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.EquipmentMagazine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.0791f, 0.0726f),
						localAngles = new Vector3(0f, 270f, 292.8411f),
						localScale = new Vector3(0.0773f, 0.0773f, 0.0773f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.NovaOnHeal,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
						childName = "Head",
						localPos = new Vector3(0.0949f, 0.0945f, 0.0654f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.5349f, 0.5349f, 0.5349f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
						childName = "Head",
						localPos = new Vector3(-0.0949f, 0.0945f, 0.0105f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(-0.5349f, 0.5349f, 0.5349f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Infusion,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
						childName = "Pelvis",
						localPos = new Vector3(-0.0703f, 0.0238f, -0.0366f),
						localAngles = new Vector3(0f, 45f, 0f),
						localScale = new Vector3(0.5253f, 0.5253f, 0.5253f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Medkit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
						childName = "Chest",
						localPos = new Vector3(0.0039f, -0.0125f, -0.0546f),
						localAngles = new Vector3(290f, 180f, 0f),
						localScale = new Vector3(0.4907f, 0.4907f, 0.4907f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Bandolier,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
						childName = "Chest",
						localPos = new Vector3(0.0035f, 0f, 0f),
						localAngles = new Vector3(270f, 0f, 0f),
						localScale = new Vector3(0.1684f, 0.242f, 0.242f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BounceNearby,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
						childName = "Chest",
						localPos = new Vector3(-0.0922f, 0.4106f, -0.0015f),
						localAngles = new Vector3(290.3197f, 89f, 0f),
						localScale = new Vector3(0.214f, 0.214f, 0.214f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.IgniteOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
						childName = "ThighL",
						localPos = new Vector3(0.0494f, 0.0954f, 0.0015f),
						localAngles = new Vector3(90f, 0f, 0f),
						localScale = new Vector3(0.3165f, 0.3165f, 0.3165f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.StunChanceOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
						childName = "ThighR",
						localPos = new Vector3(0.001f, 0.3609f, 0.0523f),
						localAngles = new Vector3(90f, 0f, 0f),
						localScale = new Vector3(0.5672f, 0.5672f, 0.5672f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Firework,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
						childName = "Muzzle",
						localPos = new Vector3(0.0086f, 0.0069f, 0.0565f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1194f, 0.1194f, 0.1194f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarDagger,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
						childName = "Chest",
						localPos = new Vector3(-0.0015f, 0.2234f, -0.0655f),
						localAngles = new Vector3(277.637f, 358.2474f, 1.4903f),
						localScale = new Vector3(0.3385f, 0.3385f, 0.3385f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Knurl,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
						childName = "LowerArmL",
						localPos = new Vector3(-0.0186f, 0.0405f, -0.0049f),
						localAngles = new Vector3(78.8707f, 36.6722f, 105.8275f),
						localScale = new Vector3(0.0848f, 0.0848f, 0.0848f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BeetleGland,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
						childName = "Chest",
						localPos = new Vector3(0.0852f, 0.0577f, 0f),
						localAngles = new Vector3(359.9584f, 0.1329f, 39.8304f),
						localScale = new Vector3(0.0553f, 0.0553f, 0.0553f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintBonus,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
						childName = "Pelvis",
						localPos = new Vector3(-0.075f, 0.095f, 0f),
						localAngles = new Vector3(270f, 251.0168f, 0f),
						localScale = new Vector3(0.1655f, 0.1655f, 0.1655f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SecondarySkillMagazine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
						childName = "Gun",
						localPos = new Vector3(-0.0018f, 0.0002f, 0.097f),
						localAngles = new Vector3(84.2709f, 200.5981f, 25.0139f),
						localScale = new Vector3(0.0441f, 0.0441f, 0.0441f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.StickyBomb,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
						childName = "Pelvis",
						localPos = new Vector3(0.0594f, 0.0811f, 0.0487f),
						localAngles = new Vector3(8.4958f, 176.5473f, 162.7601f),
						localScale = new Vector3(0.0736f, 0.0736f, 0.0736f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.TreasureCache,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
						childName = "Pelvis",
						localPos = new Vector3(0.0589f, 0.1056f, -0.0174f),
						localAngles = new Vector3(0.2454f, 195.0205f, 89.0854f),
						localScale = new Vector3(0.4092f, 0.4092f, 0.4092f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BossDamageBonus,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
						childName = "Pelvis",
						localPos = new Vector3(-0.0371f, 0.0675f, -0.052f),
						localAngles = new Vector3(90f, 41.5689f, 0f),
						localScale = new Vector3(0.2279f, 0.2279f, 0.2279f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SlowOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
						childName = "Pelvis",
						localPos = new Vector3(-0.0074f, 0.076f, -0.0864f),
						localAngles = new Vector3(0f, 23.7651f, 0f),
						localScale = new Vector3(0.0687f, 0.0687f, 0.0687f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ExtraLife,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.3001f, 0.0555f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.2645f, 0.2645f, 0.2645f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.KillEliteFrenzy,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
						childName = "Head",
						localPos = new Vector3(0f, 0.1882f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.2638f, 0.2638f, 0.2638f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.RepeatHeal,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
						childName = "UpperArmR",
						localPos = new Vector3(-0.0393f, 0.1484f, 0f),
						localAngles = new Vector3(270f, 90f, 0f),
						localScale = new Vector3(0.1511f, 0.1511f, 0.1511f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.AutoCastEquipment,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
						childName = "Chest",
						localPos = new Vector3(-0.0722f, 0.0921f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.4208f, 0.4208f, 0.4208f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.IncreaseHealing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
						childName = "Head",
						localPos = new Vector3(0.1003f, 0.269f, 0f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.3395f, 0.3395f, 0.3395f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
						childName = "Head",
						localPos = new Vector3(-0.1003f, 0.269f, 0f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.3395f, 0.3395f, -0.3395f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.TitanGoldDuringTP,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
						childName = "Chest",
						localPos = new Vector3(-0.0571f, 0.3027f, 0.0755f),
						localAngles = new Vector3(335.0033f, 343.2951f, 0f),
						localScale = new Vector3(0.1191f, 0.1191f, 0.1191f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintWisp,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
						childName = "UpperArmR",
						localPos = new Vector3(-0.0283f, 0.0452f, -0.0271f),
						localAngles = new Vector3(0f, 270f, 0f),
						localScale = new Vector3(0.1385f, 0.1385f, 0.1385f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BarrierOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
						childName = "Gun",
						localPos = new Vector3(-0.0097f, -0.0058f, -0.0847f),
						localAngles = new Vector3(0f, 0f, 84.3456f),
						localScale = new Vector3(0.1841f, 0.1841f, 0.1841f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.TPHealingNova,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
						childName = "UpperArmL",
						localPos = new Vector3(0.0399f, 0.1684f, 0.0121f),
						localAngles = new Vector3(0f, 73.1449f, 0f),
						localScale = new Vector3(0.2731f, 0.2731f, 0.0273f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarUtilityReplacement,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
						childName = "Head",
						localPos = new Vector3(0f, 0.2387f, -0.199f),
						localAngles = new Vector3(0f, 270f, 0f),
						localScale = new Vector3(0.2833f, 0.2833f, 0.2833f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Thorns,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
						childName = "UpperArmL",
						localPos = new Vector3(0f, 0f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.4814f, 0.4814f, 0.4814f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
						childName = "Head",
						localPos = new Vector3(0f, 0.1738f, 0.1375f),
						localAngles = new Vector3(270f, 0f, 0f),
						localScale = new Vector3(0.2866f, 0.2866f, 0.2866f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.NovaOnLowHealth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
						childName = "Head",
						localPos = new Vector3(-0.0484f, -0.0116f, 0.0283f),
						localAngles = new Vector3(316.2306f, 45.1087f, 303.6165f),
						localScale = new Vector3(0.1035f, 0.1035f, 0.1035f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LunarTrinket,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
						childName = "LowerArmL",
						localPos = new Vector3(0f, 0.3249f, 0.0381f),
						localAngles = new Vector3(0f, 0f, 90f),
						localScale = new Vector3(1f, 1f, 1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Plant,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
						childName = "UpperArmR",
						localPos = new Vector3(-0.0663f, 0.2266f, 0f),
						localAngles = new Vector3(4.9717f, 270f, 54.4915f),
						localScale = new Vector3(0.0429f, 0.0429f, 0.0429f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Bear,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.3014f, 0.0662f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.2034f, 0.2034f, 0.2034f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.DeathMark,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
						childName = "LowerArmR",
						localPos = new Vector3(0f, 0.4099f, 0.0252f),
						localAngles = new Vector3(277.5254f, 0f, 0f),
						localScale = new Vector3(-0.0375f, -0.0341f, -0.0464f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ExplodeOnDeath,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
						childName = "Pelvis",
						localPos = new Vector3(0.0595f, 0.0696f, -0.0543f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.0283f, 0.0283f, 0.0283f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Seed,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
						childName = "Head",
						localPos = new Vector3(-0.1702f, 0.1366f, -0.026f),
						localAngles = new Vector3(344.0657f, 196.8238f, 275.5892f),
						localScale = new Vector3(0.0275f, 0.0275f, 0.0275f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SprintOutOfCombat,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
						childName = "Pelvis",
						localPos = new Vector3(0.1001f, -0.0132f, 0f),
						localAngles = new Vector3(0f, 0f, 20.1526f),
						localScale = new Vector3(0.2845f, 0.2845f, 0.2845f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = JunkContent.Items.CooldownOnCrit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySkull"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.3997f, 0f),
						localAngles = new Vector3(270f, 0f, 0f),
						localScale = new Vector3(0.2789f, 0.2789f, 0.2789f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Phasing,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
						childName = "CalfL",
						localPos = new Vector3(-0.0063f, 0.2032f, -0.0507f),
						localAngles = new Vector3(90f, 0f, 0f),
						localScale = new Vector3(0.1454f, 0.2399f, 0.16f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.PersonalShield,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.2649f, 0.0689f),
						localAngles = new Vector3(304.1204f, 90f, 270f),
						localScale = new Vector3(0.1057f, 0.1057f, 0.1057f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ShockNearby,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
						childName = "Chest",
						localPos = new Vector3(0.0008f, 0.3747f, -0.0423f),
						localAngles = new Vector3(297.6866f, 1.3864f, 358.5596f),
						localScale = new Vector3(0.3229f, 0.3229f, 0.3229f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ShieldOnly,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
						childName = "Head",
						localPos = new Vector3(0.0868f, 0.3114f, 0f),
						localAngles = new Vector3(348.1819f, 268.0985f, 0.3896f),
						localScale = new Vector3(0.3521f, 0.3521f, 0.3521f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
						childName = "Head",
						localPos = new Vector3(-0.0868f, 0.3114f, 0f),
						localAngles = new Vector3(11.8181f, 268.0985f, 359.6104f),
						localScale = new Vector3(0.3521f, 0.3521f, -0.3521f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.AlienHead,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
						childName = "Chest",
						localPos = new Vector3(0.0417f, 0.2791f, -0.0493f),
						localAngles = new Vector3(284.1172f, 243.7966f, 260.89f),
						localScale = new Vector3(0.6701f, 0.6701f, 0.6701f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.HeadHunter,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
						childName = "Head",
						localPos = new Vector3(0f, 0.2556f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.4851f, 0.1617f, 0.1617f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
						childName = "Pelvis",
						localPos = new Vector3(-0.1509f, 0.0659f, 0f),
						localAngles = new Vector3(0f, 0f, 69.9659f),
						localScale = new Vector3(0.2732f, 0.2732f, 0.2732f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FlatHealth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
						childName = "Head",
						localPos = new Vector3(0f, 0.3429f, -0.0671f),
						localAngles = new Vector3(294.98f, 180f, 180f),
						localScale = new Vector3(0.1245f, 0.1155f, 0.1155f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Tooth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
						childName = "Head",
						localPos = new Vector3(0f, 0.0687f, 0.0998f),
						localAngles = new Vector3(344.9017f, 0f, 0f),
						localScale = new Vector3(7.5452f, 7.5452f, 7.5452f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Pearl,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
						childName = "LowerArmR",
						localPos = new Vector3(0f, 0f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.ShinyPearl,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
						childName = "LowerArmL",
						localPos = new Vector3(0f, 0f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
						childName = "ThighR",
						localPos = new Vector3(0.0155f, 0.2145f, 0.0615f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.0475f, 0.0475f, 0.0475f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Squid,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
						childName = "Head",
						localPos = new Vector3(-0.0164f, 0.1641f, -0.0005f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.2235f, 0.3016f, 0.3528f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Icicle,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
						childName = "Base",
						localPos = new Vector3(-0.658f, -1.0806f, 0.015f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(1f, 1f, 1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.Talisman,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
						childName = "Base",
						localPos = new Vector3(0.8357f, -0.7042f, -0.2979f),
						localAngles = new Vector3(270f, 0f, 0f),
						localScale = new Vector3(1f, 1f, 1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.LaserTurbine,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.0622f, -0.0822f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.2159f, 0.2159f, 0.2159f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FocusConvergence,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
						childName = "Base",
						localPos = new Vector3(-0.0554f, -1.6605f, -0.3314f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = JunkContent.Items.Incubator,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.3453f, 0f),
						localAngles = new Vector3(353.0521f, 317.2421f, 69.6292f),
						localScale = new Vector3(0.0528f, 0.0528f, 0.0528f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.FireballsOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
						childName = "LowerArmL",
						localPos = new Vector3(0f, 0.3365f, -0.0878f),
						localAngles = new Vector3(270f, 0f, 0f),
						localScale = new Vector3(0.0761f, 0.0761f, 0.0761f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.SiphonOnLowHealth,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
						childName = "Pelvis",
						localPos = new Vector3(0.0542f, 0.0206f, -0.0019f),
						localAngles = new Vector3(0f, 303.4368f, 0f),
						localScale = new Vector3(0.0385f, 0.0385f, 0.0385f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
						childName = "ThighR",
						localPos = new Vector3(0f, 0.0575f, -0.0178f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.0486f, 0.0486f, 0.0486f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.MonstersOnShrineUse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
						childName = "ThighR",
						localPos = new Vector3(0.0022f, 0.084f, 0.066f),
						localAngles = new Vector3(352.4521f, 260.6884f, 341.5106f),
						localScale = new Vector3(0.0246f, 0.0246f, 0.0246f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Items.RandomDamageZone,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
						childName = "LowerArmL",
						localPos = new Vector3(0.0709f, 0.4398f, 0.0587f),
						localAngles = new Vector3(349.218f, 235.9453f, 0f),
						localScale = new Vector3(0.0465f, 0.0465f, 0.0465f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Fruit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
						childName = "Chest",
						localPos = new Vector3(-0.0513f, 0.2348f, -0.1839f),
						localAngles = new Vector3(354.7403f, 305.3714f, 336.9526f),
						localScale = new Vector3(0.2118f, 0.2118f, 0.2118f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixRed,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
						childName = "Head",
						localPos = new Vector3(0.40101f, 0.31309f, 0.76865f),
						localAngles = new Vector3(11.4538f, 140.8733f, 85.28788f),
						localScale = new Vector3(0.6f, 0.6f, 0.6f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
						childName = "Head",
						localPos = new Vector3(0.35125f, -0.27999f, 0.62476f),
						localAngles = new Vector3(328.4482f, 137.3227f, 116.6408f),
						localScale = new Vector3(0.6f, 0.6f, 0.6f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixBlue,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[2]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
						childName = "Head",
						localPos = new Vector3(0.59228f, 0.0181f, 0.722f),
						localAngles = new Vector3(0.17355f, 47.33923f, 92.78063f),
						localScale = new Vector3(1.7f, 1.7f, 1.7f),
						limbMask = LimbFlags.None
					},
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
						childName = "Head",
						localPos = new Vector3(0.35566f, -0.03587f, 0.96063f),
						localAngles = new Vector3(2.90661f, 8.35415f, 80.45375f),
						localScale = new Vector3(1.2f, 1.2f, 1.2f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixWhite,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
						childName = "Head",
						localPos = new Vector3(0.51351f, 0.02604f, 1.57591f),
						localAngles = new Vector3(0.16265f, 8.04598f, 91.29423f),
						localScale = new Vector3(0.15f, 0.15f, 0.2f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixPoison,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
						childName = "Head",
						localPos = new Vector3(0.65049f, -0.01048f, 0.90211f),
						localAngles = new Vector3(0f, 21.39419f, 0f),
						localScale = new Vector3(0.2f, 0.25f, 0.5f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.AffixHaunted,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
						childName = "Head",
						localPos = new Vector3(0.32713f, -0.01177f, 1.50846f),
						localAngles = new Vector3(0f, 23.94768f, 90f),
						localScale = new Vector3(0.45f, 0.45f, 0.45f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.CritOnUse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
						childName = "Head",
						localPos = new Vector3(0f, 0.1861f, 0.2328f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.2326f, 0.2326f, 0.2326f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.DroneBackup,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
						childName = "Pelvis",
						localPos = new Vector3(0.0604f, 0.1269f, 0f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.2641f, 0.2641f, 0.2641f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Lightning,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayLightningArmRight"),
						childName = "UpperArmR",
						localPos = new Vector3(0f, 0f, 0f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.3413f, 0.3413f, 0.3413f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.BurnNearby,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
						childName = "Pelvis",
						localPos = new Vector3(0.078f, 0.065f, 0f),
						localAngles = new Vector3(359.1402f, 0.1068f, 331.8908f),
						localScale = new Vector3(0.0307f, 0.0307f, 0.0307f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.CrippleWard,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
						childName = "Pelvis",
						localPos = new Vector3(0.0768f, -0.0002f, 0f),
						localAngles = new Vector3(0f, 270f, 0f),
						localScale = new Vector3(0.2812f, 0.2812f, 0.2812f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.2584f, -0.0987f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.2188f, 0.2188f, 0.2188f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.GainArmor,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
						childName = "CalfR",
						localPos = new Vector3(0f, 0.3011f, 0.0764f),
						localAngles = new Vector3(77.5634f, 0f, 0f),
						localScale = new Vector3(0.6279f, 0.6279f, 0.6279f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Recycle,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.1976f, -0.0993f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.0802f, 0.0802f, 0.0802f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.FireBallDash,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
						childName = "Pelvis",
						localPos = new Vector3(0.0727f, 0.0252f, 0f),
						localAngles = new Vector3(270f, 0f, 0f),
						localScale = new Vector3(0.1891f, 0.1891f, 0.1891f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Cleanse,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
						childName = "Chest",
						localPos = new Vector3(0f, 0.1996f, -0.0489f),
						localAngles = new Vector3(0f, 180f, 0f),
						localScale = new Vector3(0.0821f, 0.0821f, 0.0821f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Tonic,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
						childName = "Pelvis",
						localPos = new Vector3(0.066f, 0.058f, 0f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.1252f, 0.1252f, 0.1252f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Gateway,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
						childName = "Pelvis",
						localPos = new Vector3(0.0807f, 0.0877f, 0f),
						localAngles = new Vector3(0f, 90f, 0f),
						localScale = new Vector3(0.0982f, 0.0982f, 0.0982f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Meteor,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
						childName = "Base",
						localPos = new Vector3(0f, -1.7606f, -0.9431f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(1f, 1f, 1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Saw,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
						childName = "Base",
						localPos = new Vector3(0f, -1.7606f, -0.9431f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1f, 0.1f, 0.1f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Blackhole,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
						childName = "Base",
						localPos = new Vector3(0f, -1.7606f, -0.9431f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.5f, 0.5f, 0.5f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.Scanner,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
						childName = "Pelvis",
						localPos = new Vector3(0.0857f, 0.0472f, 0.0195f),
						localAngles = new Vector3(270f, 154.175f, 0f),
						localScale = new Vector3(0.0861f, 0.0861f, 0.0861f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.DeathProjectile,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
						childName = "Pelvis",
						localPos = new Vector3(0f, 0.028f, -0.0977f),
						localAngles = new Vector3(0f, 180f, 0f),
						localScale = new Vector3(0.0596f, 0.0596f, 0.0596f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.LifestealOnHit,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
						childName = "Head",
						localPos = new Vector3(-0.2175f, 0.4404f, -0.141f),
						localAngles = new Vector3(44.0939f, 33.5151f, 43.5058f),
						localScale = new Vector3(0.1246f, 0.1246f, 0.1246f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
			{
				keyAsset = RoR2Content.Equipment.TeamWarCry,
				displayRuleGroup = new DisplayRuleGroup
				{
					rules = new ItemDisplayRule[1]
					{
					new ItemDisplayRule
					{
						ruleType = ItemDisplayRuleType.ParentedPrefab,
						followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
						childName = "Pelvis",
						localPos = new Vector3(0f, 0f, 0.1866f),
						localAngles = new Vector3(0f, 0f, 0f),
						localScale = new Vector3(0.1233f, 0.1233f, 0.1233f),
						limbMask = LimbFlags.None
					}
					}
				}
			});
			itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
			itemDisplayRuleSet.GenerateRuntimeValues();
		}

		private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
		{
			CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[defaultRenderers.Length];
			defaultRenderers.CopyTo(array, 0);
			array[0].defaultMaterial = materials[0];
			array[1].defaultMaterial = materials[1];
			array[bodyRendererIndex].defaultMaterial = materials[2];
			array[4].defaultMaterial = materials[3];
			return array;
		}
	}
}
