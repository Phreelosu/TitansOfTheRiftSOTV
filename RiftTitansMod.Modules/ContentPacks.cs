using System.Collections;
using RoR2.ContentManagement;

namespace RiftTitansMod.Modules {

	internal class ContentPacks : IContentPackProvider
	{
		internal ContentPack contentPack = new ContentPack();

		public string identifier => "com.ndp.RiftTitans";

		public void Initialize()
		{
			ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
		}

		private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
		{
			addContentPackProvider((IContentPackProvider)(object)this);
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
		{
			contentPack.identifier = identifier;
			contentPack.bodyPrefabs.Add(Prefabs.bodyPrefabs.ToArray());
			contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
			contentPack.effectDefs.Add(Assets.effectDefs.ToArray());
			contentPack.entityStateTypes.Add(States.entityStates.ToArray());
			contentPack.masterPrefabs.Add(Prefabs.masterPrefabs.ToArray());
			contentPack.networkSoundEventDefs.Add(Assets.networkSoundEventDefs.ToArray());
			contentPack.projectilePrefabs.Add(Prefabs.projectilePrefabs.ToArray());
			contentPack.skillDefs.Add(Skills.skillDefs.ToArray());
			contentPack.skillFamilies.Add(Skills.skillFamilies.ToArray());
			contentPack.survivorDefs.Add(Prefabs.survivorDefinitions.ToArray());
			contentPack.unlockableDefs.Add(Unlockables.unlockableDefs.ToArray());
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
		{
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}
	}
}
