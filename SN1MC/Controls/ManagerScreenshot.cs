
using HarmonyLib;

namespace SN1MC.Controls
{
    class ManagerScreenshot
    {
		[HarmonyPatch(typeof(ScreenshotManager), nameof(ScreenshotManager.LateUpdate))]
		public static class GameInput_GetButtonUp__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ScreenshotManager __instance)
			{
				if (SN1MC.UsingSteamVR)
				{
					if (GameInput.GetButtonDown(GameInput.Button.TakePicture) && !WaitScreen.IsWaiting && !SaveLoadManager.main.isSaving)
					{
						__instance.TakeScreenshot();
					}
				}
				else
                {
					if (!GameInput.GetButtonHeld(GameInput.Button.MoveDown) && !GameInput.GetButtonHeld(GameInput.Button.MoveUp))
					{
						if (GameInput.GetButtonDown(GameInput.Button.TakePicture) && !WaitScreen.IsWaiting && !SaveLoadManager.main.isSaving)
						{
							__instance.TakeScreenshot();
						}
					}
				}
				return false;
			}
		}
	}
}
