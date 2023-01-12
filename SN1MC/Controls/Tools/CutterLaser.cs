
using HarmonyLib;
using UnityEngine;
//Makes it so laser cutter is aimed with the vr controller
namespace SN1MC.Controls.Tools
{
    class CutterLaser
    {
		[HarmonyPatch(typeof(LaserCutter), nameof(LaserCutter.UpdateTarget))]
		public static class LaserCutter_UpdateTarget__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(LaserCutter __instance)
			{
				__instance.activeCuttingTarget = null;
				if (__instance.usingPlayer != null)
				{
					Vector3 vector = default(Vector3);
					GameObject gameObject = null;
					CustomTargeting.TraceFPSTargetPosition(Player.main.gameObject, 2f, ref gameObject, ref vector, true);
					if (gameObject == null)
					{
						InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
						if (component != null && component.GetMostRecent() != null)
						{
							gameObject = component.GetMostRecent().gameObject;
						}
					}
					if (gameObject)
					{
						Sealed exists = gameObject.FindAncestor<Sealed>();
						if (exists)
						{
							__instance.activeCuttingTarget = exists;
						}
					}
				}
				return false;
			}
		}

	}
}
