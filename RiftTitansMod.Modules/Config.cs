using BepInEx.Configuration;

namespace RiftTitansMod.Modules {

	public static class Config
	{
		public static void ReadConfig()
		{
		}

		internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
		{
			return RiftTitansPlugin.instance.Config.Bind(new ConfigDefinition(characterName, "Enabled"), defaultValue: true, new ConfigDescription("Set to false to disable this character", null));
		}

		internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
		{
			return RiftTitansPlugin.instance.Config.Bind(new ConfigDefinition(characterName, "Enabled"), defaultValue: true, new ConfigDescription("Set to false to disable this enemy", null));
		}
	}
}
