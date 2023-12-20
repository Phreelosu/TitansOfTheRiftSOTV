/*using System;
using On.RoR2;
using RoR2;
using UnityEngine;

namespace RiftTitansMod.Modules {

	internal static class Skins
	{
		internal struct SkinDefInfo
		{
			internal RoR2.SkinDef[] BaseSkins;

			internal Sprite Icon;

			internal string NameToken;

			internal RoR2.UnlockableDef UnlockableDef;

			internal GameObject RootObject;

			internal RoR2.CharacterModel.RendererInfo[] RendererInfos;

			internal RoR2.SkinDef.MeshReplacement[] MeshReplacements;

			internal RoR2.SkinDef.GameObjectActivation[] GameObjectActivations;

			internal RoR2.SkinDef.ProjectileGhostReplacement[] ProjectileGhostReplacements;

			internal RoR2.SkinDef.MinionSkinReplacement[] MinionSkinReplacements;

			internal string Name;
		}

		internal static RoR2.SkinDef CreateSkinDef(string skinName, Sprite skinIcon, RoR2.CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root)
		{
			return CreateSkinDef(skinName, skinIcon, rendererInfos, mainRenderer, root, null);
		}

		internal static RoR2.SkinDef CreateSkinDef(string skinName, Sprite skinIcon, RoR2.CharacterModel.RendererInfo[] rendererInfos, SkinnedMeshRenderer mainRenderer, GameObject root, RoR2.UnlockableDef unlockableDef)
		{
			SkinDefInfo skinDefInfo = default(SkinDefInfo);
			skinDefInfo.BaseSkins = Array.Empty<RoR2.SkinDef>();
			skinDefInfo.GameObjectActivations = new RoR2.SkinDef.GameObjectActivation[0];
			skinDefInfo.Icon = skinIcon;
			skinDefInfo.MeshReplacements = new RoR2.SkinDef.MeshReplacement[0];
			skinDefInfo.MinionSkinReplacements = new RoR2.SkinDef.MinionSkinReplacement[0];
			skinDefInfo.Name = skinName;
			skinDefInfo.NameToken = skinName;
			skinDefInfo.ProjectileGhostReplacements = new RoR2.SkinDef.ProjectileGhostReplacement[0];
			skinDefInfo.RendererInfos = rendererInfos;
			skinDefInfo.RootObject = root;
			skinDefInfo.UnlockableDef = unlockableDef;
			SkinDefInfo skinDefInfo2 = skinDefInfo;
			//RoR2.SkinDef asd = DoNothing(orig, self);
			RoR2.SkinDef skinDef = ScriptableObject.CreateInstance<RoR2.SkinDef>();
			skinDef.baseSkins = skinDefInfo2.BaseSkins;
			skinDef.icon = skinDefInfo2.Icon;
			skinDef.unlockableDef = skinDefInfo2.UnlockableDef;
			skinDef.rootObject = skinDefInfo2.RootObject;
			skinDef.rendererInfos = skinDefInfo2.RendererInfos;
			skinDef.gameObjectActivations = skinDefInfo2.GameObjectActivations;
			skinDef.meshReplacements = skinDefInfo2.MeshReplacements;
			skinDef.projectileGhostReplacements = skinDefInfo2.ProjectileGhostReplacements;
			skinDef.minionSkinReplacements = skinDefInfo2.MinionSkinReplacements;
			skinDef.nameToken = skinDefInfo2.NameToken;
			skinDef.name = skinDefInfo2.Name;
			//RoR2.SkinDef.Awake -= new hook_Awake(DoNothing);
			return skinDef;
		}

		/*private static void DoNothing(orig_Awake orig, RoR2.SkinDef self)
		{
            if (orig == null)
            {
				return;
            }
			return;
		}
	}
}
*/