using Cysharp.Threading.Tasks;
using Duckov.UI;
using FMOD;
using FMODUnity;
using HarmonyLib;
using ItemStatsSystem;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace FishingWithElectricity
{
	[HarmonyPatch(typeof(Action_FishingV2))]
	public class Action_FishingV2Patch
	{
		private static List<DamageConfig>? _damageInfos;

		public static List<DamageConfig> DamageInfos
		{
			get
			{
				_damageInfos ??= ConfigUtil.GetDamageConfig();
				return _damageInfos;
			}
		}

		private static ChannelGroup _sfxGroup;
		public static ChannelGroup SfxGroup
		{
			get
			{
				if (!_sfxGroup.hasHandle())
				{
					RESULT result = RuntimeManager.GetBus("bus:/Master/SFX").getChannelGroup(out _sfxGroup);
					if (result != RESULT.OK)
					{
						Console.WriteLine("FMOD failed to get sfx group: " + result);
						Debug.LogError("FMOD failed to get sfx group: " + result);
					}
				}
				return _sfxGroup;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("CatchFish")]
		private static bool CatchFishPatch(Action_FishingV2 __instance, Item ___baitItem, float ___luck)
		{
			if (___baitItem.TypeID.Equals(942))
			{
				CatchFish(__instance, ___baitItem, ___luck).Forget();
				return false;
			}

			return true;
		}

		private static async UniTaskVoid CatchFish(Action_FishingV2 __instance, Item ___baitItem, float ___luck)
		{
			if (___baitItem.TypeID.Equals(942))
			{
				var fishList = new List<Item>();

				try
				{
					var items = await UniTask.WhenAll(
						Enumerable.Range(0, 10)
							.Select(_ => __instance.lootSpawner.Spawn(___baitItem.TypeID, ___luck))
					);
					fishList.AddRange(items);
				}
				catch (Exception ex)
				{
					Debug.LogError($"Spawn failed: {ex.Message}");
				}

				var notify = __instance.gotFishText.ToPlainText() + " " +
							 string.Join(" , ", fishList.GroupBy(x => x.DisplayName)
								 .Select(x => $"{x.Key} x {x.Count()}")) + " !";
				foreach (var item in fishList)
				{
					__instance.characterController.PickupItem(item);
				}

				NotificationText.Push(notify);
				RESULT result = RuntimeManager.CoreSystem.playSound(ModBehaviour.electricitySound, SfxGroup, false, out _);
				if (!RESULT.OK.Equals(result))
				{
					Debug.LogError("FMOD failed to play sound: " + result + "\n");
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch("TransToFailBack")]
		private static void TransToFailBackPatch(Action_FishingV2 __instance)
		{
			var (text, damageInfo) = DamageSelector.GetRandomDamageInfo();
			__instance.characterController.PopText(text);
			__instance.characterController.Health.Hurt(damageInfo);
		}
	}
}