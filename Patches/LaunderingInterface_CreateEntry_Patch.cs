using System;
using HarmonyLib;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.UI;

namespace MoreRealisticLaundering.Patches
{
	[HarmonyPatch(typeof(LaunderingInterface), "CreateEntry")]
	public class LaunderingInterface_CreateEntry_Patch
	{
		public static void Prefix(LaunderingOperation op)
		{
			bool flag = ((op != null) ? op.business : null) == null;
			if (!flag)
			{
				string text;
				bool flag2 = MRLCore.Instance.aliasMap.TryGetValue(op.business.name, out text);
				if (flag2)
				{
					MRLCore.Instance.MaybeBoost(op);
				}
			}
		}
	}
}
