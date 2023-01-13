
using HarmonyLib;

namespace SN1MC.Controls
{
	extern alias SteamVRRef;
	class QuickSlotButtons
    {
		[HarmonyPatch(typeof(uGUI_QuickSlots), nameof(uGUI_QuickSlots.HandleInput))]
		public static class uGUI_QuickSlots_HandleInput__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(uGUI_QuickSlots __instance)
			{
				VRInputManager vrInput = new VRInputManager();
				Player main = Player.main;
				if (!main.GetCanItemBeUsed())
				{
					return false;
				}
				bool flag = uGUI.isIntro || IntroLifepodDirector.IsActive;
				if (!flag)
				{
					int i = 0;
					int quickSlotButtonsCount = Player.quickSlotButtonsCount;
					while (i < quickSlotButtonsCount)
					{
						if (main.GetQuickSlotKeyDown(i))
						{
							__instance.target.SlotKeyDown(i);
						}
						else if (main.GetQuickSlotKeyHeld(i))
						{
							__instance.target.SlotKeyHeld(i);
						}
						if (main.GetQuickSlotKeyUp(i))
						{
							__instance.target.SlotKeyUp(i);
						}
						i++;
					}
					if (SN1MC.UsingSteamVR)
					{
						if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
						{
							__instance.target.SlotNext();
						}
						else if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
						{
							__instance.target.SlotPrevious();
						}
					}
					else
                    {
						if (GameInput.GetButtonHeld(GameInput.Button.MoveDown) && GameInput.GetButtonHeld(GameInput.Button.MoveUp))
						{
							if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
							{
								__instance.target.SlotNext();
							}
							else if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
							{
								__instance.target.SlotPrevious();
							}
						}
					}
				}
				if (AvatarInputHandler.main != null && AvatarInputHandler.main.IsEnabled())
				{
					if (main.GetLeftHandDown())
					{
						__instance.target.SlotLeftDown();
					}
					else if (main.GetLeftHandHeld())
					{
						__instance.target.SlotLeftHeld();
					}
					if (main.GetLeftHandUp())
					{
						__instance.target.SlotLeftUp();
					}
					if (main.GetRightHandDown())
					{
						__instance.target.SlotRightDown();
					}
					else if (main.GetRightHandHeld())
					{
						__instance.target.SlotRightHeld();
					}
					if (main.GetRightHandUp())
					{
						__instance.target.SlotRightUp();
					}
					if (!flag && GameInput.GetButtonDown(GameInput.Button.Exit))
					{
						__instance.target.DeselectSlots();
					}
				}
				return false;
			}
		}
	}
}
