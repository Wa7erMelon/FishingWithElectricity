using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using System.Linq;
using static Duckov.Utilities.GameplayDataSettings;

namespace FishingWithElectricity
{
	[HarmonyPatch(typeof(Item))]
	public class ItemPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("get_Tags")]
		private static void AddBaitTag(Item __instance, TagCollection __result)
		{
			if (__instance.TypeID.Equals(942))
			{
				if (!__result.list.Contains(Tags.Bait))
				{
					__result.list.Add(Tags.Bait);
				}
			}

			__result.list = __result.list.Distinct().ToList();
		}
	}
}