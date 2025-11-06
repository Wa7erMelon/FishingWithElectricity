using Duckov.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishingWithElectricity
{
	public static class Extension
	{
		public static string MyToString(this TagCollection tagCollection)
		{
			string text = "";
			foreach (var tag in tagCollection.list)
			{
				text += $"{tag.name}({tag.GetInstanceID()})";
			}
			return text;
		}
	}
}
