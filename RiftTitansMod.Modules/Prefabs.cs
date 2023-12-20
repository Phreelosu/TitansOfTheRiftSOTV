using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Rendering;

namespace RiftTitansMod.Modules {

	internal static class Prefabs
	{
		private static PhysicMaterial ragdollMaterial;

		internal static List<SurvivorDef> survivorDefinitions = new List<SurvivorDef>();

		internal static List<GameObject> bodyPrefabs = new List<GameObject>();

		internal static List<GameObject> masterPrefabs = new List<GameObject>();

		internal static List<GameObject> projectilePrefabs = new List<GameObject>();

		internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix, UnlockableDef unlockableDef, float sortPosition)
		{
			string displayNameToken = "NDP_" + namePrefix + "_BODY_NAME";
			string descriptionToken = "NDP_" + namePrefix + "_BODY_DESCRIPTION";
			string outroFlavorToken = "NDP_" + namePrefix + "_BODY_OUTRO_FLAVOR";
			string mainEndingEscapeFailureFlavorToken = "NDP_" + namePrefix + "_BODY_OUTRO_FAILURE";
			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = bodyPrefab;
			survivorDef.displayPrefab = displayPrefab;
			survivorDef.primaryColor = charColor;
			survivorDef.displayNameToken = displayNameToken;
			survivorDef.descriptionToken = descriptionToken;
			survivorDef.outroFlavorToken = outroFlavorToken;
			survivorDef.mainEndingEscapeFailureFlavorToken = mainEndingEscapeFailureFlavorToken;
			survivorDef.desiredSortPosition = sortPosition;
			survivorDef.unlockableDef = unlockableDef;
			survivorDefinitions.Add(survivorDef);
		}

		internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix)
		{
			RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, namePrefix, null, 100f);
		}

		internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix, float sortPosition)
		{
			RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, namePrefix, null, sortPosition);
		}

		internal static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string namePrefix, UnlockableDef unlockableDef)
		{
			RegisterNewSurvivor(bodyPrefab, displayPrefab, charColor, namePrefix, unlockableDef, 100f);
		}

		internal static GameObject CreateDisplayPrefab(string modelName, GameObject prefab, BodyInfo bodyInfo)
		{
			if (!Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"))
			{
				Debug.LogError(bodyInfo.bodyNameToClone + "Body is not a valid body, character creation failed");
				return null;
			}
			GameObject gameObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"), modelName + "Prefab", true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Prefabs.cs", "CreateDisplayPrefab", 56);
			GameObject gameObject2 = CreateModel(gameObject, modelName);
			Transform transform = SetupModel(gameObject, gameObject2.transform, bodyInfo);
			gameObject2.AddComponent<CharacterModel>().baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;
			Assets.ConvertAllRenderersToHopooShader(gameObject2);
			return gameObject2.gameObject;
		}

		internal static GameObject CreatePrefab(string bodyName, string modelName, BodyInfo bodyInfo)
		{
			if (!Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"))
			{
				Debug.LogError(bodyInfo.bodyNameToClone + "Body is not a valid body, character creation failed");
				return null;
			}
			GameObject gameObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyInfo.bodyNameToClone + "Body"), bodyName, true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Prefabs.cs", "CreatePrefab", 76);
			Transform transform = null;
			GameObject gameObject2 = null;
			if (modelName != "mdl")
			{
				Debug.Log(modelName);
				gameObject2 = CreateModel(gameObject, modelName);
				if (gameObject2 == null)
				{
					gameObject2 = gameObject.GetComponentInChildren<CharacterModel>().gameObject;
				}
				transform = SetupModel(gameObject, gameObject2.transform, bodyInfo);
			}
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			((Object)(object)component).name = bodyInfo.bodyName;
			component.baseNameToken = bodyInfo.bodyNameToken;
			component.subtitleNameToken = bodyInfo.subtitleNameToken;
			component.portraitIcon = bodyInfo.characterPortrait;
			component._defaultCrosshairPrefab = bodyInfo.crosshair;
			component.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
			component.rootMotionInMainState = false;
			component.baseMaxHealth = bodyInfo.maxHealth;
			component.levelMaxHealth = bodyInfo.healthGrowth;
			component.baseRegen = bodyInfo.healthRegen;
			component.levelRegen = component.baseRegen * 0.2f;
			component.baseMaxShield = bodyInfo.shield;
			component.levelMaxShield = bodyInfo.shieldGrowth;
			component.baseMoveSpeed = bodyInfo.moveSpeed;
			component.levelMoveSpeed = bodyInfo.moveSpeedGrowth;
			component.baseAcceleration = bodyInfo.acceleration;
			component.baseJumpPower = bodyInfo.jumpPower;
			component.levelJumpPower = bodyInfo.jumpPowerGrowth;
			component.baseDamage = bodyInfo.damage;
			component.levelDamage = component.baseDamage * 0.2f;
			component.baseAttackSpeed = bodyInfo.attackSpeed;
			component.levelAttackSpeed = bodyInfo.attackSpeedGrowth;
			component.baseArmor = bodyInfo.armor;
			component.levelArmor = bodyInfo.armorGrowth;
			component.baseCrit = bodyInfo.crit;
			component.levelCrit = bodyInfo.critGrowth;
			component.baseJumpCount = bodyInfo.jumpCount;
			component.sprintingSpeedMultiplier = 1.45f;
			component.hideCrosshair = false;
			component.aimOriginTransform = transform.Find("AimOrigin");
			component.hullClassification = HullClassification.Human;
			component.preferredPodPrefab = bodyInfo.podPrefab;
			component.isChampion = false;
			component.bodyColor = bodyInfo.bodyColor;
			if (transform != null)
			{
				SetupCharacterDirection(gameObject, transform, gameObject2.transform);
			}
			SetupCameraTargetParams(gameObject);
			if (transform != null)
			{
				SetupModelLocator(gameObject, transform, gameObject2.transform);
			}
			SetupRigidbody(gameObject);
			SetupCapsuleCollider(gameObject);
			SetupMainHurtbox(gameObject, gameObject2);
			SetupFootstepController(gameObject2);
			SetupRagdoll(gameObject2);
			SetupAimAnimator(gameObject, gameObject2);
			bodyPrefabs.Add(gameObject);
			return gameObject;
		}

		internal static void CreateGenericDoppelganger(GameObject bodyPrefab, string masterName, string masterToCopy)
		{
			GameObject gameObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/" + masterToCopy + "MonsterMaster"), masterName, true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Prefabs.cs", "CreateGenericDoppelganger", 161);
			gameObject.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
			masterPrefabs.Add(gameObject);
		}

		private static Transform SetupModel(GameObject prefab, Transform modelTransform)
		{
			GameObject gameObject = new GameObject("ModelBase");
			gameObject.transform.parent = prefab.transform;
			gameObject.transform.localPosition = new Vector3(0f, -0.92f, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			GameObject gameObject2 = new GameObject("CameraPivot");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 1.6f, 0f);
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = Vector3.one;
			GameObject gameObject3 = new GameObject("AimOrigin");
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = new Vector3(0f, 1.8f, 0f);
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localScale = Vector3.one;
			prefab.GetComponent<CharacterBody>().aimOriginTransform = gameObject3.transform;
			modelTransform.parent = gameObject.transform;
			modelTransform.localPosition = Vector3.zero;
			modelTransform.localRotation = Quaternion.identity;
			return gameObject.transform;
		}

		private static Transform SetupModel(GameObject prefab, Transform modelTransform, BodyInfo bodyInfo)
		{
			GameObject gameObject = new GameObject("ModelBase");
			gameObject.transform.parent = prefab.transform;
			gameObject.transform.localPosition = bodyInfo.modelBasePosition;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			GameObject gameObject2 = new GameObject("CameraPivot");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = bodyInfo.cameraPivotPosition;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = Vector3.one;
			GameObject gameObject3 = new GameObject("AimOrigin");
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = bodyInfo.aimOriginPosition;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localScale = Vector3.one;
			prefab.GetComponent<CharacterBody>().aimOriginTransform = gameObject3.transform;
			modelTransform.parent = gameObject.transform;
			modelTransform.localPosition = Vector3.zero;
			modelTransform.localRotation = Quaternion.identity;
			return gameObject.transform;
		}

		private static GameObject CreateModel(GameObject main, string modelName)
		{
			Object.DestroyImmediate(main.transform.Find("ModelBase").gameObject);
			Object.DestroyImmediate(main.transform.Find("CameraPivot").gameObject);
			Object.DestroyImmediate(main.transform.Find("AimOrigin").gameObject);
			if (Assets.mainAssetBundle.LoadAsset<GameObject>(modelName) == null)
			{
				Debug.LogError("Trying to load a null model- check to see if the name in your code matches the name of the object in Unity");
				return null;
			}
			return Object.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>(modelName));
		}

		internal static GameObject CreateDisplayPrefab(string modelName, GameObject prefab)
		{
			GameObject gameObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), modelName + "Prefab", true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Prefabs.cs", "CreateDisplayPrefab", 242);
			GameObject gameObject2 = CreateModel(gameObject, modelName);
			Transform transform = SetupModel(gameObject, gameObject2.transform);
			gameObject2.AddComponent<CharacterModel>().baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;
			Assets.ConvertAllRenderersToHopooShader(gameObject2);
			return gameObject2.gameObject;
		}

		public static Transform SetupModel2(GameObject prefab, Transform modelTransform)
		{
			GameObject gameObject = new GameObject("ModelBase");
			gameObject.transform.parent = prefab.transform;
			gameObject.transform.localPosition = new Vector3(0f, -0.92f, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			modelTransform.parent = gameObject.transform;
			modelTransform.localPosition = Vector3.zero;
			modelTransform.localRotation = Quaternion.identity;
			return gameObject.transform;
		}

		internal static void SetupCharacterModel(GameObject prefab, CustomRendererInfo[] rendererInfo, int mainRendererIndex)
		{
			CharacterModel characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();
			ChildLocator component = characterModel.GetComponent<ChildLocator>();
			characterModel.body = prefab.GetComponent<CharacterBody>();
			if (!component)
			{
				Debug.LogError("Failed CharacterModel setup: ChildLocator component does not exist on the model");
				return;
			}
			List<CharacterModel.RendererInfo> list = new List<CharacterModel.RendererInfo>();
			for (int i = 0; i < rendererInfo.Length; i++)
			{
				if (!component.FindChild(rendererInfo[i].childName))
				{
					Debug.LogError("Trying to add a RendererInfo for a renderer that does not exist: " + rendererInfo[i].childName);
					continue;
				}
				Renderer component2 = component.FindChild(rendererInfo[i].childName).GetComponent<Renderer>();
				if ((bool)component2)
				{
					list.Add(new CharacterModel.RendererInfo
					{
						renderer = component.FindChild(rendererInfo[i].childName).GetComponent<Renderer>(),
						defaultMaterial = rendererInfo[i].material,
						ignoreOverlays = rendererInfo[i].ignoreOverlays,
						defaultShadowCastingMode = ShadowCastingMode.On
					});
				}
			}
			characterModel.baseRendererInfos = list.ToArray();
			characterModel.autoPopulateLightInfos = true;
			characterModel.invisibilityCount = 0;
			characterModel.temporaryOverlays = new List<TemporaryOverlay>();
			if (mainRendererIndex > characterModel.baseRendererInfos.Length)
			{
				Debug.LogError("mainRendererIndex out of range: not setting mainSkinnedMeshRenderer for " + prefab.name);
			}
			else
			{
				characterModel.mainSkinnedMeshRenderer = characterModel.baseRendererInfos[mainRendererIndex].renderer.GetComponent<SkinnedMeshRenderer>();
			}
		}

		private static void SetupCharacterDirection(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
		{
			CharacterDirection component = prefab.GetComponent<CharacterDirection>();
			component.targetTransform = modelBaseTransform;
			component.overrideAnimatorForwardTransform = null;
			component.rootMotionAccumulator = null;
			component.modelAnimator = modelTransform.GetComponent<Animator>();
			component.driveFromRootRotation = false;
			component.turnSpeed = 720f;
		}

		private static void SetupCameraTargetParams(GameObject prefab)
		{
			CameraTargetParams component = prefab.GetComponent<CameraTargetParams>();
			component.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/MercBody").GetComponent<CameraTargetParams>().cameraParams;
			component.cameraPivotTransform = prefab.transform.Find("ModelBase").Find("CameraPivot");
			//component.aimMode = CameraTargetParams.AimType.Standard;
			component.recoil = Vector2.zero;
			//component.idealLocalCameraPos = Vector3.zero;
			component.dontRaycastToPivot = false;
		}

		private static void SetupModelLocator(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
		{
			ModelLocator component = prefab.GetComponent<ModelLocator>();
			component.modelTransform = modelTransform;
			component.modelBaseTransform = modelBaseTransform;
		}

		private static void SetupRigidbody(GameObject prefab)
		{
			Rigidbody component = prefab.GetComponent<Rigidbody>();
			component.mass = 100f;
		}

		private static void SetupCapsuleCollider(GameObject prefab)
		{
			CapsuleCollider component = prefab.GetComponent<CapsuleCollider>();
			component.center = new Vector3(0f, 0f, 0f);
			component.radius = 0.5f;
			component.height = 1.82f;
			component.direction = 1;
		}

		private static void SetupMainHurtbox(GameObject prefab, GameObject model)
		{
			ChildLocator component = model.GetComponent<ChildLocator>();
			if (!component.FindChild("MainHurtbox"))
			{
				Debug.LogError("Could not set up main hurtbox: make sure you have a transform pair in your prefab's ChildLocator component called 'MainHurtbox'");
				return;
			}
			HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();
			HurtBox hurtBox = component.FindChild("MainHurtbox").gameObject.AddComponent<HurtBox>();
			hurtBox.gameObject.layer = LayerIndex.entityPrecise.intVal;
			hurtBox.healthComponent = prefab.GetComponent<HealthComponent>();
			hurtBox.isBullseye = true;
			hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
			hurtBox.hurtBoxGroup = hurtBoxGroup;
			hurtBox.indexInGroup = 0;
			short num = 1;
			List<HurtBox> list = new List<HurtBox>();
			ChildLocator.NameTransformPair[] transformPairs = component.transformPairs;
			for (int i = 0; i < transformPairs.Length; i++)
			{
				ChildLocator.NameTransformPair nameTransformPair = transformPairs[i];
				if (nameTransformPair.name.Contains("Hurtbox") && !nameTransformPair.name.Contains("Main"))
				{
					HurtBox hurtBox2 = nameTransformPair.transform.gameObject.AddComponent<HurtBox>();
					hurtBox2.gameObject.layer = LayerIndex.entityPrecise.intVal;
					hurtBox2.healthComponent = prefab.GetComponent<HealthComponent>();
					hurtBox2.isBullseye = false;
					hurtBox2.damageModifier = HurtBox.DamageModifier.Normal;
					hurtBox2.hurtBoxGroup = hurtBoxGroup;
					hurtBox2.indexInGroup = num;
					list.Add(hurtBox2);
					num = (short)(num + 1);
				}
			}
			list.Insert(0, hurtBox);
			HurtBox[] hurtBoxes = list.ToArray();
			hurtBoxGroup.hurtBoxes = hurtBoxes;
			hurtBoxGroup.mainHurtBox = hurtBox;
			hurtBoxGroup.bullseyeCount = 1;
		}

		private static void SetupFootstepController(GameObject model)
		{
			FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
			footstepHandler.baseFootstepString = "Play_player_footstep";
			footstepHandler.sprintFootstepOverrideString = "";
			footstepHandler.enableFootstepDust = true;
			footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");
		}

		private static void SetupRagdoll(GameObject model)
		{
			RagdollController component = model.GetComponent<RagdollController>();
			if (!component)
			{
				return;
			}
			if (ragdollMaterial == null)
			{
				ragdollMaterial = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<RagdollController>().bones[1].GetComponent<Collider>().material;
			}
			Transform[] bones = component.bones;
			foreach (Transform transform in bones)
			{
				if ((bool)transform)
				{
					transform.gameObject.layer = LayerIndex.ragdoll.intVal;
					Collider component2 = transform.GetComponent<Collider>();
					if ((bool)component2)
					{
						component2.material = ragdollMaterial;
						component2.sharedMaterial = ragdollMaterial;
					}
				}
			}
		}

		private static void SetupAimAnimator(GameObject prefab, GameObject model)
		{
			AimAnimator aimAnimator = model.AddComponent<AimAnimator>();
			aimAnimator.directionComponent = prefab.GetComponent<CharacterDirection>();
			aimAnimator.pitchRangeMax = 80f;
			aimAnimator.pitchRangeMin = -80f;
			aimAnimator.yawRangeMin = -60f;
			aimAnimator.yawRangeMax = 60f;
			aimAnimator.pitchGiveupRange = 30f;
			aimAnimator.yawGiveupRange = 10f;
			aimAnimator.giveupDuration = 3f;
			aimAnimator.inputBank = prefab.GetComponent<InputBankTest>();
		}

		internal static HitBoxGroup SetupHitbox(GameObject prefab, Transform hitboxTransform, string hitboxName)
		{
			HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();
			HitBox hitBox = hitboxTransform.gameObject.AddComponent<HitBox>();
			hitboxTransform.gameObject.layer = LayerIndex.projectile.intVal;
			hitBoxGroup.hitBoxes = new HitBox[1] { hitBox };
			hitBoxGroup.groupName = hitboxName;
			return hitBoxGroup;
		}

		internal static void SetupHitbox(GameObject prefab, string hitboxName, params Transform[] hitboxTransforms)
		{
			HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();
			List<HitBox> list = new List<HitBox>();
			foreach (Transform transform in hitboxTransforms)
			{
				HitBox item = transform.gameObject.AddComponent<HitBox>();
				transform.gameObject.layer = LayerIndex.projectile.intVal;
				list.Add(item);
			}
			hitBoxGroup.hitBoxes = list.ToArray();
			hitBoxGroup.groupName = hitboxName;
		}
	}
}