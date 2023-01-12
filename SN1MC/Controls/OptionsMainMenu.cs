
/*using HarmonyLib;
using Oculus.Platform;
using Platform.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace SN1MC.Controls
{
	class OptionsMainMenu
    {
       /* [HarmonyPatch(typeof(MainMenuOptions), nameof(MainMenuOptions.Update))]
        class MainMenuOptions_Update_Patch
        {
            static bool Prefix(MainMenuOptions __instance)
            {
                if (__instance.tabbedPanel != null && Input.GetKeyDown(KeyCode.Escape) || GameInput.GetButtonDown(GameInput.Button.UICancel))
                {
                    if (__instance.tabbedPanel.dialog.open)
                    {
                        __instance.tabbedPanel.dialog.Close();
                        return false;
                    }
                    __instance.OnBack();
                }
                return false;
            }
        }
		/*public static VRUtil.SDK sdk;
		[HarmonyPatch(typeof(VRUtil), nameof(VRUtil.GetLoadedSDK))]
		class VRUtil_GetLoadedSDK_Patch
		{
			static bool Prefix( ref VRUtil.SDK __result)
			{
				if (XRSettings.loadedDeviceName == "Oculus")
				{
					sdk = VRUtil.SDK.Oculus;
				}
				if (XRSettings.loadedDeviceName == "OpenVR")
				{
					sdk = VRUtil.SDK.OpenVR;
				}
				ErrorMessage.AddDebug("SDK: " + sdk);
				Debug.Log("SDK: " + sdk);
				__result = sdk;
				return false;
			}
		}

		[HarmonyPatch(typeof(VRInitialization), nameof(VRInitialization.InitializeOVR))]
		class VRInitialization_InitializeOVR_Patch
		{
			static bool Prefix(VRInitialization __instance)
			{
				if (OVRManager.instance != null)
				{
					UnityEngine.Object.Destroy(__instance.gameObject);
					return false;
				}
				if (XRSettings.enabled)
				{
					__instance.gameObject.AddComponent<OVRManager>();
					OVRManager ovrMan = __instance.gameObject.GetComponent<OVRManager>();
					ovrMan.Invoke("Awake", 1.0f);
					Debug.Log("OVR Manager: " + ovrMan);
					VRUtil.Recenter();
					Core.Initialize("993520564054974");
					Debug.Log("Oculus Started");
				}
				OVRManager.AudioOutChanged += __instance.OnAudioOutChanged;
				return false;
			}
		}

		[HarmonyPatch(typeof(VRInitialization), nameof(VRInitialization.Awake))]
		class VRInitialization_Awake_Patch
		{
			static bool Prefix(VRInitialization __instance)
			{
				VRUtil.SDK loadedSDK = VRUtil.GetLoadedSDK();
				Debug.Log("LoadedSDK: " + loadedSDK);
				if (loadedSDK == VRUtil.SDK.Oculus)
				{
					Debug.Log("Oculus Starting");
					__instance.InitializeOVR();
				}
				else if (loadedSDK == VRUtil.SDK.OpenVR)
				{
					Debug.Log("Steamvr Starting");
					__instance.InitializeSteamVR();
				}
				UnityEngine.Object.DontDestroyOnLoad(__instance.gameObject);
				return false;
			}
		}


		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetUIDirection))]
		class GameInput_Awake_Patch
		{
			static bool Prefix(out bool buttonDown, ref Vector2 __result, GameInput __instance)
			{
				Vector2 zero = Vector2.zero;
				buttonDown = false;

				//if (GameInput.axisValues[3] == 1)
				if (GameInput.GetButtonDown(GameInput.Button.UIDown))
				{
					buttonDown = true;
					zero.y = 1f;
					__result = zero;
				}
				if (GameInput.GetButtonHeld(GameInput.Button.UIDown))
				{
					zero.y = 1f;
					__result = zero;
				}
				//if (GameInput.axisValues[3] == -1)
				if (GameInput.GetButtonDown(GameInput.Button.UIUp))
				{
					buttonDown = true;
					zero.y = -1f;
					__result = zero;
				}
				if (GameInput.GetButtonHeld(GameInput.Button.UIUp))
				{
					zero.y = -1f;
					__result = zero;
				}
				//if (GameInput.axisValues[2] == 1)
				if (GameInput.GetButtonDown(GameInput.Button.UIRight))
				{
					buttonDown = true;
					zero.x = 1f;
					__result = zero;
				}
				if (GameInput.GetButtonHeld(GameInput.Button.UIRight))
				{
					zero.x = 1f;
					__result = zero;
				}
				//if (GameInput.axisValues[2] == -1)
				if (GameInput.GetButtonDown(GameInput.Button.UILeft))
				{
					buttonDown = true;
					zero.x = -1f;
					__result = zero;
				}
				if (GameInput.GetButtonHeld(GameInput.Button.UILeft))
				{
					zero.x = -1f;
					__result = zero;
				}

				__result = zero;

				return false;
			}
		}


		/*public static readonly List<InputDevice> xrDevices = new List<InputDevice>();
		[HarmonyPatch(typeof(GameInput), nameof(GameInput.UpdateControllerAvailable))]
		class GameInput_Update_Patch
		{
			static bool Prefix(GameInput __instance)
			{
				bool flag = false;
				if (GameInput.GetUseOculusInputManager())
				{
					flag = true;
					GameInput.automaticControllerLayout = GameInput.ControllerLayout.XboxOne;
				}
				else if (Application.platform == RuntimePlatform.GameCoreScarlett)
				{
					flag = true;
					GameInput.automaticControllerLayout = GameInput.ControllerLayout.Scarlett;
				}
				else if (Application.platform == RuntimePlatform.XboxOne)
				{
					flag = true;
					GameInput.automaticControllerLayout = GameInput.ControllerLayout.XboxOne;
				}
				else if (GameInput.IsPS4OrPS5Platform())
				{
					flag = true;
					GameInput.automaticControllerLayout = GameInput.ControllerLayout.PS4;
				}
				else if (Application.platform == RuntimePlatform.Switch)
				{
					flag = true;
					GameInput.automaticControllerLayout = GameInput.ControllerLayout.Switch;
				}
				/*else if(XRSettings.enabled)
                {
					InputDevices.GetDevices(xrDevices);
					foreach (InputDevice text in  xrDevices)
					{
						if (text != null)
						{
							if (text.characteristics == InputDeviceCharacteristics.Left || text.characteristics == InputDeviceCharacteristics.Right)
							{
								bool haptic;

								haptic.
								flag = true;
								GameInput.automaticControllerLayout = GameInput.GetControllerLayoutFromName(text.name);
								break;
							}
						}
					}
				}
				else
				{
					foreach (string text in UnityEngine.Input.GetJoystickNames())
					{
						if (text != string.Empty)
						{
							flag = true;
							GameInput.automaticControllerLayout = GameInput.GetControllerLayoutFromName(text);
							break;
						}
					}
				}
				if (flag != GameInput.controllerAvailable)
				{
					GameInput.controllerAvailable = flag;
					GameInput.bindingsChanged = true;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.UpdateKeyInputs))]
		class GameInput_UpdateKeyInputs_Patch
		{
			static bool Prefix(bool useKeyboard, bool useController, GameInput __instance)
			{
				GameInput.ControllerLayout controllerLayout = GameInput.GetControllerLayout();
				//controllerLayout = GameInput.ControllerLayout.Xbox360;
				float unscaledTime = Time.unscaledTime;
				int num = -1;
				PlatformServices services = PlatformUtils.main.GetServices();
				if (services != null)
				{
					num = services.GetActiveController();
				}
				for (int i = 0; i < GameInput.inputs.Count; i++)
				{
					GameInput.InputState inputState = default(GameInput.InputState);
					inputState.timeDown = GameInput.inputStates[i].timeDown;
					GameInput.Device device = GameInput.inputs[i].device;
					//if(GameInput.inputs[i].device == GameInput.Device.Controller)
						//if(GameInput.GetKeyDown(GameInput.inputs[i].keyCode))
							//ErrorMessage.AddDebug("Device: " + GameInput.inputs[i].keyCode);
					KeyCode keyCodeForControllerLayout = __instance.GetKeyCodeForControllerLayout(GameInput.inputs[i].keyCode, controllerLayout);
					if (keyCodeForControllerLayout != KeyCode.None)
					{
						KeyCode keyCode = keyCodeForControllerLayout;
						if (num >= 1)
						{
							keyCode = keyCodeForControllerLayout + num * 20;
						}
						inputState.flags |= __instance.GetInputState(keyCode);
						if (inputState.flags != (GameInput.InputStateFlags)0U && (GameInput.controllerEnabled || device != GameInput.Device.Controller))
						{
							GameInput.lastDevice = device;
						}
					}
					else
					{
						bool flag = (GameInput.inputStates[i].flags & GameInput.InputStateFlags.Held) > (GameInput.InputStateFlags)0U;
						float num2 = GameInput.axisValues[(int)GameInput.inputs[i].axis];
						bool flag2;
						if (GameInput.inputs[i].axisPositive)
						{
							flag2 = (num2 > GameInput.inputs[i].axisDeadZone);
						}
						else
						{
							flag2 = (num2 < -GameInput.inputs[i].axisDeadZone);
						}
						if (flag2)
						{
							inputState.flags |= GameInput.InputStateFlags.Held;
						}
						if (flag2 && !flag)
						{
							inputState.flags |= GameInput.InputStateFlags.Down;
						}
						if (!flag2 && flag)
						{
							inputState.flags |= GameInput.InputStateFlags.Up;
						}
					}
					if ((inputState.flags & GameInput.InputStateFlags.Down) != (GameInput.InputStateFlags)0U)
					{
						GameInput.lastInputPressed[(int)device] = i;
						inputState.timeDown = unscaledTime;
					}
					if ((device == GameInput.Device.Controller && !useController) || (device == GameInput.Device.Keyboard && !useKeyboard))
					{
						inputState.flags = (GameInput.InputStateFlags)0U;
						if ((GameInput.inputStates[i].flags & GameInput.InputStateFlags.Held) > (GameInput.InputStateFlags)0U)
						{
							inputState.flags |= GameInput.InputStateFlags.Up;
						}
					}
					GameInput.inputStates[i] = inputState;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.Initialize))]
		class GameInput_Initialize_Patch
		{
			static bool Prefix(GameInput __instance)
			{
				InputUtils.Initialize();
				int num = GameInput.GetMaximumEnumValue(typeof(GameInput.AnalogAxis)) + 1;
				GameInput.axisValues = new float[num];
				GameInput.lastAxisValues = new float[num];
				GameInput.numButtons = GameInput.GetMaximumEnumValue(typeof(GameInput.Button)) + 1;
				GameInput.numDevices = GameInput.GetMaximumEnumValue(typeof(GameInput.Device)) + 1;
				GameInput.numBindingSets = GameInput.GetMaximumEnumValue(typeof(GameInput.BindingSet)) + 1;
				GameInput.buttonBindings = new Array3<int>(GameInput.numDevices, GameInput.numButtons, GameInput.numBindingSets);
				GameInput.lastInputPressed = new int[GameInput.numDevices];
				__instance.ClearLastInputPressed();
				//ErrorMessage.AddDebug("axisValues: " + GameInput.axisValues);
				//Debug.Log("axisValues: " + GameInput.axisValues);
				//ErrorMessage.AddDebug("lastAxisValues: " + GameInput.lastAxisValues);
				//Debug.Log("lastAxisValues: " + GameInput.lastAxisValues);
				//ErrorMessage.AddDebug("numButtons: " + GameInput.numButtons);
				//Debug.Log("numButtons: " + GameInput.numButtons);
                //ErrorMessage.AddDebug("numDevices: " + GameInput.numDevices);
				//Debug.Log("numDevices: " + GameInput.numDevices);
				foreach (object obj in Enum.GetValues(typeof(KeyCode)))
				{
					KeyCode keyCode = (KeyCode)obj;
					if (keyCode != KeyCode.None && (keyCode < KeyCode.JoystickButton0 || keyCode > KeyCode.Joystick8Button19))
					{
						//ErrorMessage.AddDebug("KeycodeIf: " + keyCode);
						GameInput.AddKeyInput(GameInput.GetKeyCodeAsInputName(keyCode), keyCode, GameInput.GetKeyCodeDevice(keyCode));
					}
				}
				GameInput.AddAxisInput("MouseWheelUp", GameInput.AnalogAxis.MouseWheel, true, GameInput.Device.Keyboard, 0f);
				GameInput.AddAxisInput("MouseWheelDown", GameInput.AnalogAxis.MouseWheel, false, GameInput.Device.Keyboard, 0f);
				GameInput.AddAxisInput("ControllerRightStickRight", GameInput.AnalogAxis.ControllerRightStickX, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerRightStickLeft", GameInput.AnalogAxis.ControllerRightStickX, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerRightStickUp", GameInput.AnalogAxis.ControllerRightStickY, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerRightStickDown", GameInput.AnalogAxis.ControllerRightStickY, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickRight", GameInput.AnalogAxis.ControllerLeftStickX, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickLeft", GameInput.AnalogAxis.ControllerLeftStickX, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickUp", GameInput.AnalogAxis.ControllerLeftStickY, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickDown", GameInput.AnalogAxis.ControllerLeftStickY, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickRightMenu", GameInput.AnalogAxis.ControllerLeftStickX, true, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftStickLeftMenu", GameInput.AnalogAxis.ControllerLeftStickX, false, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftStickUpMenu", GameInput.AnalogAxis.ControllerLeftStickY, false, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftStickDownMenu", GameInput.AnalogAxis.ControllerLeftStickY, true, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftTrigger", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0.5f);
				GameInput.AddAxisInput("ControllerRightTrigger", GameInput.AnalogAxis.ControllerRightTrigger, true, GameInput.Device.Controller, 0.5f);
				GameInput.AddAxisInput("ControllerDPadRight", GameInput.AnalogAxis.ControllerDPadX, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerDPadLeft", GameInput.AnalogAxis.ControllerDPadX, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerDPadUp", GameInput.AnalogAxis.ControllerDPadY, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerDPadDown", GameInput.AnalogAxis.ControllerDPadY, false, GameInput.Device.Controller, 0f);
				/*GameInput.AddAxisInput("MouseWheelUp", GameInput.AnalogAxis.MouseWheel, true, GameInput.Device.Keyboard, 0f);
				GameInput.AddAxisInput("MouseWheelDown", GameInput.AnalogAxis.MouseWheel, false, GameInput.Device.Keyboard, 0f);
				GameInput.AddAxisInput("ControllerRightStickRight", GameInput.AnalogAxis.ControllerRightStickX, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerRightStickLeft", GameInput.AnalogAxis.ControllerRightStickX, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerRightStickUp", GameInput.AnalogAxis.ControllerRightStickY, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerRightStickDown", GameInput.AnalogAxis.ControllerRightStickY, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickRight", GameInput.AnalogAxis.ControllerLeftStickX, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickLeft", GameInput.AnalogAxis.ControllerLeftStickX, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickUp", GameInput.AnalogAxis.ControllerLeftStickY, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickDown", GameInput.AnalogAxis.ControllerLeftStickY, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerLeftStickRightMenu", GameInput.AnalogAxis.ControllerLeftStickX, true, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftStickLeftMenu", GameInput.AnalogAxis.ControllerLeftStickX, false, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftStickUpMenu", GameInput.AnalogAxis.ControllerLeftStickY, false, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftStickDownMenu", GameInput.AnalogAxis.ControllerLeftStickY, true, GameInput.Device.Controller, 0.75f);
				GameInput.AddAxisInput("ControllerLeftTrigger", GameInput.AnalogAxis.ControllerLeftTrigger, true, GameInput.Device.Controller, 0.5f);
				GameInput.AddAxisInput("ControllerRightTrigger", GameInput.AnalogAxis.ControllerRightTrigger, true, GameInput.Device.Controller, 0.5f);
				GameInput.AddAxisInput("ControllerDPadRight", GameInput.AnalogAxis.ControllerDPadX, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerDPadLeft", GameInput.AnalogAxis.ControllerDPadX, false, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerDPadUp", GameInput.AnalogAxis.ControllerDPadY, true, GameInput.Device.Controller, 0f);
				GameInput.AddAxisInput("ControllerDPadDown", GameInput.AnalogAxis.ControllerDPadY, false, GameInput.Device.Controller, 0f);
				GameInput.inputStates = new GameInput.InputState[GameInput.inputs.Count];
				return false;
			}
		}

	
	[HarmonyPatch(typeof(uGUI_QuickSlots), nameof(uGUI_QuickSlots.HandleInput))]
		class GameInput_GetKeyCodeForControllerLayout_Patch
		{
			static bool Prefix( uGUI_QuickSlots __instance)
			{
				Player main = Player.main;
				if (!main.GetCanItemBeUsed())
				{
					return false;
				}
				bool isIntroActive = uGUI.isIntro || IntroLifepodDirector.IsActive;
				if (!isIntroActive)
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

					if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
					{
						__instance.target.SlotNext();
					}
					else if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
					{
						__instance.target.SlotPrevious();
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
					if (!isIntroActive && GameInput.GetButtonDown(GameInput.Button.Exit))
					{
						__instance.target.DeselectSlots();
					}
					
				}
				return false;
			}
		}
	}
}*/
