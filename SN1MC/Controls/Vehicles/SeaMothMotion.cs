/*
using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SN1MC.Controls.Vehicles
{
	class SeaMothMotion
	{
		[HarmonyPatch(typeof(Vehicle), "OnPilotModeBegin")]
		public static class VehicleReversePatchOnPilotModeBegin
		{
			[HarmonyReversePatch]
			[MethodImpl(MethodImplOptions.NoInlining)]
			public static void OnPilotModeBegin(Vehicle instance)
			{
				ErrorMessage.AddDebug("Instance: " + instance);
			}
		}

		[HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.OnPilotModeBegin))]
		public static class SeaMoth_OnPilotModeBegin__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(SeaMoth __instance)
			{
				VehicleReversePatchOnPilotModeBegin.OnPilotModeBegin(__instance);
				UWE.Utils.EnterPhysicsSyncSection();
				Player.main.inSeamoth = true;
				__instance.bubbles.Play();
				__instance.ambienceSound.PlayUI();
				if (__instance.leftHandPlug && __instance.rightHandPlug)
				{
					CyclopsSubControl.isPilot = true;
					Player.main.armsController.SetWorldIKTarget(__instance.leftHandPlug, __instance.rightHandPlug);
				}
				//Player.main.armsController.SetWorldIKTarget(__instance.leftHandPlug, __instance.rightHandPlug);
				if (__instance.enterSeamoth)
				{
					__instance.enterSeamoth.Play();
				}
				int num = __instance.volumeticLights.Length;
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						__instance.volumeticLights[i].DisableVolume();
					}
				}
				__instance.onLightsToggled(__instance.toggleLights.GetLightsActive());
				return false;
			}
		}

		public static Vector2 GetLookDelta()
		{
			Vector2 vector = Vector2.zero;
			if (!GameInput.scanningInput && !GameInput.clearInput)
			{
				if (GameInput.controllerEnabled)
				{
					Vector2 zero = Vector2.zero;
					float f = GameInput.GetAnalogValueForButton(GameInput.Button.LookRight) - GameInput.GetAnalogValueForButton(GameInput.Button.LookLeft);
					float f2 = GameInput.GetAnalogValueForButton(GameInput.Button.LookUp) - GameInput.GetAnalogValueForButton(GameInput.Button.LookDown);
					zero.x = CustomUpdateMoveDirection.GetMoveDirection().x; //Mathf.Sign(f) * Mathf.Pow(Mathf.Abs(f), 2f) * 500f * GameInput.controllerSensitivity.x * Time.deltaTime;
					zero.y = CustomUpdateMoveDirection.GetMoveDirection().y;// Mathf.Sign(f2) * Mathf.Pow(Mathf.Abs(f2), 2f) * 500f * GameInput.controllerSensitivity.y * Time.deltaTime;
					if (GameInput.invertController)
					{
						zero.y = -zero.y;
					}
					vector += zero;
				}
				if (GameInput.IsKeyboardAvailable())
				{
					float num = GameInput.mouseSensitivity;
					float num2 = GameInput.mouseSensitivity;
					Vector2 zero2 = Vector2.zero;
					zero2.x += GameInput.axisValues[8] * num2;
					zero2.y += GameInput.axisValues[9] * num;
					if (GameInput.invertMouse)
					{
						zero2.y = -zero2.y;
					}
					vector += zero2;
				}
			}
			return vector;
		}

		[HarmonyPatch(typeof(Vehicle), nameof(Vehicle.Update))]
		public static class Vehicle_Update__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(Vehicle __instance)
			{
				if (__instance.CanPilot())
				{
					__instance.steeringWheelYaw = Mathf.Lerp(__instance.steeringWheelYaw, 0f, Time.deltaTime);
					__instance.steeringWheelPitch = Mathf.Lerp(__instance.steeringWheelPitch, 0f, Time.deltaTime);
					if (__instance.mainAnimator)
					{
						__instance.mainAnimator.SetFloat("view_yaw", __instance.steeringWheelYaw * 70f);
						__instance.mainAnimator.SetFloat("view_pitch", __instance.steeringWheelPitch * 45f);
					}
				}
				if (__instance.GetPilotingMode() && __instance.CanPilot() && (__instance.moveOnLand || __instance.transform.position.y < Ocean.GetOceanLevel()))
				{
					if (CyclopsSubControl.rightHandAttached || CyclopsSubControl.leftHandAttached)
					{
						Vector2 vector = AvatarInputHandler.main.IsEnabled() ? GetLookDelta() : Vector2.zero;
						if (__instance.rightHandPlug)
							CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Right");
						if (__instance.leftHandPlug)
							CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Left");
						if (__instance.rightHandPlug && __instance.leftHandPlug)
							CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Both");
						__instance.steeringWheelYaw = Mathf.Clamp(__instance.steeringWheelYaw + vector.x * __instance.steeringReponsiveness, -1f, 1f);
						__instance.steeringWheelPitch = Mathf.Clamp(__instance.steeringWheelPitch + vector.y * __instance.steeringReponsiveness, -1f, 1f);
						if (__instance.controlSheme == Vehicle.ControlSheme.Submersible)
						{
							float d = 3f;
							__instance.useRigidbody.AddTorque(__instance.transform.up * vector.x * __instance.sidewaysTorque * 0.0015f * d, ForceMode.VelocityChange);
							__instance.useRigidbody.AddTorque(__instance.transform.right * -vector.y * __instance.sidewaysTorque * 0.0015f * d, ForceMode.VelocityChange);
							__instance.useRigidbody.AddTorque(__instance.transform.forward * -vector.x * __instance.sidewaysTorque * 0.0002f * d, ForceMode.VelocityChange);
						}
						else if ((__instance.controlSheme == Vehicle.ControlSheme.Submarine || __instance.controlSheme == Vehicle.ControlSheme.Mech) && vector.x != 0f)
						{
							__instance.useRigidbody.AddTorque(__instance.transform.up * vector.x * __instance.sidewaysTorque, ForceMode.VelocityChange);
						}
					}
				}
				bool flag = __instance.IsPowered();
				if (__instance.wasPowered != flag)
				{
					__instance.wasPowered = flag;
					__instance.OnPoweredChanged(flag);
				}
				__instance.ReplenishOxygen();
				return false;
			}
		}
		[HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
		public static class SeaMoth_Updaate__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(SeaMoth __instance)
			{
				VehicleReversePatchUpdate.Update(__instance);
				__instance.UpdateSounds();
				if (CyclopsSubControl.isPilot)
				{
					if (__instance.rightHandPlug)
					{
						float rightHandDistance = Vector3.Distance(__instance.rightHandPlug.position, VRHandsController.rightController.transform.position); if (GameInput.GetButtonHeld(GameInput.Button.MoveDown))
						{
							CyclopsSubControl.rightHandAttached = true;

							if (VRHandsController.rightController.transform.parent != __instance.rightHandPlug)
							{
								ErrorMessage.AddDebug("Parented Right");
								CyclopsSubControl.SetParent(VRHandsController.rightController.transform, __instance.rightHandPlug, true);
							}
							VRHandsController.rightController.transform.localPosition = __instance.rightHandPlug.localPosition;
							CyclopsSubControl.SetWorldIKTargetRight(__instance.rightHandPlug);
						}
						if (GameInput.GetButtonUp(GameInput.Button.MoveDown))
						{
							CyclopsSubControl.rightHandAttached = false;
							if (VRHandsController.rightController.transform.parent != Player.main.camRoot.transform)
							{
								CyclopsSubControl.SetParent(VRHandsController.rightController.transform, Player.main.camRoot.transform, false);
							}
							CyclopsSubControl.SetWorldIKTargetRight(__instance.rightHandPlug);
						}
					}
					if (__instance.leftHandPlug)
					{
						float leftHandDistance = Vector3.Distance(__instance.leftHandPlug.position, VRHandsController.leftController.transform.position);
						if (GameInput.GetButtonHeld(GameInput.Button.MoveUp))
						{
							CyclopsSubControl.leftHandAttached = true;
							if (VRHandsController.leftController.transform.parent != __instance.leftHandPlug)
							{
								ErrorMessage.AddDebug("Parented Left");
								CyclopsSubControl.SetParent(VRHandsController.leftController.transform, __instance.leftHandPlug, true);
							}
							VRHandsController.leftController.transform.localPosition = __instance.leftHandPlug.localPosition;
							CyclopsSubControl.SetWorldIKTargetLeft(__instance.leftHandPlug);
						}
						if (GameInput.GetButtonUp(GameInput.Button.MoveUp))
						{
							CyclopsSubControl.leftHandAttached = false;
							if (VRHandsController.leftController.transform.parent != Player.main.camRoot.transform)
							{
								CyclopsSubControl.SetParent(VRHandsController.leftController.transform, Player.main.camRoot.transform, false);
							}
							CyclopsSubControl.SetWorldIKTargetLeft(__instance.leftHandPlug);
						}
					}
					if (CyclopsSubControl.rightHandAttached)
						CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, null, "Right");
					if (CyclopsSubControl.leftHandAttached)
						CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Left");
					if (CyclopsSubControl.leftHandAttached && CyclopsSubControl.rightHandAttached)
						CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Both");
				}
				if (__instance.GetPilotingMode() && !__instance.ignoreInput)
				{
					string buttonFormat = LanguageCache.GetButtonFormat("PressToExit", GameInput.Button.Exit);
					HandReticle.main.SetTextRaw(HandReticle.TextType.Use, buttonFormat);
					HandReticle.main.SetTextRaw(HandReticle.TextType.UseSubscript, string.Empty);
					Vector3 vector = AvatarInputHandler.main.IsEnabled() ? CustomUpdateMoveDirection.GetMoveDirection() : Vector3.zero;
					if (vector.magnitude > 0.1f)
					{
						__instance.ConsumeEngineEnergy(Time.deltaTime * __instance.enginePowerConsumption * vector.magnitude);
					}
					__instance.toggleLights.CheckLightToggle();
				}
				__instance.UpdateScreenFX();
				__instance.UpdateDockedAnim();
				return false;
			}

			[HarmonyPatch(typeof(Vehicle), "Update")]
			public static class VehicleReversePatchUpdate
			{
				[HarmonyReversePatch]
				[MethodImpl(MethodImplOptions.NoInlining)]
				public static void Update(Vehicle instance)
				{
					ErrorMessage.AddDebug("Instance: " + instance);
				}
			}
		}
		[HarmonyPatch(typeof(Vehicle), nameof(Vehicle.ApplyPhysicsMove))]
		public static class Vehicle_ApplyPhysicsMove__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(Vehicle __instance)
			{
				if (__instance.GetPilotingMode())
				{
					if (__instance.worldForces.IsAboveWater() != __instance.wasAboveWater)
					{
						__instance.PlaySplashSound();
						__instance.wasAboveWater = __instance.worldForces.IsAboveWater();
					}
					bool flag = __instance.transform.position.y < Ocean.GetOceanLevel() && __instance.transform.position.y < __instance.worldForces.waterDepth && !__instance.precursorOutOfWater;
					if (__instance.moveOnLand || flag)
					{
						if (__instance.controlSheme == Vehicle.ControlSheme.Submersible)
						{
							Vector3 vector = Vector3.zero;
							if (__instance.IsAutopilotEnabled)
							{
								vector = __instance.CalculateAutopilotLocalWishDir();
							}
							else
							{
								if (CyclopsSubControl.rightHandAttached && CyclopsSubControl.leftHandAttached)
								{
									vector = (AvatarInputHandler.main.IsEnabled() ? CustomUpdateMoveDirection.GetMoveDirection() : Vector3.zero);
									if (CyclopsSubControl.rightHandAttached)
										CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Right");
									if (CyclopsSubControl.leftHandAttached)
										CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Left");
									if (CyclopsSubControl.leftHandAttached && CyclopsSubControl.rightHandAttached)
										CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandPlug, __instance.leftHandPlug, "Both");
								}
								else
                                {
									vector = (AvatarInputHandler.main.IsEnabled() ? GameInput.GetMoveDirection() : Vector3.zero);
								}
							}
							vector.Normalize();
							float d = Mathf.Abs(vector.x) * __instance.sidewardForce + Mathf.Max(0f, vector.z) * __instance.forwardForce + Mathf.Max(0f, -vector.z) * __instance.backwardForce + Mathf.Abs(vector.y * __instance.verticalForce);
							Vector3 force = __instance.transform.rotation * (d * vector) * Time.deltaTime;
							for (int i = 0; i < __instance.accelerationModifiers.Length; i++)
							{
								__instance.accelerationModifiers[i].ModifyAcceleration(ref force);
							}
							__instance.useRigidbody.AddForce(force, ForceMode.VelocityChange);
							return false;
						}
						if (__instance.controlSheme == Vehicle.ControlSheme.Submarine || __instance.controlSheme == Vehicle.ControlSheme.Mech)
						{
							Vector3 vector2;
							Vector3 vector3;
							if (__instance.IsAutopilotEnabled)
							{
								vector2 = __instance.CalculateAutopilotLocalWishDir();
								vector2 = Vector3.Min(Vector3.Max(vector2, -Vector3.one), Vector3.one);
								vector3 = vector2;
								vector3.y = 0f;
							}
							else
							{
								vector2 = (AvatarInputHandler.main.IsEnabled() ? GameInput.GetMoveDirection() : Vector3.zero);
								vector3 = new Vector3(vector2.x, 0f, vector2.z);
							}
							float num = Mathf.Abs(vector3.x) * __instance.sidewardForce + Mathf.Max(0f, vector3.z) * __instance.forwardForce + Mathf.Max(0f, -vector3.z) * __instance.backwardForce;
							vector3 = __instance.transform.rotation * vector3;
							vector3.y = 0f;
							vector3 = Vector3.Normalize(vector3);
							if (__instance.onGround)
							{
								vector3 = Vector3.ProjectOnPlane(vector3, __instance.surfaceNormal);
								vector3.y = Mathf.Clamp(vector3.y, -0.5f, 0.5f);
								num *= __instance.onGroundForceMultiplier;
							}
							Vector3 b = new Vector3(0f, vector2.y, 0f);
							b.y *= __instance.verticalForce * Time.deltaTime;
							Vector3 force2 = num * vector3 * Time.deltaTime + b;
							__instance.OverrideAcceleration(ref force2);
							for (int j = 0; j < __instance.accelerationModifiers.Length; j++)
							{
								__instance.accelerationModifiers[j].ModifyAcceleration(ref force2);
							}
							__instance.useRigidbody.AddForce(force2, ForceMode.VelocityChange);
						}
					}
				}
				return false;
			}
		}
	}
}*/
