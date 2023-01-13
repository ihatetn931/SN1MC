
using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Valve.VR;
//Where the magic happens for vr controlled cyclops
namespace SN1MC.Controls.Vehicles
{
	extern alias SteamVRRef;
	public static class CyclopsSubControl
    {
		
		public static bool rightHandAttached = false;
		public static bool leftHandAttached = false;
		public static bool isPilot = false;
		//public static float leftRight = 0;
		public static bool set = false;
		public static Quaternion initialObjectRotation;
		public static Quaternion initialControllerRotation;
		public static float rotationX = 0f;
		public static float sensitivityX = 2f;
		private const float VERTICAL_LIMIT = 60f;
		public static float _mVerticalTurnSpeed = 2.5f;


		[HarmonyPatch(typeof(SubControl), nameof(SubControl.Update))]
		public static class SubControl_Update__Patch
		{
			public static Rigidbody _rigidBody;
			[HarmonyPrefix]
			static bool Prefix(SubControl __instance)
			{
				if (!__instance.LOD.IsFull())
				{
					return false;
				}
				__instance.appliedThrottle = false;
				if (__instance.controlMode == SubControl.Mode.DirectInput)
				{
					//Vector3 midPoint = (rightT.position + leftT.position) * 0.5f;
					if (rightHandAttached || leftHandAttached)
					{
						if (rightT||leftT)
						{
							__instance.throttle = CustomUpdateMoveDirection.GetMoveDirection();
							if (leftT)
								 CustomUpdateMoveDirection.UpdateMoveDirection(rightT, leftT, "Left");
							if(rightT)
								CustomUpdateMoveDirection.UpdateMoveDirection(rightT, leftT, "Right");
							if (rightT && leftT)
								CustomUpdateMoveDirection.UpdateMoveDirection(rightT, leftT, "Both");
						}	
					}
					else
						__instance.throttle = GameInput.GetMoveDirection();
					if (__instance.canAccel && (double)__instance.throttle.magnitude > 0.0001)
					{
						float num = 0f;
						float amount = __instance.throttle.magnitude * __instance.cyclopsMotorMode.GetPowerConsumption() * Time.deltaTime / __instance.sub.GetPowerRating();
						if (!GameModeUtils.RequiresPower() || __instance.powerRelay.ConsumeEnergy(amount, out num))
						{
							__instance.lastTimeThrottled = Time.time;
							__instance.appliedThrottle = true;
						}
					}
					if (__instance.appliedThrottle && __instance.canAccel)
					{
						float topClamp = 0.33f;
						if (__instance.useThrottleIndex == 1)
						{
							topClamp = 0.66f;
						}
						if (__instance.useThrottleIndex == 2)
						{
							topClamp = 1f;
						}
						__instance.engineRPMManager.AccelerateInput(topClamp);
						for (int i = 0; i < __instance.throttleHandlers.Length; i++)
						{
							__instance.throttleHandlers[i].OnSubAppliedThrottle();
						}
						if (__instance.lastTimeThrottled < Time.time - 5f)
						{
							global::Utils.PlayFMODAsset(__instance.engineStartSound, MainCamera.camera.transform, 20f);
						}
					}
					if (AvatarInputHandler.main.IsEnabled())
					{
						if (GameInput.GetButtonDown(GameInput.Button.RightHand))
						{
							__instance.transform.parent.BroadcastMessage("ToggleFloodlights", null, SendMessageOptions.DontRequireReceiver);
						}
						if (GameInput.GetButtonDown(GameInput.Button.Exit))
						{
							Player.main.TryEject();
							isPilot = false;
						}
					}
				}
				if (!__instance.appliedThrottle)
				{
					__instance.throttle = new Vector3(0f, 0f, 0f);
				}
				__instance.UpdateAnimation();
				return false;
			}
		}

		[HarmonyPatch(typeof(SubControl), nameof(SubControl.UpdateAnimation))]
		public static class SubControl_UpdateAnimation__Patch
		{
			[HarmonyPrefix]
			static bool Prefix( SubControl __instance)
			{
				float b = 0f;
				float b2 = 0f;
				if ((double)Mathf.Abs(__instance.throttle.x) > 0.0001)
				{
					ShipSide useShipSide;
					b = __instance.throttle.x;
					if (Mathf.Clamp(b, -1, 1) > 0.1f)
					{
						useShipSide = ShipSide.Port;
						b = b * 90;
					}
					else
					{
						useShipSide = ShipSide.Starboard;
						b = b * 90;
					}
					/*if (__instance.throttle.x > 0f)
					{
						useShipSide = ShipSide.Port;
						//b = 90f;
						//b = __instance.throttle.x;
					}
					else
					{
						useShipSide = ShipSide.Starboard;
						//b = -90f;
							//b =- __instance.throttle.x;
					}*/
					if (__instance.throttle.x < -0.1f || __instance.throttle.x > 0.1f)
					{
						for (int i = 0; i < __instance.turnHandlers.Length; i++)
						{
							__instance.turnHandlers[i].OnSubTurn(useShipSide);
						}
					}
				}
				if ((double)Mathf.Abs(__instance.throttle.y) > 0.0001)
				{
					b2 = __instance.throttle.y;
					if (Mathf.Clamp(b2, -1, 1) > 0.1f)
					{
						b2 = b2 * 90;
					}
					else
					{
						b2 = b2 * 90;
					}
					/*if (__instance.throttle.y > 0f)
					{
						//b2 = 90f;
					}
					else
					{
						//b2 = -90f;
					}*/
				}
				__instance.steeringWheelYaw = Mathf.Lerp(__instance.steeringWheelYaw, Mathf.Clamp(b,-90,90), Time.deltaTime * __instance.steeringReponsiveness);
				__instance.steeringWheelPitch = Mathf.Lerp(__instance.steeringWheelPitch, Mathf.Clamp(b2, -90, 90), Time.deltaTime * __instance.steeringReponsiveness);
				if (__instance.mainAnimator)
				{
					__instance.mainAnimator.SetFloat("view_yaw", __instance.steeringWheelYaw);
					__instance.mainAnimator.SetFloat("view_pitch", __instance.steeringWheelPitch);
					Player.main.playerAnimator.SetFloat("cyclops_yaw", __instance.steeringWheelYaw);
					Player.main.playerAnimator.SetFloat("cyclops_pitch", __instance.steeringWheelPitch);
				}
				return false;
			}
		}

