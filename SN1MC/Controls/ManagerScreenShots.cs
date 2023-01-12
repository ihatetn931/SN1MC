
using HarmonyLib;

namespace SN1MC.Controls
{
    class ManagerScreenShots
    {
		[HarmonyPatch(typeof(ScreenshotManager), nameof(ScreenshotManager.LateUpdate))]
		public static class ScreenshotManager_LateUpdate__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ScreenshotManager __instance)
			{
				if (GameInput.GetButtonDown(GameInput.Button.TakePicture) && !WaitScreen.IsWaiting && !SaveLoadManager.main.isSaving)
				{
					__instance.TakeScreenshot();
				}
				return false;
			}
		}
	}
}
