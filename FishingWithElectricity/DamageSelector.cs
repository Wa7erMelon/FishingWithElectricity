using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace FishingWithElectricity
{
	public class DamageSelector
	{
		private static readonly Random random = new Random();
		private static List<DamageConfig> _damageInfos;
		private static int _totalWeight;
		private static bool _isInitialized;

		public static void Initialize(List<DamageConfig> damageInfos)
		{
			_damageInfos = damageInfos ?? throw new ArgumentNullException(nameof(damageInfos));
			_totalWeight = damageInfos.Sum(x => x.DamageRate);
			_isInitialized = true;
			Debug.Log(GetActualProbabilities());
		}

		public static DamageConfig SelectRandomDamage()
		{
			if (!_isInitialized)
				throw new InvalidOperationException("请先调用 Initialize 方法初始化配置");

			int randomNumber = random.Next(0, _totalWeight);
			int currentWeight = 0;

			foreach (var damageInfo in _damageInfos)
			{
				currentWeight += damageInfo.DamageRate;
				if (randomNumber < currentWeight)
					return damageInfo;
			}

			return _damageInfos.Last();
		}


		public static string GetActualProbabilities()
		{
			if (!_isInitialized)
				throw new InvalidOperationException("请先调用 Initialize 方法初始化配置");

			var probabilities = _damageInfos.ToDictionary(
				x => x,
				x => (double)x.DamageRate / _totalWeight * 100
			);

			return string.Join("\n", probabilities.Select(kvp =>
				$"{kvp.Key.Text}: {kvp.Value:F2}%"));
		}

		public static (string, DamageInfo) GetRandomDamageInfo()
		{
			var damageConfig = DamageSelector.SelectRandomDamage();
			var damageInfo = new DamageInfo()
			{
				damageValue = damageConfig.DamageValue,
				elementFactors = new List<ElementFactor>()
				{
					new ElementFactor()
					{
						elementType = ElementTypes.electricity,
						factor = 1f
					}
				},
				isExplosion = true
			};
			return (damageConfig.Text, damageInfo);
		}
	}
}