using BepInEx;
using BepInEx.Configuration;
using HG;
using R2API;
using R2API.Utils;
using RiftTitansMod.Modules;
using RiftTitansMod.Modules.Survivors;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RiftTitansMod {

	[BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	[BepInPlugin("com.phreel.TitansOfTheRiftSOTV", "TitansoftheRiftSOTV", "0.2.0")]
	[R2APISubmoduleDependency(nameof(DirectorAPI), nameof(PrefabAPI), nameof(LanguageAPI), nameof(SoundAPI))]
	public class RiftTitansPlugin : BaseUnityPlugin
	{
		public const string MODUID = "com.Phreel.TitansOfTheRiftSOTV";

		public const string MODNAME = "TitansoftheRiftSOTV";

		public const string MODVERSION = "0.2.0";

		public const string developerPrefix = "Phreel";

		public static DirectorAPI.DirectorCardHolder BaronCard;
		public static DirectorAPI.DirectorCardHolder BaronLoopCard;
		public static DirectorAPI.DirectorCardHolder BlueCard;
		public static DirectorAPI.DirectorCardHolder BlueLoopCard;
		public static DirectorAPI.DirectorCardHolder ChickenCard;
		public static DirectorAPI.DirectorCardHolder ChickenLoopCard;
		public static DirectorAPI.DirectorCardHolder HeraldCard;
		public static DirectorAPI.DirectorCardHolder HeraldLoopCard;
		public static DirectorAPI.DirectorCardHolder ReksaiCard;
		public static DirectorAPI.DirectorCardHolder ReksaiLoopCard;

		public static List<StageSpawnInfo> StageList = new List<StageSpawnInfo>();

		public static RiftTitansPlugin instance;

		private void Awake()
		{
			instance = this;
			Files.PluginInfo = this.Info;
			Assets.Initialize();
			ReadConfig();
			States.RegisterStates();
			Buffs.RegisterBuffs();
			Projectiles.RegisterProjectiles();
			Tokens.AddTokens();
			ItemDisplays.PopulateDisplays();
			Baron.CreateCharacter();
			Reksai.CreateCharacter();
			Blue.CreateCharacter();
			Chicken.CreateCharacter();
			Herald.CreateCharacter();
			new ContentPacks().Initialize();
			ContentManager.onContentPacksAssigned += LateSetup;
			Hook();
		}

		private void Start()
        {
			SoundBanks.Init();
        }

		private void LateSetup(ReadOnlyArray<ReadOnlyContentPack> obj)
		{
			Baron.SetItemDisplays();
			Herald.SetItemDisplays();
			Reksai.SetItemDisplays();
			Chicken.SetItemDisplays();
			Blue.SetItemDisplays();
		}

		public void ReadConfig()
		{
			string stages = base.Config.Bind<string>(new ConfigDefinition("Spawns", "Stage List"), "ancientloft, arena, blackbeach, blackbeach2, dampcavesimple, foggyswamp, frozenwall, golemplains, golemplains2, goolake, itancientloft, itdampcave, itfrozenwall, itgolemplains, itgoolake, itskymeadow, rootjungle, shipgraveyard, skymeadow, snowyforest, sulfurpools, voidstage, wispgraveyard, drybasin, slumberingsatellite, FBLScene, artifactworld, goldshores", new ConfigDescription("What stages the monster will show up on. Add a '- loop' after the stagename to make it only spawn after looping. List of stage names can be found at https://github.com/risk-of-thunder/R2Wiki/wiki/List-of-scene-names")).Value;
			//string gwRemoveStages = base.Config.Bind<string>(new ConfigDefinition("Spawns", "Remove Greater Wisps"), "goldshores, dampcavesimple, itdampcave, sulfurpools, skymeadow, itskymeadow", new ConfigDescription("Remove Greater Wisps from these stages to prevent role overlap.")).Value;

			//parse stage
			stages = new string(stages.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
			string[] splitStages = stages.Split(',');
			foreach (string str in splitStages)
			{
				string[] current = str.Split('-');

				string name = current[0];
				int minStages = 0;
				if (current.Length > 1)
				{
					minStages = 5;
				}

				StageList.Add(new StageSpawnInfo(name, minStages));
			}

			/*parse removeGW
			gwRemoveStages = new string(gwRemoveStages.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
			string[] splitimpRemoveStages = stages.Split(',');
			foreach (string str in splitimpRemoveStages)
			{
				string[] current = str.Split('-');  //in case people try to use the Stage List format

				string name = current[0];

				SceneDef sd = ScriptableObject.CreateInstance<SceneDef>();
				sd.baseSceneNameOverride = name;

				DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.GreaterWisp, DirectorAPI.GetStageEnumFromSceneDef(sd), name);
			}*/
		}
		public class StageSpawnInfo
		{
			private string stageName;
			private int minStages;

			public StageSpawnInfo(string stageName, int minStages)
			{
				this.stageName = stageName;
				this.minStages = minStages;
			}

			public string GetStageName() { return stageName; }
			public int GetMinStages() { return minStages; }
		}

		private void Hook()
		{
		}
	}
}
