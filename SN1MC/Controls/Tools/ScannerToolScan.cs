
using HarmonyLib;
using UnityEngine;
//Aim scanner tool with the vr controller
namespace SN1MC.Controls.Tools
{
    class ScannerToolScan
    {
		[HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.UpdateTarget))]
		public static class PDAScanner_UpdateTarget_Patch
		{
			[HarmonyPrefix]
			static bool Prefix(float distance, bool self)
			{
				PDAScanner.ScanTarget scanTarget = default(PDAScanner.ScanTarget);
				scanTarget.Invalidate();
				GameObject candidate;
				if (self)
				{
					candidate = ((Player.main != null) ? Player.main.gameObject : null);
				}
				else
				{
					CustomTargeting.AddToIgnoreList(Player.main.gameObject);
					float num;
					CustomTargeting.GetTarget(distance, out candidate, out num);
				}
				scanTarget.Initialize(candidate);
				if (PDAScanner.scanTarget.techType != scanTarget.techType || PDAScanner.scanTarget.gameObject != scanTarget.gameObject || PDAScanner.scanTarget.uid != scanTarget.uid)
				{
					if (PDAScanner.scanTarget.isPlayer && PDAScanner.scanTarget.hasUID && PDAScanner.cachedProgress.ContainsKey(PDAScanner.scanTarget.uid))
					{
						PDAScanner.cachedProgress[PDAScanner.scanTarget.uid] = 0f;
					}
					float progress;
					if (scanTarget.hasUID && PDAScanner.cachedProgress.TryGetValue(scanTarget.uid, out progress))
					{
						scanTarget.progress = progress;
					}
					PDAScanner.scanTarget = scanTarget;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnAltDown))]
		public static class ScannerTool_OnAltDown_Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result, ScannerTool __instance)
			{
				__instance.Scan();
				__result = true;
				return false;
			}
		}

		[HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnAltHeld))]
		public static class ScannerTool_OnAltHeld_Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result, ScannerTool __instance)
			{
				__instance.Scan();
				__result = true;
				return false;

			}
		}
	}
}