		public static void SetParent(Transform child, Transform parent, bool setting)
		{
			if (setting)
			{
				child.SetParent(parent);
				child.position = parent.position;
				child.rotation = parent.rotation;
			}
			else
            {
				child.SetParent(parent);
			}
		}

		public static void SetWorldIKTargetRight(Transform rightTarget)
		{
			Player.main.armsController.rightWorldTarget = rightTarget;
			Player.main.armsController.reconfigureWorldTarget = true;
		}

		public static void SetWorldIKTargetLeft(Transform leftTarget)
		{
			Player.main.armsController.leftWorldTarget = leftTarget;
			Player.main.armsController.reconfigureWorldTarget = true;
		}

		public static Transform rightT;
		public static Transform leftT;
		[HarmonyPatch(typeof(PilotingChair), nameof(PilotingChair.Update))]
		public static class PilotingChair_Update__Patch
		{
			[HarmonyPrefix]
			static void Postfix(PilotingChair __instance)
			{
				if (isPilot)
				{
					if (rightT == null)
						rightT = __instance.rightHandPlug;
					if (__instance.rightHandPlug)
					{
						float rightHandDistance = Vector3.Distance(__instance.rightHandPlug.position, VRHandsController.rightController.transform.position); 
						if (GameInput.GetButtonHeld(GameInput.Button.MoveDown))
						{
							rightHandAttached = true;
							if (VRHandsController.rightController.transform.parent != __instance.rightHandPlug)
							{
								//ErrorMessage.AddDebug("Parented Right");
								SetParent(VRHandsController.rightController.transform, __instance.rightHandPlug, true);
							}
							VRHandsController.rightController.transform.localPosition = __instance.rightHandPlug.localPosition;
							SetWorldIKTargetRight(__instance.rightHandPlug);
						}
						if (GameInput.GetButtonUp(GameInput.Button.MoveDown))
						{
							rightHandAttached = false;
							if (VRHandsController.rightController.transform.parent != Player.main.camRoot.transform)
							{
								SetParent(VRHandsController.rightController.transform, Player.main.camRoot.transform, false);
							}
							SetWorldIKTargetRight(rightT);
						}
					}
					if (leftT == null)
						leftT = __instance.leftHandPlug;
					if (__instance.leftHandPlug)
					{
						float leftHandDistance = Vector3.Distance(__instance.leftHandPlug.position, VRHandsController.leftController.transform.position);
						if (GameInput.GetButtonHeld(GameInput.Button.MoveUp))
						{
							leftHandAttached = true;
							if (VRHandsController.leftController.transform.parent != __instance.leftHandPlug)
							{
								//ErrorMessage.AddDebug("Parented Left");
								SetParent(VRHandsController.leftController.transform, __instance.leftHandPlug, true);
							}
							VRHandsController.leftController.transform.localPosition = __instance.leftHandPlug.localPosition;
							SetWorldIKTargetLeft(__instance.leftHandPlug);
						}
						if (GameInput.GetButtonUp(GameInput.Button.MoveUp))
						{
							leftHandAttached = false;
							if (VRHandsController.leftController.transform.parent != Player.main.camRoot.transform)
							{
								SetParent(VRHandsController.leftController.transform, Player.main.camRoot.transform, false);
							}
							SetWorldIKTargetLeft(leftT);
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(PilotingChair), nameof(PilotingChair.OnSteeringStart))]
		public static class PilotingChair_OnSteeringStart__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(CinematicModeEventData eventData, PilotingChair __instance)
			{
				if (__instance.enabled && eventData.cinematicController == __instance.cinematicController && eventData.player.GetCurrentSub() == __instance.subRoot)
				{
					if (VRCustomOptionsMenu.CyclopsPilot)
					{
						__instance.currentPlayer = eventData.player;
						__instance.currentPlayer.EnterPilotingMode(__instance, false);
						__instance.Subscribe(__instance.currentPlayer, true);
						if (__instance.leftHandPlug && __instance.rightHandPlug)
						{
							isPilot = true;
							//Player.main.armsController.SetWorldIKTarget(__instance.leftHandPlug, __instance.rightHandPlug);
						}
						UWE.Utils.GetEntityRoot(__instance.gameObject).BroadcastMessage("StartPiloting");
					}
					else
                    {
						__instance.currentPlayer = eventData.player;
						__instance.currentPlayer.EnterPilotingMode(__instance, false);
						__instance.Subscribe(__instance.currentPlayer, true);
						if (__instance.leftHandPlug && __instance.rightHandPlug)
						{
							Player.main.armsController.SetWorldIKTarget(__instance.leftHandPlug, __instance.rightHandPlug);
						}
						UWE.Utils.GetEntityRoot(__instance.gameObject).BroadcastMessage("StartPiloting");
					}
				}
				return false;
			}
		}
	}
}
