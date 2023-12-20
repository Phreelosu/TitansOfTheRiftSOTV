using System.Collections.Generic;
using System.IO;
using System.Reflection;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace RiftTitansMod.Modules {

	internal static class Assets
	{
		internal static AssetBundle mainAssetBundle;

		internal static GameObject baronYellEffect;

		internal static GameObject baronSpitConeEffect;

		internal static GameObject baronChargeEffect;

		internal static GameObject baronSpitEffect;

		internal static GameObject baronLineChargeEffect;

		internal static GameObject baronGroundChargeEffect;

		internal static GameObject baronTentacleEffect;

		internal static NetworkSoundEventDef baronTentacleSound;

		internal static NetworkSoundEventDef baronSpitImpactSound;

		internal static GameObject heraldChargeChargeEffect;

		internal static GameObject heraldSlamEffect;

		internal static GameObject heraldCrashEffect;

		internal static GameObject blueSlamEffect;

		internal static GameObject reksaiUnburrowEffect;

		internal static GameObject reksaiBurrowEffect;

		internal static GameObject reksaiYellEffect;

		internal static GameObject reksaiAttackEffect;

		internal static GameObject reksaiSpecialEffect;

		internal static GameObject burrowEffect;

		internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

		internal static List<EffectDef> effectDefs = new List<EffectDef>();

		internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");

		internal static Shader cloud = Resources.Load<Shader>("Shaders/fx/hgcloudremap");

		internal static Material commandoMat;

		private static string[] assetNames = new string[0];

		private const string assetbundleName = "monstermodassets";

		internal static void Initialize()
		{
			LoadAssetBundle();
			LoadSoundbank();
			PopulateAssets();
		}

		internal static void LoadAssetBundle()
		{
			if (mainAssetBundle == null)
			{
				using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiftTitansMod.monstermodassets");
				mainAssetBundle = AssetBundle.LoadFromStream(stream);
			}
			assetNames = mainAssetBundle.GetAllAssetNames();
		}

		internal static void LoadSoundbank()
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiftTitansMod.ReksaiSoundbank.bnk");
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			SoundAPI.SoundBanks.Add(array);
		}

		internal static void PopulateAssets()
		{
			if (!mainAssetBundle)
			{
				Debug.LogError("There is no AssetBundle to load assets from.");
				return;
			}
			PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/effects/impacteffects/BeetleGuardGroundSlam"), "_", true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Assets.cs", "PopulateAssets", 94).transform.Find("ParticleInitial").Find("Decal");
			reksaiUnburrowEffect = LoadEffect("ReksaiUnburrowEffect", parentToTransform: true);
			reksaiUnburrowEffect.AddComponent<AlignToNormal>();
			reksaiUnburrowEffect.AddComponent<ShakeEmitter>();
			ConvertAllRenderersToHopooShader(reksaiUnburrowEffect);
			reksaiBurrowEffect = LoadEffect("ReksaiBurrowEffect", parentToTransform: true);
			reksaiBurrowEffect.AddComponent<AlignToNormal>();
			reksaiBurrowEffect.AddComponent<ShakeEmitter>();
			ConvertAllRenderersToHopooShader(reksaiBurrowEffect);
			reksaiAttackEffect = LoadEffect("ReksaiAttackEffect", parentToTransform: true);
			reksaiSpecialEffect = LoadEffect("ReksaiSpecialEffect", parentToTransform: true);
			reksaiYellEffect = LoadEffect("ReksaiScreamEffect", parentToTransform: true);
			baronYellEffect = LoadEffect("BaronScreamEffect", parentToTransform: true);
			baronYellEffect.AddComponent<ShakeEmitter>();
			ShakeEmitter shakeEmitter = baronYellEffect.AddComponent<ShakeEmitter>();
			shakeEmitter.duration = 2.2f;
			shakeEmitter.radius = 200f;
			shakeEmitter.wave.amplitude = 0.9f;
			shakeEmitter.wave.frequency = 270f;
			baronSpitEffect = LoadEffect("BaronSpitEffect", parentToTransform: true);
			baronSpitImpactSound = CreateNetworkSoundEventDef("BaronSpitHit");
			baronTentacleEffect = LoadEffect("BaronTentacleEffect", parentToTransform: true);
			ShakeEmitter shakeEmitter2 = baronTentacleEffect.AddComponent<ShakeEmitter>();
			shakeEmitter.duration = 1f;
			shakeEmitter.radius = 40f;
			shakeEmitter.wave.amplitude = 0.6f;
			shakeEmitter.wave.frequency = 180f;
			ConvertAllRenderersToHopooShader(baronTentacleEffect);
			baronTentacleSound = CreateNetworkSoundEventDef("BaronTentacle");
			baronLineChargeEffect = LoadEffect("BaronLineChargeEffect", parentToTransform: true);
			baronGroundChargeEffect = LoadEffect("BaronGroundChargeEffect", parentToTransform: true);
			baronGroundChargeEffect.AddComponent<DestroyOnParticleEnd>();
			ShakeEmitter shakeEmitter3 = baronGroundChargeEffect.AddComponent<ShakeEmitter>();
			shakeEmitter.duration = 4f;
			shakeEmitter.radius = 200f;
			shakeEmitter.wave.amplitude = 0.5f;
			shakeEmitter.wave.frequency = 100f;
			baronChargeEffect = LoadEffect("BaronChargeEffect", parentToTransform: true);
			baronSpitConeEffect = LoadEffect("BaronSpitConeEffect", parentToTransform: true);
			heraldChargeChargeEffect = LoadEffect("HeraldChargeChargeEffect", parentToTransform: true);
			heraldCrashEffect = LoadEffect("HeraldCrashEffect", parentToTransform: true);
			heraldCrashEffect.AddComponent<ShakeEmitter>();
			heraldCrashEffect.AddComponent<AlignToNormal>();
			ConvertAllRenderersToHopooShader(heraldCrashEffect);
			heraldSlamEffect = LoadEffect("HeraldSlamEffect", parentToTransform: true);
			heraldSlamEffect.AddComponent<ShakeEmitter>();
			heraldSlamEffect.AddComponent<AlignToNormal>();
			ConvertAllRenderersToHopooShader(heraldSlamEffect);
			blueSlamEffect = LoadEffect("BlueSlamEffect");
			blueSlamEffect.AddComponent<AlignToNormal>();
			heraldSlamEffect.AddComponent<ShakeEmitter>();
			ConvertAllRenderersToHopooShader(blueSlamEffect);
		}

		private static GameObject CreateTracer(string originalTracerName, string newTracerName)
		{
			if (Resources.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null)
			{
				return null;
			}
			GameObject gameObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true, "C:\\Users\\natep\\source\\repos\\RiftTitansMod\\RiftTitansMod\\Modules\\Assets.cs", "CreateTracer", 165);
			if (!gameObject.GetComponent<EffectComponent>())
			{
				gameObject.AddComponent<EffectComponent>();
			}
			if (!gameObject.GetComponent<VFXAttributes>())
			{
				gameObject.AddComponent<VFXAttributes>();
			}
			if (!(Object)(object)gameObject.GetComponent<NetworkIdentity>())
			{
				gameObject.AddComponent<NetworkIdentity>();
			}
			gameObject.GetComponent<Tracer>().speed = 250f;
			gameObject.GetComponent<Tracer>().length = 50f;
			AddNewEffectDef(gameObject);
			return gameObject;
		}

		internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
		{
			NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
			networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
			networkSoundEventDef.eventName = eventName;
			networkSoundEventDefs.Add(networkSoundEventDef);
			return networkSoundEventDef;
		}

		internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
		{
			if (!objectToConvert)
			{
				return;
			}
			MeshRenderer[] componentsInChildren = objectToConvert.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if ((bool)meshRenderer && (bool)meshRenderer.material)
				{
					meshRenderer.material.shader = hotpoo;
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
			{
				if ((bool)skinnedMeshRenderer && (bool)skinnedMeshRenderer.material)
				{
					skinnedMeshRenderer.material.shader = hotpoo;
				}
			}
		}

		internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
		{
			MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
			CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				array[i] = new CharacterModel.RendererInfo
				{
					defaultMaterial = componentsInChildren[i].material,
					renderer = componentsInChildren[i],
					defaultShadowCastingMode = ShadowCastingMode.On,
					ignoreOverlays = false
				};
			}
			return array;
		}

		internal static Texture LoadCharacterIcon(string characterName)
		{
			return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
		}

		internal static GameObject LoadCrosshair(string crosshairName)
		{
			if (Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null)
			{
				return Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
			}
			return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
		}

		private static GameObject LoadEffect(string resourceName)
		{
			return LoadEffect(resourceName, "", parentToTransform: false);
		}

		private static GameObject LoadEffect(string resourceName, string soundName)
		{
			return LoadEffect(resourceName, soundName, parentToTransform: false);
		}

		private static GameObject LoadEffect(string resourceName, bool parentToTransform)
		{
			return LoadEffect(resourceName, "", parentToTransform);
		}

		private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
		{
			bool flag = false;
			for (int i = 0; i < assetNames.Length; i++)
			{
				if (assetNames[i].Contains(resourceName.ToLower()))
				{
					flag = true;
					i = assetNames.Length;
				}
			}
			if (!flag)
			{
				Debug.LogError("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
				return null;
			}
			GameObject gameObject = mainAssetBundle.LoadAsset<GameObject>(resourceName);
			gameObject.AddComponent<DestroyOnTimer>().duration = 12f;
			gameObject.AddComponent<NetworkIdentity>();
			gameObject.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
			EffectComponent effectComponent = gameObject.AddComponent<EffectComponent>();
			effectComponent.applyScale = false;
			effectComponent.effectIndex = EffectIndex.Invalid;
			effectComponent.parentToReferencedTransform = parentToTransform;
			effectComponent.positionAtReferencedTransform = true;
			effectComponent.soundName = soundName;
			AddNewEffectDef(gameObject, soundName);
			return gameObject;
		}

		private static void AddNewEffectDef(GameObject effectPrefab)
		{
			AddNewEffectDef(effectPrefab, "");
		}

		private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
		{
			EffectDef effectDef = new EffectDef();
			effectDef.prefab = effectPrefab;
			effectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
			effectDef.prefabName = effectPrefab.name;
			effectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
			effectDef.spawnSoundEventName = soundName;
			effectDefs.Add(effectDef);
		}

		public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
		{
			if (!commandoMat)
			{
				commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;
			}
			Material material = Object.Instantiate(commandoMat);
			Material material2 = mainAssetBundle.LoadAsset<Material>(materialName);
			if (!material2)
			{
				Debug.LogError("Failed to load material: " + materialName + " - Check to see that the name in your Unity project matches the one in this code");
				return commandoMat;
			}
			material.name = materialName;
			material.SetColor("_Color", material2.GetColor("_Color"));
			material.SetTexture("_MainTex", material2.GetTexture("_MainTex"));
			material.SetColor("_EmColor", emissionColor);
			material.SetFloat("_EmPower", emission);
			material.SetTexture("_EmTex", material2.GetTexture("_EmissionMap"));
			material.SetFloat("_NormalStrength", normalStrength);
			return material;
		}

		public static Material CreateMaterial(string materialName)
		{
			return CreateMaterial(materialName, 0f);
		}

		public static Material CreateMaterial(string materialName, float emission)
		{
			return CreateMaterial(materialName, emission, Color.white);
		}

		public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
		{
			return CreateMaterial(materialName, emission, emissionColor, 0f);
		}
	}
}
