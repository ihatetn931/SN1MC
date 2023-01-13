
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
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					__result = vrInput.GetButtonDown(button, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					__result = (GameInput.GetInputStateForButton(button).flags & GameInput.InputStateFlags.Down) > (GameInput.InputStateFlags)0U;
				//if (__result)
					//ErrorMessage.AddDebug("ResultGetButtonDown: " + button);
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetButtonHeld))]
		public static class GameInput_GetButtonHeld__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameInput.Button button, ref bool __result)
			{
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					__result = vrInput.GetButton(button, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					__result = (GameInput.GetInputStateForButton(button).flags & GameInput.InputStateFlags.Up) > (GameInput.InputStateFlags)0U;
				//if (__result)
					//ErrorMessage.AddDebug("ResultGetButtonHeld: " + button);
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetButtonUp))]
		public static class GameInput_GetButtonUp__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameInput.Button button, ref bool __result)
			{
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					__result = vrInput.GetButtonUp(button, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					__result = (GameInput.GetInputStateForButton(button).flags & GameInput.InputStateFlags.Up) > (GameInput.InputStateFlags)0U;
				//if(__result)
					//ErrorMessage.AddDebug("ResultGetButtonUp: " + button);
				return false;
			}
		}

		[HarmonyPatch(typeof(GameInput), nameof(GameInput.UpdateMoveDirection))]
		public static class GameInput_UpdateMoveDirection__Patch
		{
			[HarmonyPrefix]
			static bool Prefix()
			{
				VRInputManager vrInput = new VRInputManager();
				float num = 0f;
				num += GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
				num -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
				float num2 = 0f;
				num2 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
				num2 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
				float num3 = 0f;
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
				//num3 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp);
				//num3 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);
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

		/*		[HarmonyPatch(typeof(GameInputExtensions), nameof(GameInputExtensions.GetButtonDown))]
				public static class GameInputExtensions_GetButtonDown__Patch
				{
					[HarmonyPrefix]
					static bool Prefix(GameInput.Button[] buttons, ref bool __result)
					{
						int i = 0;
						int num = buttons.Length;
						while (i < num)
						{
							Debug.Log("Buttons: " + buttons[i]);
							if (GameInput.GetButtonDown(buttons[i]))
							{
								__result =  true;
							}
							i++;
						}
						__result =  false;
						return false;
					}
				}*/

		/*		[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetInputState))]
				public static class GameInput_InputStateFlags__Patch
				{
					[HarmonyPrefix]
					static bool Prefix(KeyCode keyCode, ref GameInput.InputStateFlags __result)
					{

						GameInput.InputStateFlags inputStateFlags = (GameInput.InputStateFlags)0U;
						if (InputUtils.GetKey(keyCode))
						{
							ErrorMessage.AddDebug("KeyCode: " + keyCode);
							inputStateFlags |= GameInput.InputStateFlags.Held;
						}
						if (InputUtils.GetKeyDown(keyCode))
						{
							ErrorMessage.AddDebug("KeyCodeDown: " + keyCode);
							inputStateFlags |= GameInput.InputStateFlags.Down;
						}
						if (InputUtils.GetKeyUp(keyCode))
						{
							ErrorMessage.AddDebug("KeyCodeUp: " + keyCode);
							inputStateFlags |= GameInput.InputStateFlags.Up;
						}
						__result = inputStateFlags;
						return false;
					}
				}

				/*public static bool GetKey(SteamVRButtonCode keyCode)
				{
					return SteamVRRef.Valve.VR.SteamVR_Input.GetState(keyCode, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);// Input.GetKey(keyCode);
				}

				public static bool GetKeyDown(string keyCode)
				{
					return SteamVRRef.Valve.VR.SteamVR_Input.GetStateDown(keyCode, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any); ;//Input.GetKeyDown(keyCode);
				}

				public static bool GetKeyUp(string keyCode)
				{
					return SteamVRRef.Valve.VR.SteamVR_Input.GetStateUp(keyCode, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);//Input.GetKeyUp(keyCode);
				}

				public static bool GetXRInput(KeyCode key)
				{
					switch (key)
					{
						case KeyCode.JoystickButton0:
							// ControllerButtonA
							return VRInputManager.LeftHandButton || VRInputManager.ADown;
						case KeyCode.JoystickButton1:
							// ControllerButtonB
							return VRInputManager.BDown;
						case KeyCode.JoystickButton2:
							// ControllerButtonY
							return VRInputManager.YDown;
						case KeyCode.JoystickButton3:
							// ControllerButtonX
							return VRInputManager.XDown;
						case KeyCode.JoystickButton4:
							// ControllerButtonLeftBumper
							return VRInputManager.MoveUpDown;
						case KeyCode.JoystickButton5:
							// ControllerButtonRightBumper
							return VRInputManager.MoveDownDown;
						case KeyCode.JoystickButton6:
							// ControllerButtonBack - reservered by "oculus" button
							return VRInputManager.ButtonUIMenu;
						case KeyCode.JoystickButton7:
							// ControllerButtonHome
							return VRInputManager.PdaButton;
						case KeyCode.JoystickButton8:
							// ControllerButtonLeftStick
							return VRInputManager.LeftJoyStickPressed;
						case KeyCode.JoystickButton9:
							// ControllerButtonRightStick
							return VRInputManager.RightJoyStickPress;
						/*case KeyCode.JoystickButton10:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton11:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton12:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton13:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton14:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton15:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton16:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton17:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton18:
							// ControllerButtonRightStick
							return true;
						case KeyCode.JoystickButton19:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button0:
							// ControllerButtonA
							return true;
						case KeyCode.Joystick1Button1:
							// ControllerButtonB
							return true;
						case KeyCode.Joystick1Button2:
							// ControllerButtonY
							return true;
						case KeyCode.Joystick1Button3:
							// ControllerButtonX
							return true;
						case KeyCode.Joystick1Button4:
							// ControllerButtonLeftBumper
							return true;
						case KeyCode.Joystick1Button5:
							// ControllerButtonRightBumper
							return true;
						case KeyCode.Joystick1Button6:
							// ControllerButtonBack - reservered by "oculus" button
							return true;
						case KeyCode.Joystick1Button7:
							// ControllerButtonHome
							return true;
						case KeyCode.Joystick1Button8:
							// ControllerButtonLeftStick
							return true;
						case KeyCode.Joystick1Button9:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button10:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button11:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button12:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button13:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button14:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button15:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button16:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button17:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button18:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick1Button19:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button0:
							// ControllerButtonA
							return true;
						case KeyCode.Joystick2Button1:
							// ControllerButtonB
							return true;
						case KeyCode.Joystick2Button2:
							// ControllerButtonY
							return true;
						case KeyCode.Joystick2Button3:
							// ControllerButtonX
							return true;
						case KeyCode.Joystick2Button4:
							// ControllerButtonLeftBumper
							return true;
						case KeyCode.Joystick2Button5:
							// ControllerButtonRightBumper
							return true;
						case KeyCode.Joystick2Button6:
							// ControllerButtonBack - reservered by "oculus" button
							return true;
						case KeyCode.Joystick2Button7:
							// ControllerButtonHome
							return true;
						case KeyCode.Joystick2Button8:
							// ControllerButtonLeftStick
							return true;
						case KeyCode.Joystick2Button9:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button10:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button11:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button12:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button13:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button14:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button15:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button16:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button17:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button18:
							// ControllerButtonRightStick
							return true;
						case KeyCode.Joystick2Button19:
							// ControllerButtonRightStick
							return true;
						default:
							return false;
					}
				}

				[HarmonyPatch(typeof(GameInput), "UpdateKeyInputs")]
				internal class UpdateKeyInputsPatch
				{
					public static bool Prefix(bool useKeyboard, bool useController, GameInput __instance)
					{
						//XRInputManager xrInput = XRInputManager.GetXRInputManager();
						//if (!xrInput.hasControllers())
					//	{
						//	return true;
					//	}

						float unscaledTime = Time.unscaledTime;
						for (int i = 0; i < GameInput.inputs.Count; i++)
						{
							GameInput.InputState inputState = default;
							GameInput.InputState prevInputState = GameInput.inputStates[i];
							inputState.timeDown = prevInputState.timeDown;
							bool wasHeld = (prevInputState.flags & GameInput.InputStateFlags.Held) > 0U;

							GameInput.Input currentInput = GameInput.inputs[i];
							GameInput.Device device = currentInput.device;
							KeyCode key = currentInput.keyCode;

							if (key != KeyCode.None)
							{
								bool pressed = GetXRInput(key);
								if (pressed)
									ErrorMessage.AddDebug("Key: " + key);
								GameInput.InputStateFlags prevState = GameInput.inputStates[i].flags;
								if (pressed && (prevState == GameInput.InputStateFlags.Held && prevState == GameInput.InputStateFlags.Down))
								{
									inputState.flags |= GameInput.InputStateFlags.Held;
								}
								if (pressed && prevState == GameInput.InputStateFlags.Up)
								{
									inputState.flags |= GameInput.InputStateFlags.Down;
								}
								if (!pressed)
								{
									inputState.flags |= GameInput.InputStateFlags.Up;
								}
								if (inputState.flags != 0U && !PlatformUtils.isConsolePlatform && (GameInput.controllerEnabled || device != GameInput.Device.Controller))
								{
									GameInput.lastDevice = device;
								}
							}
							else
							{
								float axisValue = GameInput.axisValues[(int)currentInput.axis];
								bool isPressed;
								if (GameInput.inputs[i].axisPositive)
								{
									isPressed = (axisValue > currentInput.axisDeadZone);
								}
								else
								{
									isPressed = (axisValue < -currentInput.axisDeadZone);
								}
								if (isPressed)
								{
									inputState.flags |= GameInput.InputStateFlags.Held;
								}
								if (isPressed && !wasHeld)
								{
									inputState.flags |= GameInput.InputStateFlags.Down;
								}
								if (!isPressed && wasHeld)
								{
									inputState.flags |= GameInput.InputStateFlags.Up;
								}
							}

							if ((inputState.flags & GameInput.InputStateFlags.Down) != 0U)
							{
								int lastIndex = GameInput.lastInputPressed[(int)device];
								int newIndex = i;
								inputState.timeDown = unscaledTime;
								if (lastIndex > -1)
								{
									GameInput.Input lastInput = GameInput.inputs[lastIndex];
									bool isSameTime = inputState.timeDown == GameInput.inputStates[lastIndex].timeDown;
									bool lastAxisIsGreater = Mathf.Abs(GameInput.axisValues[(int)lastInput.axis]) > Mathf.Abs(GameInput.axisValues[(int)currentInput.axis]);
									if (isSameTime && lastAxisIsGreater)
									{
										newIndex = lastIndex;
									}
								}
								GameInput.lastInputPressed[(int)device] = newIndex;
							}

							if ((device == GameInput.Device.Controller && !useController) || (device == GameInput.Device.Keyboard && !useKeyboard))
							{
								inputState.flags = 0U;
								if (wasHeld)
								{
									inputState.flags |= GameInput.InputStateFlags.Up;
								}
							}
							GameInput.inputStates[i] = inputState;
						}

						return false;
					}
					/*
						[HarmonyPatch(typeof(GameInput), nameof(GameInput.Initialize))]
						public static class GameInput_Initialize__Patch
						{
							[HarmonyPrefix]
							static bool Prefix(GameInput __instance)
							{
								InputUtils.Initialize();
								int num = GameInput.GetMaximumEnumValue(typeof(GameInput.AnalogAxis)) + 1;
								GameInput.axisValues = new float[num];
								GameInput.lastAxisValues = new float[num];
								GameInput.numButtons = GameInput.GetMaximumEnumValue(typeof(GameInput.Button));
								GameInput.numDevices = GameInput.GetMaximumEnumValue(typeof(GameInput.Device)) + 1;
								GameInput.numBindingSets = GameInput.GetMaximumEnumValue(typeof(GameInput.BindingSet)) + 1;
								GameInput.buttonBindings = new Array3<int>(GameInput.numDevices, GameInput.numButtons, GameInput.numBindingSets);
								GameInput.lastInputPressed = new int[GameInput.numDevices];
								__instance.ClearLastInputPressed();
								foreach (object obj in Enum.GetValues(typeof(KeyCode)))
								{
									KeyCode keyCode = (KeyCode)obj;
									if (keyCode != KeyCode.None && (keyCode < KeyCode.Joystick1Button0 || keyCode > KeyCode.Joystick8Button19))
									{
										GameInput.AddKeyInput(GameInput.GetKeyCodeAsInputName(keyCode), keyCode, GameInput.GetKeyCodeDevice(keyCode));
									}
								}
								foreach (object obj in Enum.GetValues(typeof(SteamVRButtonCode)))
								{
									SteamVRButtonCode buttonCode = (SteamVRButtonCode)obj;
									if (buttonCode != SteamVRButtonCode.None)
									{
										//AddKeyInput(GetKeyCodeAsInputName(buttonCode), buttonCode, GameInput.Device.Controller);
										AddKeyInput(buttonCode.ToString(), buttonCode, GameInput.Device.Keyboard);
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
								GameInput.inputStates = new GameInput.InputState[GameInput.inputs.Count];
								inputStates = new InputState[inputs.Count];
								//foreach (var t in GameInput.inputStates)
								//{
								//	Debug.Log("InputStatesFlags: " + t.flags);
								//	Debug.Log("InputStatesTimeDown: " + t.timeDown);
								//}
								return false;
							}
						}
						public static InputState[] inputStates;
						public struct InputState
						{
							public InputStateFlags flags;
							public float timeDown;
						}

						[Flags]
						public enum InputStateFlags : uint
						{
							Down = 1U,
							Up = 2U,
							Held = 4U
						}*/

		/*public static InputStateFlags GetInputState(SteamVRButtonCode keyCode)
			{
				InputStateFlags inputStateFlags = (InputStateFlags)0U;
				if (GetKey(keyCode))
				{
					inputStateFlags |= InputStateFlags.Held;
				}
				/*if (GetKeyDown(keyCode))
				{
					inputStateFlags |= InputStateFlags.Down;
				}
				if (GetKeyUp(keyCode))
				{
					inputStateFlags |= InputStateFlags.Up;
				}
				return inputStateFlags;
			}*/

		/*private static void AddKeyInput(string name, SteamVRButtonCode keyCode, GameInput.Device device)
		{
			Input item = default(Input);
			item.name = name;
			item.keyCode = keyCode;
			item.device = device;
			inputs.Add(item);
			//Debug.Log("ItemDevice: " + item.device);
			//Debug.Log("ItemKeyCode: " + item.keyCode);
			//Debug.Log("ItemName: " + item.name);
		}


		private static List<Input> inputs = new List<Input>();

		private struct Input
		{
			public string name;
			public SteamVRButtonCode keyCode;
			public GameInput.Device device;
		}

		public enum SteamVRButtonCode
		{
			Jump,
			PDA,
			Deconstruct,
			Exit,
			LeftHand,
			RightHand,
			CycleNext,
			CyclePrev,
			Slot1,
			Slot2,
			Slot3,
			Slot4,
			Slot5,
			AltTool,
			TakePicture,
			Reload,
			Sprint,
			MoveUp,
			MoveDown,
			MoveForward,
			MoveBackward,
			MoveLeft,
			MoveRight,
			AutoMove,
			LookUp,
			LookDown,
			LookLeft,
			LookRight,
			UISubmit,
			UICancel,
			UIClear,
			UIAssign,
			UILeft,
			UIRight,
			UIUp,
			UIDown,
			UIMenu,
			UIAdjustLeft,
			UIAdjustRight,
			UINextTab,
			UIPrevTab,
			Feedback,
			UIRightStickAdjustLeft,
			UIRightStickAdjustRight,
			None
		}


		/*	[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetInputStateForButton))]
			public static class GameInput_GetInputStateForButton__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Button button, ref GameInput.InputState __result)
				{
					GameInput.InputState inputState = default(GameInput.InputState);
					if (!GameInput.clearInput && !GameInput.scanningInput)
					{
						for (int i = 0; i < GameInput.numDevices; i++)
						{
							for (int j = 0; j < GameInput.numBindingSets; j++)
							{
								int bindingInternal = GameInput.GetBindingInternal((GameInput.Device)i, button, (GameInput.BindingSet)j);
								if (bindingInternal != -1)
								{
									inputState.flags |= GameInput.inputStates[bindingInternal].flags;
									inputState.timeDown = Mathf.Max(inputState.timeDown, GameInput.inputStates[bindingInternal].timeDown);
								}
							}
						}
					}
					__result = inputState;
					return false;
				}
			}

			/*[HarmonyPatch(typeof(GameInput), nameof(GameInput.GetBindingInternal))]
			public static class GameInput_GetBindingInternal__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Device device, GameInput.Button button, GameInput.BindingSet bindingSet, ref int __result)
				{
					__result = GameInput.buttonBindings[(int)device, (int)button, (int)bindingSet];
					return false;
				}
			}
			private static int GetInputIndex(string name)
			{
				for (int i = 0; i < inputs.Count; i++)
				{
					if (inputs[i].name == name)
					{
						return i;
					}
				}
				return -1;
			}
			[HarmonyPatch(typeof(GameInput), nameof(GameInput.SetBindingInternal), new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(string) })]
			public static class GameInput_SetBindingInternal__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Device device, GameInput.Button button, GameInput.BindingSet bindingSet, string input)
				{
					int inputIndex = GetInputIndex(input);
					if (inputIndex == -1 && !string.IsNullOrEmpty(input))
					{
						Debug.LogErrorFormat("GameInput: Input {0} not found", new object[]
						{
					input
						});
					}
					GameInput.SetBindingInternal(device, button, bindingSet, inputIndex);
					return false;
				}
			}

			private struct Input
			{
				// Token: 0x040043A9 RID: 17321
				public string name;

				// Token: 0x040043AA RID: 17322
				public KeyCode keyCode;

				// Token: 0x040043AB RID: 17323
				public GameInput.AnalogAxis axis;

				// Token: 0x040043AC RID: 17324
				public bool axisPositive;

				// Token: 0x040043AD RID: 17325
				public GameInput.Device device;

				// Token: 0x040043AE RID: 17326
				public float axisDeadZone;
			}
			private static List<Input> inputs = new List<Input>();


			private static void AddKeyInput(string name, KeyCode keyCode, GameInput.Device device)
			{
				Input item = default(Input);
				item.name = name;
				item.keyCode = keyCode;
				item.device = device;
				inputs.Add(item);
			}


			[HarmonyPatch(typeof(GameInput), nameof(GameInput.SetBindingInternal), new Type[] { typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameInput.BindingSet), typeof(int) })]
			public static class GameInput_SetBindingInternal1__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Device device, GameInput.Button button, GameInput.BindingSet bindingSet, int inputIndex)
				{
					GameInput.buttonBindings[(int)device, (int)button, (int)bindingSet] = inputIndex;
					GameInput.bindingsChanged = true;
					return false;
				}
			}

			[HarmonyPatch(typeof(GameInput), nameof(GameInput.SetBinding))]
			public static class GameInput_SetBinding__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Device device, GameInput.Button button, GameInput.BindingSet bindingSet, string input)
				{
					GameInput.SetBindingInternal(device, button, bindingSet, input);
					return false;
				}
			}

			[HarmonyPatch(typeof(GameInput), nameof(GameInput.SafeSetBinding))]
			public static class GameInput_SafeSetBinding__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Device device, GameInput.Button button, GameInput.BindingSet bindingSet, string input)
				{
					if (!string.IsNullOrEmpty(input))
					{
						using (ListPool<KeyValuePair<GameInput.Button, GameInput.BindingSet>> listPool = Pool<ListPool<KeyValuePair<GameInput.Button, GameInput.BindingSet>>>.Get())
						{
							List<KeyValuePair<GameInput.Button, GameInput.BindingSet>> list = listPool.list;
							BindConflicts.GetConflicts(device, input, button, list);
							for (int i = 0; i < list.Count; i++)
							{
								KeyValuePair<GameInput.Button, GameInput.BindingSet> keyValuePair = list[i];
								GameInput.SetBinding(device, keyValuePair.Key, keyValuePair.Value, null);
							}
						}
						foreach (object obj in Enum.GetValues(typeof(GameInput.BindingSet)))
						{
							GameInput.BindingSet bindingSet2 = (GameInput.BindingSet)obj;
							if (bindingSet2 != bindingSet && GameInput.GetBinding(device, button, bindingSet2) == input)
							{
								GameInput.SetBinding(device, button, bindingSet2, null);
							}
						}
					}
					GameInput.SetBinding(device, button, bindingSet, input);
					return false;
				}
			}

			[HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.TryBind))]
			public static class uGUI_TabbedControlsPanel_TryBind__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(GameInput.Device device, GameInput.Button button, GameInput.BindingSet bindingSet, string input, uGUI_TabbedControlsPanel __instance)
				{
					if (string.IsNullOrEmpty(input))
					{
						string binding = GameInput.GetBinding(device, button, bindingSet);
						if (!string.IsNullOrEmpty(binding))
						{
							string arg = string.Format("<color=#ADF8FFFF>{0}</color>", uGUI.GetDisplayTextForBinding(GameInput.GetInputName(binding)));
							string text = string.Format(Language.main.Get("UnbindFormat"), arg, string.Format("<color=#ADF8FFFF>{0}</color>", Language.main.Get("Option" + button.ToString())));
							__instance.dialog.Show(text, delegate (int option)
							{
								if (option == 1)
								{
									GameInput.SetBinding(device, button, bindingSet, null);
								}
							}, new string[]
							{
						Language.main.Get("No"),
						Language.main.Get("Yes")
							});
						}
						return false;
					}
					using (ListPool<KeyValuePair<GameInput.Button, GameInput.BindingSet>> listPool = Pool<ListPool<KeyValuePair<GameInput.Button, GameInput.BindingSet>>>.Get())
					{
						List<KeyValuePair<GameInput.Button, GameInput.BindingSet>> list = listPool.list;
						BindConflicts.GetConflicts(device, input, button, list);
						if (list.Count > 0)
						{
							StringBuilder stringBuilder = new StringBuilder();
							string value = Language.main.Get("InputSeparator");
							for (int i = 0; i < list.Count; i++)
							{
								if (i > 0)
								{
									stringBuilder.Append(value);
								}
								stringBuilder.AppendFormat("<color=#ADF8FFFF>{0}</color>", Language.main.Get("Option" + list[i].Key.ToString()));
							}
							string arg2 = string.Format("<color=#ADF8FFFF>{0}</color>", uGUI.GetDisplayTextForBinding(GameInput.GetInputName(input)));
							string format = Language.main.GetFormat<string, StringBuilder, string>("BindConflictFormat", arg2, stringBuilder, string.Format("<color=#ADF8FFFF>{0}</color>", Language.main.Get("Option" + button.ToString())));
							__instance.dialog.Show(format, delegate (int option)
							{
								if (option == 1)
								{
									GameInput.SafeSetBinding(device, button, bindingSet, input);
								}
							}, new string[]
							{
						Language.main.Get("No"),
						Language.main.Get("Yes")
							});
						}
						else
						{
							GameInput.SafeSetBinding(device, button, bindingSet, input);
						}
					}
					return false;
				}
			}
			private static GameInput.Device GetKeyCodeDevice(KeyCode keyCode)
			{
				if (keyCode >= KeyCode.JoystickButton0 && keyCode <= KeyCode.Joystick8Button19)
				{
					return GameInput.Device.Controller;
				}
				return GameInput.Device.Keyboard;
			}
			[HarmonyPatch(typeof(GameInput), nameof(GameInput.Initialize))]
			public static class GameInput_Initialize__Patch
			{
				[HarmonyPrefix]
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
					foreach (object obj in Enum.GetValues(typeof(KeyCode)))
					{
						KeyCode keyCode = (KeyCode)obj;
						if (keyCode != KeyCode.None && (keyCode < KeyCode.Joystick1Button0 || keyCode > KeyCode.Joystick8Button19))
						{
							AddKeyInput(GetKeyCodeAsInputName(keyCode), keyCode, GetKeyCodeDevice(keyCode));
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
					GameInput.inputStates = new GameInput.InputState[inputs.Count];
					return false;
				}
			}
			private static string GetKeyCodeAsInputName(KeyCode keyCode)
			{
				switch (keyCode)
				{
					case KeyCode.Alpha0:
						return "0";
					case KeyCode.Alpha1:
						return "1";
					case KeyCode.Alpha2:
						return "2";
					case KeyCode.Alpha3:
						return "3";
					case KeyCode.Alpha4:
						return "4";
					case KeyCode.Alpha5:
						return "5";
					case KeyCode.Alpha6:
						return "6";
					case KeyCode.Alpha7:
						return "7";
					case KeyCode.Alpha8:
						return "8";
					case KeyCode.Alpha9:
						return "9";
					default:
						switch (keyCode)
						{
							case KeyCode.Mouse0:
								return "MouseButtonLeft";
							case KeyCode.Mouse1:
								return "MouseButtonRight";
							case KeyCode.Mouse2:
								return "MouseButtonMiddle";
							case KeyCode.JoystickButton0:
								return "ControllerButtonA";
							case KeyCode.JoystickButton1:
								return "ControllerButtonB";
							case KeyCode.JoystickButton2:
								return "ControllerButtonX";
							case KeyCode.JoystickButton3:
								return "ControllerButtonY";
							case KeyCode.JoystickButton4:
								return "ControllerButtonLeftBumper";
							case KeyCode.JoystickButton5:
								return "ControllerButtonRightBumper";
							case KeyCode.JoystickButton6:
								return "ControllerButtonBack";
							case KeyCode.JoystickButton7:
								return "ControllerButtonHome";
							case KeyCode.JoystickButton8:
								return "ControllerButtonLeftStick";
							case KeyCode.JoystickButton9:
								return "ControllerButtonRightStick";
						}
						return keyCode.ToString();
				}
			}
			[HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.AddBindingOption), new Type[] { typeof(int), typeof(string), typeof(GameInput.Device), typeof(GameInput.Button), typeof(GameObject) })]
			public static class uGUI_TabbedControlsPanel_AddBindingOption__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(int tabIndex, string label, GameInput.Device device, GameInput.Button button, out GameObject bindingObject, ref uGUI_Bindings __result, uGUI_TabbedControlsPanel __instance )
				{
					bindingObject = __instance.AddItem(tabIndex, __instance.bindingOptionPrefab, label);
					uGUI_Bindings componentInChildren = bindingObject.GetComponentInChildren<uGUI_Bindings>();
					__instance.bindings.Add(componentInChildren);
					componentInChildren.Initialize(device, button, new Action<GameInput.Device, GameInput.Button, GameInput.BindingSet, string>(__instance.TryBind));
					__result = componentInChildren;
					return false;
				}
			}

			[HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.AddBindingOption), new Type[] { typeof(int), typeof(string), typeof(GameInput.Device), typeof(GameInput.Button) } )]
			public static class uGUI_TabbedControlsPanel_AddBindingOption1__Patch
			{
				[HarmonyPrefix]
				static bool Prefix(int tabIndex, string label, GameInput.Device device, GameInput.Button button, ref uGUI_Bindings __result, uGUI_TabbedControlsPanel __instance)
				{
					GameObject gameObject;
					__result = __instance.AddBindingOption(tabIndex, label, device, button, out gameObject);
					return false;
				}
			}
			public enum KeyCode
			{
				// Token: 0x04000456 RID: 1110
				None,
				// Token: 0x04000457 RID: 1111
				Backspace = 8,
				// Token: 0x04000458 RID: 1112
				Delete = 127,
				// Token: 0x04000459 RID: 1113
				Tab = 9,
				// Token: 0x0400045A RID: 1114
				Clear = 12,
				// Token: 0x0400045B RID: 1115
				Return,
				// Token: 0x0400045C RID: 1116
				Pause = 19,
				// Token: 0x0400045D RID: 1117
				Escape = 27,
				// Token: 0x0400045E RID: 1118
				Space = 32,
				// Token: 0x0400045F RID: 1119
				Keypad0 = 256,
				// Token: 0x04000460 RID: 1120
				Keypad1,
				// Token: 0x04000461 RID: 1121
				Keypad2,
				// Token: 0x04000462 RID: 1122
				Keypad3,
				// Token: 0x04000463 RID: 1123
				Keypad4,
				// Token: 0x04000464 RID: 1124
				Keypad5,
				// Token: 0x04000465 RID: 1125
				Keypad6,
				// Token: 0x04000466 RID: 1126
				Keypad7,
				// Token: 0x04000467 RID: 1127
				Keypad8,
				// Token: 0x04000468 RID: 1128
				Keypad9,
				// Token: 0x04000469 RID: 1129
				KeypadPeriod,
				// Token: 0x0400046A RID: 1130
				KeypadDivide,
				// Token: 0x0400046B RID: 1131
				KeypadMultiply,
				// Token: 0x0400046C RID: 1132
				KeypadMinus,
				// Token: 0x0400046D RID: 1133
				KeypadPlus,
				// Token: 0x0400046E RID: 1134
				KeypadEnter,
				// Token: 0x0400046F RID: 1135
				KeypadEquals,
				// Token: 0x04000470 RID: 1136
				UpArrow,
				// Token: 0x04000471 RID: 1137
				DownArrow,
				// Token: 0x04000472 RID: 1138
				RightArrow,
				// Token: 0x04000473 RID: 1139
				LeftArrow,
				// Token: 0x04000474 RID: 1140
				Insert,
				// Token: 0x04000475 RID: 1141
				Home,
				// Token: 0x04000476 RID: 1142
				End,
				// Token: 0x04000477 RID: 1143
				PageUp,
				// Token: 0x04000478 RID: 1144
				PageDown,
				// Token: 0x04000479 RID: 1145
				F1,
				// Token: 0x0400047A RID: 1146
				F2,
				// Token: 0x0400047B RID: 1147
				F3,
				// Token: 0x0400047C RID: 1148
				F4,
				// Token: 0x0400047D RID: 1149
				F5,
				// Token: 0x0400047E RID: 1150
				F6,
				// Token: 0x0400047F RID: 1151
				F7,
				// Token: 0x04000480 RID: 1152
				F8,
				// Token: 0x04000481 RID: 1153
				F9,
				// Token: 0x04000482 RID: 1154
				F10,
				// Token: 0x04000483 RID: 1155
				F11,
				// Token: 0x04000484 RID: 1156
				F12,
				// Token: 0x04000485 RID: 1157
				F13,
				// Token: 0x04000486 RID: 1158
				F14,
				// Token: 0x04000487 RID: 1159
				F15,
				// Token: 0x04000488 RID: 1160
				Alpha0 = 48,
				// Token: 0x04000489 RID: 1161
				Alpha1,
				// Token: 0x0400048A RID: 1162
				Alpha2,
				// Token: 0x0400048B RID: 1163
				Alpha3,
				// Token: 0x0400048C RID: 1164
				Alpha4,
				// Token: 0x0400048D RID: 1165
				Alpha5,
				// Token: 0x0400048E RID: 1166
				Alpha6,
				// Token: 0x0400048F RID: 1167
				Alpha7,
				// Token: 0x04000490 RID: 1168
				Alpha8,
				// Token: 0x04000491 RID: 1169
				Alpha9,
				// Token: 0x04000492 RID: 1170
				Exclaim = 33,
				// Token: 0x04000493 RID: 1171
				DoubleQuote,
				// Token: 0x04000494 RID: 1172
				Hash,
				// Token: 0x04000495 RID: 1173
				Dollar,
				// Token: 0x04000496 RID: 1174
				Percent,
				// Token: 0x04000497 RID: 1175
				Ampersand,
				// Token: 0x04000498 RID: 1176
				Quote,
				// Token: 0x04000499 RID: 1177
				LeftParen,
				// Token: 0x0400049A RID: 1178
				RightParen,
				// Token: 0x0400049B RID: 1179
				Asterisk,
				// Token: 0x0400049C RID: 1180
				Plus,
				// Token: 0x0400049D RID: 1181
				Comma,
				// Token: 0x0400049E RID: 1182
				Minus,
				// Token: 0x0400049F RID: 1183
				Period,
				// Token: 0x040004A0 RID: 1184
				Slash,
				// Token: 0x040004A1 RID: 1185
				Colon = 58,
				// Token: 0x040004A2 RID: 1186
				Semicolon,
				// Token: 0x040004A3 RID: 1187
				Less,
				// Token: 0x040004A4 RID: 1188
				Equals,
				// Token: 0x040004A5 RID: 1189
				Greater,
				// Token: 0x040004A6 RID: 1190
				Question,
				// Token: 0x040004A7 RID: 1191
				At,
				// Token: 0x040004A8 RID: 1192
				LeftBracket = 91,
				// Token: 0x040004A9 RID: 1193
				Backslash,
				// Token: 0x040004AA RID: 1194
				RightBracket,
				// Token: 0x040004AB RID: 1195
				Caret,
				// Token: 0x040004AC RID: 1196
				Underscore,
				// Token: 0x040004AD RID: 1197
				BackQuote,
				// Token: 0x040004AE RID: 1198
				A,
				// Token: 0x040004AF RID: 1199
				B,
				// Token: 0x040004B0 RID: 1200
				C,
				// Token: 0x040004B1 RID: 1201
				D,
				// Token: 0x040004B2 RID: 1202
				E,
				// Token: 0x040004B3 RID: 1203
				F,
				// Token: 0x040004B4 RID: 1204
				G,
				// Token: 0x040004B5 RID: 1205
				H,
				// Token: 0x040004B6 RID: 1206
				I,
				// Token: 0x040004B7 RID: 1207
				J,
				// Token: 0x040004B8 RID: 1208
				K,
				// Token: 0x040004B9 RID: 1209
				L,
				// Token: 0x040004BA RID: 1210
				M,
				// Token: 0x040004BB RID: 1211
				N,
				// Token: 0x040004BC RID: 1212
				O,
				// Token: 0x040004BD RID: 1213
				P,
				// Token: 0x040004BE RID: 1214
				Q,
				// Token: 0x040004BF RID: 1215
				R,
				// Token: 0x040004C0 RID: 1216
				S,
				// Token: 0x040004C1 RID: 1217
				T,
				// Token: 0x040004C2 RID: 1218
				U,
				// Token: 0x040004C3 RID: 1219
				V,
				// Token: 0x040004C4 RID: 1220
				W,
				// Token: 0x040004C5 RID: 1221
				X,
				// Token: 0x040004C6 RID: 1222
				Y,
				// Token: 0x040004C7 RID: 1223
				Z,
				// Token: 0x040004C8 RID: 1224
				LeftCurlyBracket,
				// Token: 0x040004C9 RID: 1225
				Pipe,
				// Token: 0x040004CA RID: 1226
				RightCurlyBracket,
				// Token: 0x040004CB RID: 1227
				Tilde,
				// Token: 0x040004CC RID: 1228
				Numlock = 300,
				// Token: 0x040004CD RID: 1229
				CapsLock,
				// Token: 0x040004CE RID: 1230
				ScrollLock,
				// Token: 0x040004CF RID: 1231
				RightShift,
				// Token: 0x040004D0 RID: 1232
				LeftShift,
				// Token: 0x040004D1 RID: 1233
				RightControl,
				// Token: 0x040004D2 RID: 1234
				LeftControl,
				// Token: 0x040004D3 RID: 1235
				RightAlt,
				// Token: 0x040004D4 RID: 1236
				LeftAlt,
				// Token: 0x040004D5 RID: 1237
				LeftCommand = 310,
				// Token: 0x040004D6 RID: 1238
				LeftApple = 310,
				// Token: 0x040004D7 RID: 1239
				LeftWindows,
				// Token: 0x040004D8 RID: 1240
				RightCommand = 309,
				// Token: 0x040004D9 RID: 1241
				RightApple = 309,
				// Token: 0x040004DA RID: 1242
				RightWindows = 312,
				// Token: 0x040004DB RID: 1243
				AltGr,
				// Token: 0x040004DC RID: 1244
				Help = 315,
				// Token: 0x040004DD RID: 1245
				Print,
				// Token: 0x040004DE RID: 1246
				SysReq,
				// Token: 0x040004DF RID: 1247
				Break,
				// Token: 0x040004E0 RID: 1248
				Menu,
				// Token: 0x040004E1 RID: 1249
				Mouse0 = 323,
				// Token: 0x040004E2 RID: 1250
				Mouse1,
				// Token: 0x040004E3 RID: 1251
				Mouse2,
				// Token: 0x040004E4 RID: 1252
				Mouse3,
				// Token: 0x040004E5 RID: 1253
				Mouse4,
				// Token: 0x040004E6 RID: 1254
				Mouse5,
				// Token: 0x040004E7 RID: 1255
				Mouse6,
				// Token: 0x040004E8 RID: 1256
				JoystickButton0,
				// Token: 0x040004E9 RID: 1257
				JoystickButton1,
				// Token: 0x040004EA RID: 1258
				JoystickButton2,
				// Token: 0x040004EB RID: 1259
				JoystickButton3,
				// Token: 0x040004EC RID: 1260
				JoystickButton4,
				// Token: 0x040004ED RID: 1261
				JoystickButton5,
				// Token: 0x040004EE RID: 1262
				JoystickButton6,
				// Token: 0x040004EF RID: 1263
				JoystickButton7,
				// Token: 0x040004F0 RID: 1264
				JoystickButton8,
				// Token: 0x040004F1 RID: 1265
				JoystickButton9,
				// Token: 0x040004F2 RID: 1266
				JoystickButton10,
				// Token: 0x040004F3 RID: 1267
				JoystickButton11,
				// Token: 0x040004F4 RID: 1268
				JoystickButton12,
				// Token: 0x040004F5 RID: 1269
				JoystickButton13,
				// Token: 0x040004F6 RID: 1270
				JoystickButton14,
				// Token: 0x040004F7 RID: 1271
				JoystickButton15,
				// Token: 0x040004F8 RID: 1272
				JoystickButton16,
				// Token: 0x040004F9 RID: 1273
				JoystickButton17,
				// Token: 0x040004FA RID: 1274
				JoystickButton18,
				// Token: 0x040004FB RID: 1275
				JoystickButton19,
				// Token: 0x040004FC RID: 1276
				Joystick1Button0,
				// Token: 0x040004FD RID: 1277
				Joystick1Button1,
				// Token: 0x040004FE RID: 1278
				Joystick1Button2,
				// Token: 0x040004FF RID: 1279
				Joystick1Button3,
				// Token: 0x04000500 RID: 1280
				Joystick1Button4,
				// Token: 0x04000501 RID: 1281
				Joystick1Button5,
				// Token: 0x04000502 RID: 1282
				Joystick1Button6,
				// Token: 0x04000503 RID: 1283
				Joystick1Button7,
				// Token: 0x04000504 RID: 1284
				Joystick1Button8,
				// Token: 0x04000505 RID: 1285
				Joystick1Button9,
				// Token: 0x04000506 RID: 1286
				Joystick1Button10,
				// Token: 0x04000507 RID: 1287
				Joystick1Button11,
				// Token: 0x04000508 RID: 1288
				Joystick1Button12,
				// Token: 0x04000509 RID: 1289
				Joystick1Button13,
				// Token: 0x0400050A RID: 1290
				Joystick1Button14,
				// Token: 0x0400050B RID: 1291
				Joystick1Button15,
				// Token: 0x0400050C RID: 1292
				Joystick1Button16,
				// Token: 0x0400050D RID: 1293
				Joystick1Button17,
				// Token: 0x0400050E RID: 1294
				Joystick1Button18,
				// Token: 0x0400050F RID: 1295
				Joystick1Button19,
				// Token: 0x04000510 RID: 1296
				Joystick2Button0,
				// Token: 0x04000511 RID: 1297
				Joystick2Button1,
				// Token: 0x04000512 RID: 1298
				Joystick2Button2,
				// Token: 0x04000513 RID: 1299
				Joystick2Button3,
				// Token: 0x04000514 RID: 1300
				Joystick2Button4,
				// Token: 0x04000515 RID: 1301
				Joystick2Button5,
				// Token: 0x04000516 RID: 1302
				Joystick2Button6,
				// Token: 0x04000517 RID: 1303
				Joystick2Button7,
				// Token: 0x04000518 RID: 1304
				Joystick2Button8,
				// Token: 0x04000519 RID: 1305
				Joystick2Button9,
				// Token: 0x0400051A RID: 1306
				Joystick2Button10,
				// Token: 0x0400051B RID: 1307
				Joystick2Button11,
				// Token: 0x0400051C RID: 1308
				Joystick2Button12,
				// Token: 0x0400051D RID: 1309
				Joystick2Button13,
				// Token: 0x0400051E RID: 1310
				Joystick2Button14,
				// Token: 0x0400051F RID: 1311
				Joystick2Button15,
				// Token: 0x04000520 RID: 1312
				Joystick2Button16,
				// Token: 0x04000521 RID: 1313
				Joystick2Button17,
				// Token: 0x04000522 RID: 1314
				Joystick2Button18,
				// Token: 0x04000523 RID: 1315
				Joystick2Button19,
				// Token: 0x04000524 RID: 1316
				Joystick3Button0,
				// Token: 0x04000525 RID: 1317
				Joystick3Button1,
				// Token: 0x04000526 RID: 1318
				Joystick3Button2,
				// Token: 0x04000527 RID: 1319
				Joystick3Button3,
				// Token: 0x04000528 RID: 1320
				Joystick3Button4,
				// Token: 0x04000529 RID: 1321
				Joystick3Button5,
				// Token: 0x0400052A RID: 1322
				Joystick3Button6,
				// Token: 0x0400052B RID: 1323
				Joystick3Button7,
				// Token: 0x0400052C RID: 1324
				Joystick3Button8,
				// Token: 0x0400052D RID: 1325
				Joystick3Button9,
				// Token: 0x0400052E RID: 1326
				Joystick3Button10,
				// Token: 0x0400052F RID: 1327
				Joystick3Button11,
				// Token: 0x04000530 RID: 1328
				Joystick3Button12,
				// Token: 0x04000531 RID: 1329
				Joystick3Button13,
				// Token: 0x04000532 RID: 1330
				Joystick3Button14,
				// Token: 0x04000533 RID: 1331
				Joystick3Button15,
				// Token: 0x04000534 RID: 1332
				Joystick3Button16,
				// Token: 0x04000535 RID: 1333
				Joystick3Button17,
				// Token: 0x04000536 RID: 1334
				Joystick3Button18,
				// Token: 0x04000537 RID: 1335
				Joystick3Button19,
				// Token: 0x04000538 RID: 1336
				Joystick4Button0,
				// Token: 0x04000539 RID: 1337
				Joystick4Button1,
				// Token: 0x0400053A RID: 1338
				Joystick4Button2,
				// Token: 0x0400053B RID: 1339
				Joystick4Button3,
				// Token: 0x0400053C RID: 1340
				Joystick4Button4,
				// Token: 0x0400053D RID: 1341
				Joystick4Button5,
				// Token: 0x0400053E RID: 1342
				Joystick4Button6,
				// Token: 0x0400053F RID: 1343
				Joystick4Button7,
				// Token: 0x04000540 RID: 1344
				Joystick4Button8,
				// Token: 0x04000541 RID: 1345
				Joystick4Button9,
				// Token: 0x04000542 RID: 1346
				Joystick4Button10,
				// Token: 0x04000543 RID: 1347
				Joystick4Button11,
				// Token: 0x04000544 RID: 1348
				Joystick4Button12,
				// Token: 0x04000545 RID: 1349
				Joystick4Button13,
				// Token: 0x04000546 RID: 1350
				Joystick4Button14,
				// Token: 0x04000547 RID: 1351
				Joystick4Button15,
				// Token: 0x04000548 RID: 1352
				Joystick4Button16,
				// Token: 0x04000549 RID: 1353
				Joystick4Button17,
				// Token: 0x0400054A RID: 1354
				Joystick4Button18,
				// Token: 0x0400054B RID: 1355
				Joystick4Button19,
				// Token: 0x0400054C RID: 1356
				Joystick5Button0,
				// Token: 0x0400054D RID: 1357
				Joystick5Button1,
				// Token: 0x0400054E RID: 1358
				Joystick5Button2,
				// Token: 0x0400054F RID: 1359
				Joystick5Button3,
				// Token: 0x04000550 RID: 1360
				Joystick5Button4,
				// Token: 0x04000551 RID: 1361
				Joystick5Button5,
				// Token: 0x04000552 RID: 1362
				Joystick5Button6,
				// Token: 0x04000553 RID: 1363
				Joystick5Button7,
				// Token: 0x04000554 RID: 1364
				Joystick5Button8,
				// Token: 0x04000555 RID: 1365
				Joystick5Button9,
				// Token: 0x04000556 RID: 1366
				Joystick5Button10,
				// Token: 0x04000557 RID: 1367
				Joystick5Button11,
				// Token: 0x04000558 RID: 1368
				Joystick5Button12,
				// Token: 0x04000559 RID: 1369
				Joystick5Button13,
				// Token: 0x0400055A RID: 1370
				Joystick5Button14,
				// Token: 0x0400055B RID: 1371
				Joystick5Button15,
				// Token: 0x0400055C RID: 1372
				Joystick5Button16,
				// Token: 0x0400055D RID: 1373
				Joystick5Button17,
				// Token: 0x0400055E RID: 1374
				Joystick5Button18,
				// Token: 0x0400055F RID: 1375
				Joystick5Button19,
				// Token: 0x04000560 RID: 1376
				Joystick6Button0,
				// Token: 0x04000561 RID: 1377
				Joystick6Button1,
				// Token: 0x04000562 RID: 1378
				Joystick6Button2,
				// Token: 0x04000563 RID: 1379
				Joystick6Button3,
				// Token: 0x04000564 RID: 1380
				Joystick6Button4,
				// Token: 0x04000565 RID: 1381
				Joystick6Button5,
				// Token: 0x04000566 RID: 1382
				Joystick6Button6,
				// Token: 0x04000567 RID: 1383
				Joystick6Button7,
				// Token: 0x04000568 RID: 1384
				Joystick6Button8,
				// Token: 0x04000569 RID: 1385
				Joystick6Button9,
				// Token: 0x0400056A RID: 1386
				Joystick6Button10,
				// Token: 0x0400056B RID: 1387
				Joystick6Button11,
				// Token: 0x0400056C RID: 1388
				Joystick6Button12,
				// Token: 0x0400056D RID: 1389
				Joystick6Button13,
				// Token: 0x0400056E RID: 1390
				Joystick6Button14,
				// Token: 0x0400056F RID: 1391
				Joystick6Button15,
				// Token: 0x04000570 RID: 1392
				Joystick6Button16,
				// Token: 0x04000571 RID: 1393
				Joystick6Button17,
				// Token: 0x04000572 RID: 1394
				Joystick6Button18,
				// Token: 0x04000573 RID: 1395
				Joystick6Button19,
				// Token: 0x04000574 RID: 1396
				Joystick7Button0,
				// Token: 0x04000575 RID: 1397
				Joystick7Button1,
				// Token: 0x04000576 RID: 1398
				Joystick7Button2,
				// Token: 0x04000577 RID: 1399
				Joystick7Button3,
				// Token: 0x04000578 RID: 1400
				Joystick7Button4,
				// Token: 0x04000579 RID: 1401
				Joystick7Button5,
				// Token: 0x0400057A RID: 1402
				Joystick7Button6,
				// Token: 0x0400057B RID: 1403
				Joystick7Button7,
				// Token: 0x0400057C RID: 1404
				Joystick7Button8,
				// Token: 0x0400057D RID: 1405
				Joystick7Button9,
				// Token: 0x0400057E RID: 1406
				Joystick7Button10,
				// Token: 0x0400057F RID: 1407
				Joystick7Button11,
				// Token: 0x04000580 RID: 1408
				Joystick7Button12,
				// Token: 0x04000581 RID: 1409
				Joystick7Button13,
				// Token: 0x04000582 RID: 1410
				Joystick7Button14,
				// Token: 0x04000583 RID: 1411
				Joystick7Button15,
				// Token: 0x04000584 RID: 1412
				Joystick7Button16,
				// Token: 0x04000585 RID: 1413
				Joystick7Button17,
				// Token: 0x04000586 RID: 1414
				Joystick7Button18,
				// Token: 0x04000587 RID: 1415
				Joystick7Button19,
				// Token: 0x04000588 RID: 1416
				Joystick8Button0,
				// Token: 0x04000589 RID: 1417
				Joystick8Button1,
				// Token: 0x0400058A RID: 1418
				Joystick8Button2,
				// Token: 0x0400058B RID: 1419
				Joystick8Button3,
				// Token: 0x0400058C RID: 1420
				Joystick8Button4,
				// Token: 0x0400058D RID: 1421
				Joystick8Button5,
				// Token: 0x0400058E RID: 1422
				Joystick8Button6,
				// Token: 0x0400058F RID: 1423
				Joystick8Button7,
				// Token: 0x04000590 RID: 1424
				Joystick8Button8,
				// Token: 0x04000591 RID: 1425
				Joystick8Button9,
				// Token: 0x04000592 RID: 1426
				Joystick8Button10,
				// Token: 0x04000593 RID: 1427
				Joystick8Button11,
				// Token: 0x04000594 RID: 1428
				Joystick8Button12,
				// Token: 0x04000595 RID: 1429
				Joystick8Button13,
				// Token: 0x04000596 RID: 1430
				Joystick8Button14,
				// Token: 0x04000597 RID: 1431
				Joystick8Button15,
				// Token: 0x04000598 RID: 1432
				Joystick8Button16,
				// Token: 0x04000599 RID: 1433
				Joystick8Button17,
				// Token: 0x0400059A RID: 1434
				Joystick8Button18,
				// Token: 0x0400059B RID: 1435
				Joystick8Button19,
				Jump,
				PDA,
				Deconstruct,
				Exit,
				LeftHand,
				RightHand,
				CycleNext,
				CyclePrev,
				Slot1,
				Slot2,
				Slot3,
				Slot4,
				Slot5,
				AltTool,
				TakePicture,
				Reload,
				Sprint,
				MoveUp,
				MoveDown,
				MoveForward,
				MoveBackward,
				MoveLeft,
				MoveRight,
				AutoMove,
				LookUp,
				LookDown,
				LookLeft,
				LookRight,
				UISubmit,
				UICancel,
				UIClear,
				UIAssign,
				UILeft,
				UIRight,
				UIUp,
				UIDown,
				UIMenu,
				UIAdjustLeft,
				UIAdjustRight,
				UINextTab,
				UIPrevTab,
				Feedback,
				UIRightStickAdjustLeft,
				UIRightStickAdjustRight,
			}*/
	}
}
