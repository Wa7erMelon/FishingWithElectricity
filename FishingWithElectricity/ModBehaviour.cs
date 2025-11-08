using FMOD;
using FMODUnity;
using HarmonyLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;

namespace FishingWithElectricity
{
	public class ModBehaviour : Duckov.Modding.ModBehaviour, INotifyPropertyChanged
	{

		private EnemyCreator? _enemyCreator;

		public EnemyCreator EnemyCreator
		{
			get
			{
				_enemyCreator ??= new EnemyCreator(this);
				return _enemyCreator;
			}
		}

		private static readonly List<string> dllList = new List<string>()
		{
			"0Harmony.dll",
			"Nett.dll"
		};

		private const string Id = "FishingWithElectricity";

		internal static Sound electricitySound;

		public Harmony? harmony;

		private void Awake()
		{
			Debug.Log("FishingWithElectricity Load");
		}
		private void OnEnable()
		{
			Initialize();
		}

		private void Start()
		{
		}
		private void OnDisable()
		{
			harmony?.UnpatchAll(Id);
			electricitySound.release();
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private void Initialize()
		{
			harmony = new Harmony(Id);
			harmony.PatchAll();
			DamageSelector.Initialize(Action_FishingV2Patch.DamageInfos);
			dllList.ForEach(asm => AssemblyLoad.LoadDll(asm));

			string soundPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Electricity.mp3"); ;
			if (File.Exists(soundPath))
			{
				var soundResult = RuntimeManager.CoreSystem.createSound(soundPath, MODE.LOOP_OFF, out electricitySound);
				if (soundResult != RESULT.OK)
				{
					Debug.LogError("FMOD failed to create sound: " + soundResult + "\n");
				}
			}
		}
	}
}
