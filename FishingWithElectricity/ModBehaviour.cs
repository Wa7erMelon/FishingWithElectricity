using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using System.Linq;
using UnityEngine;
using static Duckov.Utilities.GameplayDataSettings;

namespace FishingWithElectricity
{
	public class ModBehaviour : Duckov.Modding.ModBehaviour
	{
		public Harmony harmony;

		private void Awake()
		{
			Debug.Log("FishingWithElectricity Load");
		}
		private void OnEnable()
		{
			HarmonyModLoad.LoadHarmony();
		}

		private void Start()
		{
			harmony = new Harmony("FishingWithElectricity");
			harmony.PatchAll();
		}
		private void OnDisable()
		{
			harmony.UnpatchAll("FishingWithElectricity");
		}

		[HarmonyPatch(typeof(Item))]
		public static class ItemPatch
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

		[HarmonyPatch(typeof(Action_FishingV2))]
		public static class Action_FishingV2Patch
		{
			[HarmonyPrefix]
			[HarmonyPatch("CatchFish")]
			private static bool CatchFishPatch(Action_FishingV2 __instance, Item ___baitItem, float ___luck)
			{
				if (___baitItem.TypeID.Equals(942))
				{
					for (int i = 0; i < 9; i++)
					{
						Item item = __instance.lootSpawner.Spawn(___baitItem.TypeID, ___luck).GetAwaiter().GetResult();
						__instance.characterController.PickupItem(item);
					}
				}

				return true;
			}
		}
	}
}
