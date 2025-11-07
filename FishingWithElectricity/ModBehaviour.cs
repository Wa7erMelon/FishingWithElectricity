using Duckov.UI;
using Duckov.Utilities;
using FMOD;
using FMODUnity;
using HarmonyLib;
using ItemStatsSystem;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static Duckov.Utilities.GameplayDataSettings;
using Debug = UnityEngine.Debug;

namespace FishingWithElectricity
{
	public class ModBehaviour : Duckov.Modding.ModBehaviour
	{
		private const string Id = "FishingWithElectricity";

		private static ChannelGroup sfxGroup;
		public static ChannelGroup SfxGroup
		{
			get
			{
				if (!sfxGroup.hasHandle())
				{
					RESULT result = RuntimeManager.GetBus("bus:/Master/SFX").getChannelGroup(out sfxGroup);
					if (result != RESULT.OK)
					{
						Console.WriteLine("FMOD failed to get sfx group: " + result);
						Debug.LogError("FMOD failed to get sfx group: " + result);
					}
				}
				return sfxGroup;
			}
		}

		private static Sound electricitySound;

		public Harmony harmony;

		private void Awake()
		{
			Debug.Log("FishingWithElectricity Load");
		}
		private void OnEnable()
		{
			HarmonyModLoad.LoadHarmony();
			string soundPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Electricity.mp3"); ;
			if (File.Exists(soundPath))
			{
				var soundResult = RuntimeManager.CoreSystem.createSound(soundPath, MODE.LOOP_OFF, out electricitySound);
				if (soundResult != RESULT.OK)
				{
					Debug.LogError("FMOD failed to create sound: " + soundResult + "\n");
				}
			}
		}

		private void Start()
		{
			harmony = new Harmony(Id);
			harmony.PatchAll();
		}
		private void OnDisable()
		{
			harmony.UnpatchAll(Id);

			electricitySound.release();
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
					var fishDict = new List<Item>();

					for (int i = 0; i < 10; i++)
					{
						Item item = __instance.lootSpawner.Spawn(___baitItem.TypeID, ___luck).GetAwaiter().GetResult();
						fishDict.Add(item);
					}

					var notify = __instance.gotFishText.ToPlainText() + " " +
									string.Join(" , ", fishDict.GroupBy(x => x.DisplayName)
										.Select(x => $"{x.Key} x {x.Count()}")) + " !";
					foreach (var item in fishDict)
					{
						__instance.characterController.PickupItem(item);
					}

					NotificationText.Push(notify);
					RESULT result = RuntimeManager.CoreSystem.playSound(electricitySound, SfxGroup, false, out _);
					if (!RESULT.OK.Equals(result))
					{
						Debug.LogError("FMOD failed to play sound: " + result + "\n");
					}


					return false;
				}

				return true;
			}
		}
	}
}
