
using HarmonyLib;
using UnityEngine;

namespace SN1MC
{
    class InGameMenu
    {
		[HarmonyPatch(typeof(IngameMenuPanel), nameof(IngameMenuPanel.Update))]
		public static class IngameMenuPanel_Update__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(IngameMenuPanel __instance)
			{
				//ErrorMessage.AddDebug("Open: " + __instance.tabbedPanel.dialog.open);
				if (__instance.tabbedPanel != null && Input.GetKeyDown(KeyCode.Escape))
				{
					if (__instance.tabbedPanel.dialog.open)
					{
						__instance.tabbedPanel.dialog.Close();
						return false;
					}
					__instance.OnBack();
				}
				if(GameInput.GetButtonDown(GameInput.Button.UICancel))
                {
					__instance.OnBack();
                }
				return false;
			}
		}
	}
}
