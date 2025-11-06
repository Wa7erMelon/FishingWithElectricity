using Duckov.Modding;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace FishingWithElectricity
{
	public class HarmonyModLoad
	{
		public static Assembly LoadHarmony()
		{
			_ = Assembly.GetExecutingAssembly().Location;
			string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "0Harmony.dll");
			Debug.Log((object)text);
			if (!File.Exists(text))
			{
				throw new FileNotFoundException("找不到 0Harmony.dll", text);
			}
			Assembly assembly = Assembly.LoadFrom(text);
			Console.WriteLine("已加载: " + assembly.FullName);
			return assembly;
		}
	}
}
