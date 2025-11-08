using Nett;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Debug = UnityEngine.Debug;

namespace FishingWithElectricity
{
	public static class ConfigUtil
	{
		private const string ModFolderName = @"Mods\FishingWithElectricity";
		private const string DefaultConfigResourceName = "DefaultConfig.toml";

		public static List<DamageConfig> GetDamageConfig()
		{
			var str = GetDefaultConfigString();
			var damageInfos = Toml.ReadString(str).Get<TomlTableArray>("DamageInfo")
				.Items.Select(item => item.Get<DamageConfig>()).ToList();
			return damageInfos;
		}

		private static string GetDefaultConfigString()
		{
			try
			{
				var assembly = Assembly.GetExecutingAssembly();
				var resourceName = assembly.GetManifestResourceNames()
					.FirstOrDefault(n => n.EndsWith(DefaultConfigResourceName));

				if (resourceName == null)
					return GetMinimalTemplate();

				using var stream = assembly.GetManifestResourceStream(resourceName);
				using var reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
			catch (Exception ex)
			{
				Debug.LogError($"[FwE][Config] 读取内置资源失败：{ex}");
				return GetMinimalTemplate();
			}
		}

		private static string GetMinimalTemplate()
		{
			return "[[DamageInfo]]\nDamageValue = 50\nDamageRate = 1\nText = \"我不可能告诉你任何事情啊啊啊啊啊啊啊\"";
		}
	}
}