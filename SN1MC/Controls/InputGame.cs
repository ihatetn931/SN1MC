
using HarmonyLib;
using UnityEngine;
//This is for capturing the buttons for steam vr
namespace SN1MC.Controls
{
	extern alias SteamVRRef;
	class InputGame
	{
		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetButtonDown))]
		public static class GameInput_GetButtonDown__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameInput.Button button, ref bool __result)
			{
				if (SN1MC.UsingSteamVR)
				{
					VRInputManager vrInput = new VRInputManager();
					__result = vrInput.GetButtonDown(button, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				}
				else
				{
					__result = (GameInput.GetInputStateForButton(button).flags & GameInput.InputStateFlags.Down) > (GameInput.InputStateFlags)0U;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetButtonHeld))]
		public static class GameInput_GetButtonHeld__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameInput.Button button, ref bool __result)
			{
				if (SN1MC.UsingSteamVR)
				{
					VRInputManager vrInput = new VRInputManager();
					__result = vrInput.GetButton(button, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				}
				else
				{
					__result = (GameInput.GetInputStateForButton(button).flags & GameInput.InputStateFlags.Held) > (GameInput.InputStateFlags)0U;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetButtonUp))]
		public static class GameInput_GetButtonUp__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameInput.Button button, ref bool __result)
			{
				if (SN1MC.UsingSteamVR)
				{
					VRInputManager vrInput = new VRInputManager();
					__result = vrInput.GetButtonUp(button, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				}
				else
				{
					__result = (GameInput.GetInputStateForButton(button).flags & GameInput.InputStateFlags.Up) > (GameInput.InputStateFlags)0U;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetIsRunning))]
		public static class GameInput_GetIsRunning__Patch
		{
			[HarmonyPrefix]
			static bool Prefix( ref bool __result)
			{
				int num = GameInput.runMode;
				if (SN1MC.UsingSteamVR)
				{
					if (num == 1)
					{
						__result = !GameInput.GetButtonHeld(GameInput.Button.Sprint);
						return false;
					}
					if (num != 2)
					{
						__result = (GameInput.IsPrimaryDeviceGamepad() && GameInput.isRunningMoveThreshold) || GameInput.GetButtonHeld(GameInput.Button.Sprint);
						return false;
					}
				}
				else
                {
					if (!GameInput.GetButtonHeld(GameInput.Button.MoveDown) && !GameInput.GetButtonHeld(GameInput.Button.MoveUp))
					{
						if (num == 1)
						{
							__result = !GameInput.GetButtonHeld(GameInput.Button.Sprint);
							return false;
						}
						if (num != 2)
						{
							__result = (GameInput.IsPrimaryDeviceGamepad() && GameInput.isRunningMoveThreshold) || GameInput.GetButtonHeld(GameInput.Button.Sprint);
							return false;
						}
					}
				}
				__result =  GameInput.isRunning;
				return false;
			}
		}

		public static void UpdateMoveDirectionSteamVR()
		{
			VRInputManager vrInput = new VRInputManager();
			float num = 0f;
			num += GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
			num -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
			float num2 = 0f;
			num2 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
			num2 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
			float num3 = 0f;
			//num3 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp);
			//num3 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);
			bool floatToBoolUp = false;
			bool floatToBoolDown = false;

			floatToBoolUp = vrInput.GetButton(GameInput.Button.MoveUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
			floatToBoolDown = vrInput.GetButton(GameInput.Button.MoveDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
			if (floatToBoolDown)
				num3 = -1;
			else if (floatToBoolUp)
				num3 = 1;
			else
				num3 = 0;
			if (GameInput.autoMove && num * num + num2 * num2 > 0.010000001f)
			{
				GameInput.autoMove = false;
			}
			if (GameInput.autoMove)
			{
				GameInput.moveDirection.Set(0f, num3, 1f);
			}
			else
			{
				GameInput.moveDirection.Set(num2, num3, num);
			}
			if (GameInput.IsPrimaryDeviceGamepad())
			{
				if (GameInput.autoMove)
				{
					GameInput.isRunningMoveThreshold = false;
					return;
				}
				GameInput.isRunningMoveThreshold = (GameInput.moveDirection.sqrMagnitude > 0.80999994f);
				if (!GameInput.isRunningMoveThreshold)
				{
					GameInput.moveDirection /= 0.9f;
				}
			}
		}

		public static void UpdateMoveDirectionOculus()
		{
			float num = 0f;
			num += GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
			num -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
			float num2 = 0f;
			num2 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
			num2 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
			float num3 = 0f;
			num3 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp);
			num3 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);
			if (GameInput.autoMove && num * num + num2 * num2 > 0.010000001f)
			{
				GameInput.autoMove = false;
			}
			if (GameInput.autoMove)
			{
				GameInput.moveDirection.Set(0f, num3, 1f);
			}
			else
			{
				GameInput.moveDirection.Set(num2, num3, num);
			}
			if (GameInput.IsPrimaryDeviceGamepad())
			{
				if (GameInput.autoMove)
				{
					GameInput.isRunningMoveThreshold = false;
					return;
				}
				GameInput.isRunningMoveThreshold = (GameInput.moveDirection.sqrMagnitude > 0.80999994f);
				if (!GameInput.isRunningMoveThreshold)
				{
					GameInput.moveDirection /= 0.9f;
				}
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.ScanInputs))]
		public static class GameInput_ScanInputs__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameInput __instance)
			{
				bool useKeyboard = GameInput.IsKeyboardAvailable();
				bool useController = GameInput.IsControllerAvailable() && GameInput.controllerEnabled;
				__instance.ClearLastInputPressed();
				__instance.UpdateAxisValues(useKeyboard, useController);
				__instance.UpdateKeyInputs(useKeyboard, useController);
				__instance.UpdateIsRunning();
				if (SN1MC.UsingSteamVR)
				{
					UpdateMoveDirectionSteamVR();
				}
				else
				{
					UpdateMoveDirectionOculus();
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.UpdateMoveDirection))]
		public static class GameInput_UpdateMoveDirection__Patch
		{
			[HarmonyPrefix]
			static bool Prefix()
			{
				float num = 0f;
				num += GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
				num -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
				float num2 = 0f;
				num2 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
				num2 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
				float num3 = 0f;
				num3 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp);
				num3 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);
				if (GameInput.autoMove && num * num + num2 * num2 > 0.010000001f)
				{
					GameInput.autoMove = false;
				}
				if (GameInput.autoMove)
				{
					GameInput.moveDirection.Set(0f, num3, 1f);
				}
				else
				{
					GameInput.moveDirection.Set(num2, num3, num);
				}
				if (GameInput.IsPrimaryDeviceGamepad())
				{
					if (GameInput.autoMove)
					{
						GameInput.isRunningMoveThreshold = false;
						return false;
					}
					GameInput.isRunningMoveThreshold = (GameInput.moveDirection.sqrMagnitude > 0.80999994f);
					if (!GameInput.isRunningMoveThreshold)
					{
						GameInput.moveDirection /= 0.9f;
					}
				}
				return false;
			}
		}
	}
}
