using System;
using System.IO;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace FishingWithElectricity
{
	public class AssemblyLoad
	{

		public static Assembly LoadDll(string dllName)
		{
			string dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, dllName);
			Debug.Log($"尝试加载 DLL: {dllPath}");

			if (!File.Exists(dllPath))
			{
				throw new FileNotFoundException($"找不到 {dllName}", dllPath);
			}

			var assembly = Assembly.LoadFrom(dllPath);
			Console.WriteLine($"已加载: {assembly.FullName}");
			return assembly;
		}
	}
}
