
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
//using VREnhancementsBZ;
//For the laser pointer raycast so you can point with it.
namespace SN1MC.Controls
{
	extern alias SteamVRActions;
	extern alias SteamVRRef;
	class FpsInput : PointerInputModule
	{
		private static FpsInput _main;
		public override void Process()
		{
			//throw new System.NotImplementedException();
		}

		public static FpsInput main
		{
			get
			{
				if (_main == null)
				{
					_main = new FpsInput();
				}
				return _main;
			}
		}

		public static PointerEventData.FramePressState StateForMouseButton(int buttonId)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(buttonId);
			bool mouseButtonUp = Input.GetMouseButtonUp(buttonId);
			if (mouseButtonDown && mouseButtonUp)
			{
				return PointerEventData.FramePressState.PressedAndReleased;
			}
			if (mouseButtonDown)
			{
				return PointerEventData.FramePressState.Pressed;
			}
			if (mouseButtonUp)
			{
				return PointerEventData.FramePressState.Released;
			}
			return PointerEventData.FramePressState.NotChanged;
		}
		public static Dictionary<int, PointerEventData> m_PointerData = new Dictionary<int, PointerEventData>();
		public static List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();
		public static List<RaycastResult> m_RaycastResultCache1 = new List<RaycastResult>();
		public static bool GetPointerData(int id, out PointerEventData data, bool create)
		{
			if (!m_PointerData.TryGetValue(id, out data) && create)
			{
				data = new PointerEventData(EventSystem.current)
				{
					pointerId = id
				};
				m_PointerData.Add(id, data);
				//ErrorMessage.AddDebug("Count: " + m_PointerData.Count);
				return true;
			}
			return false;
		}
		public static RaycastResult lastRaycastResult1;
		public static Vector2 pointerPosition1 = Vector2.zero;
		[HarmonyPatch(typeof(FPSInputModule), nameof(FPSInputModule.GetMousePointerEventData))]
		class FPSInputModule_GetMousePointerEventData_Patch
		{
			static bool Prefix(ref PointerInputModule.MouseState __result, FPSInputModule __instance)
			{
				PointerEventData pointerEventData;
				//PointerEventData pointerEvent;
				GetPointerData(-1, out pointerEventData, true);
				//GetPointerData(-2, out pointerEvent, true);
				pointerEventData.Reset();
				//pointerEvent.Reset();
				Vector2 cursorScreenPosition = __instance.GetCursorScreenPosition();
				Vector2 cursorScreenPositionLeft = GetCursorScreenPositionLeft();
				pointerEventData.delta = Vector2.zero;
				pointerEventData.position = cursorScreenPosition;
				pointerEventData.scrollDelta = new Vector2(GameInput.axisValues[1], -GameInput.axisValues[1]);
				pointerEventData.button = PointerEventData.InputButton.Left;

				//pointerEvent.delta = Vector2.zero;
				//pointerEvent.position = cursorScreenPositionLeft;
				m_RaycastResultCache.Clear();
				RaycastResult raycastResult = default(RaycastResult);
				//RaycastResult raycast = default(RaycastResult);
				if (__instance.lastGroup == null || !__instance.lastGroup.Raycast(pointerEventData, m_RaycastResultCache) /*|| !__instance.lastGroup.Raycast(pointerEvent, m_RaycastResultCache)*/)
				{
					EventSystem.current.RaycastAll(pointerEventData, m_RaycastResultCache);
					//EventSystem.current.RaycastAll(pointerEvent, m_RaycastResultCache);
				}
				m_RaycastResultCache.Sort(FPSInputModule.s_RaycastComparer);
				raycastResult = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
				//raycast = raycastResult;
				raycastResult.screenPosition = cursorScreenPosition;
				//raycast.screenPosition = cursorScreenPositionLeft;
				m_RaycastResultCache.Clear();
				if (raycastResult.isValid)
				{
					Camera eventCamera = raycastResult.module.eventCamera;
					if (eventCamera != null)
					{
						raycastResult.worldPosition = eventCamera.ScreenPointToRay(raycastResult.screenPosition).GetPoint(raycastResult.distance);
					}
				}
				/*if (raycast.isValid)
				{
					Camera eventCamera = raycast.module.eventCamera;
					if (eventCamera != null)
					{
						raycast.worldPosition = eventCamera.ScreenPointToRay(raycast.screenPosition).GetPoint(raycast.distance);
					}
				}*/
				pointerEventData.pointerCurrentRaycast = raycastResult;
				//pointerEvent.pointerCurrentRaycast = raycast;
				Vector2 zero = Vector2.zero;
				//Vector2 zero1 = Vector2.zero;
				if (FPSInputModule.ScreenToCanvasPoint(raycastResult, cursorScreenPosition, ref zero))
				{
					pointerEventData.delta = zero - __instance.pointerPosition;
					__instance.pointerPosition = zero;
					__instance.lastRaycastResult = raycastResult;
					__instance.lastValidRaycastTime = Time.unscaledTime;
				}
				else if (FPSInputModule.ScreenToCanvasPoint(__instance.lastRaycastResult, cursorScreenPosition, ref zero))
				{
					pointerEventData.delta = zero - __instance.pointerPosition;
					__instance.pointerPosition = zero;
					__instance.lastRaycastResult.screenPosition = cursorScreenPosition;
				}

				/*else if (FPSInputModule.ScreenToCanvasPoint(raycast, cursorScreenPositionLeft, ref zero1))
				{
					pointerEvent.delta = zero1 - __instance.pointerPosition;
					__instance.pointerPosition = zero1;
					__instance.lastRaycastResult = raycast;
					__instance.lastValidRaycastTime = Time.unscaledTime;
				}
				else if (FPSInputModule.ScreenToCanvasPoint(__instance.lastRaycastResult, cursorScreenPositionLeft, ref zero1))
				{
					pointerEvent.delta = zero1 - __instance.pointerPosition;
					__instance.pointerPosition = zero1;
					__instance.lastRaycastResult.screenPosition = cursorScreenPositionLeft;
				}*/
				else
				{
					__instance.lastRaycastResult = default(RaycastResult);
				}

				CursorManager.SetRaycastResult(__instance.lastRaycastResult);
				__instance.UpdateMouseState(pointerEventData);
				//__instance.UpdateMouseState(pointerEvent);
				__result = __instance.m_MouseState;
				return false;
			}
		}

		public static void ColliderRayCast(FPSInputModule __instance)
        {
			Ray raycast = new Ray(VRHandsController.rightController.transform.position, VRHandsController.rightController.transform.forward);
			var layerNames = new string[] { "Default", "Interior", "TerrainCollider", "Trigger", "UI", "Useable" };
			var layerMask = LayerMask.GetMask(layerNames);
			//bool triggerHit = Physics.Raycast(raycast, out triggerObject, __instance.maxInteractionDistance, layerMask);

			VRHandsController.laserPointer.LaserPointerSet(triggerObject.point);
			ErrorMessage.AddDebug("Collider Distance: " + triggerObject.distance);
			ErrorMessage.AddDebug("Collider Raycast");
			//return Player.main.camRoot.mainCam.WorldToScreenPoint(VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * FPSInputModule.current.maxInteractionDistance);
		}

		public static Vector3 UIRayCast(FPSInputModule __instance)
        {
			VRHandsController.laserPointer.LaserPointerSet(Player.main.camRoot.mainCam.ScreenPointToRay(__instance.lastRaycastResult.screenPosition).GetPoint(__instance.lastRaycastResult.distance));
			FPSInputModule.current.lastRaycastResult.Clear();
			ErrorMessage.AddDebug("GUI/PDA Distance: " + __instance.lastRaycastResult.distance);
			ErrorMessage.AddDebug("GUI/PDA Raycast");
			return Player.main.camRoot.mainCam.WorldToScreenPoint(VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * FPSInputModule.current.maxInteractionDistance);
		}

		[HarmonyPatch(typeof(FPSInputModule), nameof(FPSInputModule.GetCursorScreenPosition))]
		class GetCursorScreenPosition_Patch
		{
			public static Vector2 result;

			static bool Prefix(ref Vector2 __result, FPSInputModule __instance)
			{
				if (XRSettings.enabled)
				{
					if (VRCustomOptionsMenu.EnableMotionControls)
					{
						if (VRMenuController.rightController != null && VRMenuController.laserPointer != null)
						{
							if (uGUI.main)
							{
								result = VRMenuController.MainMenuRaycast();
							}
						}
						if (VRHandsController.laserPointer != null && VRHandsController.laserPointerLeft != null)
						{
							Ray raycast = new Ray(VRHandsController.rightController.transform.position, VRHandsController.rightController.transform.forward);
							var layerNames = new string[] { "SubRigidbodyExclude", "Interior", "TerrainCollider", "Trigger", "UI", "Useable", "Default" };
							var layerMask = LayerMask.GetMask(layerNames);
							bool triggerHit = Physics.Raycast(raycast, out triggerObject, __instance.maxInteractionDistance, layerMask);
							if (Player.main.GetPDA().isInUse && !IngameMenu.main.isActiveAndEnabled)
								result = Camera.main.WorldToScreenPoint(VRHandsController.rightController.transform.position - VRHandsController.rightController.transform.up * FPSInputModule.current.maxInteractionDistance);
							else
								result = Camera.main.WorldToScreenPoint(VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * FPSInputModule.current.maxInteractionDistance);
							if (__instance.lastRaycastResult.distance > 0 && __instance.lastRaycastResult.distance < FPSInputModule.current.maxInteractionDistance)
							{
								SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRUI.Activate();
								SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRMain.Deactivate();
								VRHandsController.laserPointer.LaserPointerSet(Camera.main.ScreenPointToRay(__instance.lastRaycastResult.screenPosition).GetPoint(__instance.lastRaycastResult.distance));
								FPSInputModule.current.lastRaycastResult.Clear();
							}
							else if (triggerObject.distance > 1 && triggerObject.distance < FPSInputModule.current.maxInteractionDistance)
							{

								SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRUI.Deactivate();
								SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRMain.Activate();

								if (Player.main.GetPDA().isInUse && !IngameMenu.main.isActiveAndEnabled)
									VRHandsController.laserPointer.LaserPointerSet(VRHandsController.rightController.transform.position - VRHandsController.rightController.transform.up * FPSInputModule.current.maxInteractionDistance);
								else
									VRHandsController.laserPointer.LaserPointerSet(triggerObject.point);
							}
							else
							{
								SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRUI.Deactivate();
								SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRMain.Activate();
								if (Player.main.GetPDA().isInUse && !IngameMenu.main.isActiveAndEnabled)
									VRHandsController.laserPointer.LaserPointerSet(VRHandsController.rightController.transform.position - VRHandsController.rightController.transform.up * FPSInputModule.current.maxInteractionDistance);
								else
									VRHandsController.laserPointer.LaserPointerSet(VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * FPSInputModule.current.maxInteractionDistance);
							}

						}
					}
					if (Cursor.lockState == CursorLockMode.Locked && !VRCustomOptionsMenu.EnableMotionControls)
					{
						Vector3 pos1 = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * FPSInputModule.current.maxInteractionDistance;
						result = MainCamera.camera.WorldToScreenPoint(pos1);
					}
					if (!VROptions.gazeBasedCursor)
					{
						result = new Vector2(Input.mousePosition.x / Screen.width * Camera.current.pixelWidth, Input.mousePosition.y / Screen.height * Camera.current.pixelHeight);
					}
				}

				__result = result;
				return false;
			}
		}
		public static Vector3 result;
		public static RaycastHit triggerObject;
		public static Vector3 GetCursorScreenPositionLeft()
		{
			if (XRSettings.enabled)
			{
				if (VRCustomOptionsMenu.EnableMotionControls)
				{
					if (VRHandsController.leftController != null)
					{
						Ray raycast = new Ray(VRHandsController.leftController.transform.position, VRHandsController.leftController.transform.forward);
						var layerNames = new string[] { "Default", "Interior", "TerrainCollider", "Trigger", "UI", "Useable" };
						var layerMask = LayerMask.GetMask(layerNames);
						bool triggerHit = Physics.Raycast(raycast, out triggerObject, FPSInputModule.current.maxInteractionDistance, layerMask);

						if (VRHandsController.laserPointerLeft != null)
						{
							result = Camera.main.WorldToScreenPoint(VRHandsController.leftController.transform.position + VRHandsController.leftController.transform.forward * FPSInputModule.current.maxInteractionDistance);
							if (FpsInput.lastRaycastResult1.distance > 0 && FpsInput.lastRaycastResult1.distance < FPSInputModule.current.maxInteractionDistance)
							{
								VRHandsController.laserPointerLeft.LaserPointerSet(Camera.main.ScreenPointToRay(FpsInput.lastRaycastResult1.screenPosition).GetPoint(FpsInput.lastRaycastResult1.distance));
								FpsInput.lastRaycastResult1.Clear();
							}
							else if (triggerObject.distance > 0 && triggerObject.distance < FPSInputModule.current.maxInteractionDistance)
							{
								VRHandsController.laserPointerLeft.LaserPointerSet(triggerObject.point);
							}
							else
							{
								VRHandsController.laserPointerLeft.LaserPointerSet(VRHandsController.leftController.transform.position + VRHandsController.leftController.transform.forward * FPSInputModule.current.maxInteractionDistance);
							}
						}
					}
				}
				if (Cursor.lockState == CursorLockMode.Locked && !VRCustomOptionsMenu.EnableMotionControls)
				{
					Vector3 pos1 = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * FPSInputModule.current.maxInteractionDistance;
					result = MainCamera.camera.WorldToScreenPoint(pos1);
				}
				if (!VROptions.gazeBasedCursor)
				{
					result = new Vector2(Input.mousePosition.x / Screen.width * Camera.current.pixelWidth, Input.mousePosition.y / Screen.height * Camera.current.pixelHeight);
				}
			}
			return result;
		}

		[HarmonyPatch(typeof(FPSInputModule), nameof(FPSInputModule.UpdateCursor))]
		class UpdateCursor_Pre_Patch
		{
			static bool Prefix(FPSInputModule __instance)
			{
				float num = 0.5f;
				float num2 = 0.1f;
				float num3 = Time.unscaledTime - __instance.lastValidRaycastTime;
				bool flag = __instance.lastGroup != null;
				if (!VROptions.GetUseGazeBasedCursor())
				{
					//flag = false;
				}

				Vector3 worldPosition = __instance.lastRaycastResult.worldPosition;
				Vector3 worldPosition1 = __instance.lastRaycastResult.worldPosition;
				if (num3 > 0f)
				{
					if (num3 > num + num2)
					{
						flag = false;
					}
					else
					{
						Vector2 cursorScreenPosition = __instance.GetCursorScreenPosition();
						Vector2 cursorScreenPositionLeft = GetCursorScreenPositionLeft();
						if (!FPSInputModule.ScreenToWorldPoint(__instance.lastRaycastResult, cursorScreenPosition, ref worldPosition))
						{
							flag = false;
						}
						if (!FPSInputModule.ScreenToWorldPoint(__instance.lastRaycastResult, cursorScreenPositionLeft, ref worldPosition1))
						{
							flag = false;
						}
					}
				}
				__instance.InitializeCursor();
				if (flag)
				{
					GameObject gameObject = __instance.lastRaycastResult.gameObject;
					if (gameObject != null)
					{
						Graphic component = gameObject.GetComponent<Graphic>();
						if (component != null)
						{
							Canvas canvas = component.canvas;
							if (canvas != null)
							{
								RectTransform component2 = canvas.GetComponent<RectTransform>();
								__instance._cursorCanvasGO.layer = gameObject.layer;
								__instance._cursorCanvasRT.localScale = component2.lossyScale;
								__instance._cursorCanvasRT.position = worldPosition;
								__instance._cursorCanvasRT.rotation = component2.rotation;
								__instance._cursorCanvas.sortingLayerID = canvas.sortingLayerID;
								__instance._cursorCanvas.sortingOrder = canvas.sortingOrder + 1;
								Color color = __instance._cursorGraphic.color;
								color.a = 1f - Mathf.Clamp01((num3 - num) / num2);
								__instance._cursorGraphic.color = color;
							}
						}
					}
				}
				if (__instance._cursorCanvasGO.activeSelf != flag)
				{
					__instance._cursorCanvasGO.SetActive(flag);
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(FPSInputModule), nameof(FPSInputModule.UpdateMouseState))]
		public static class Builder_GetAimTransform__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(PointerEventData leftData, FPSInputModule __instance)
			{
				PointerEventData pointerEventData;
				GetPointerData(-2, out pointerEventData, true);
				__instance.CopyFromTo(leftData, pointerEventData);
				pointerEventData.button = PointerEventData.InputButton.Right;
				PointerEventData pointerEventData2;
				GetPointerData(-3, out pointerEventData2, true);
				__instance.CopyFromTo(leftData, pointerEventData2);
				pointerEventData2.button = PointerEventData.InputButton.Middle;
				__instance.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
				__instance.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), pointerEventData);
				__instance.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), pointerEventData2);
				if (GameInput.GetPrimaryDevice() == GameInput.Device.Controller)
				{

					bool buttonDown = GameInput.GetButtonDown(uGUI.button0);
					bool buttonUp = GameInput.GetButtonUp(uGUI.button0);
					if (__instance.m_MouseState.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonState == PointerEventData.FramePressState.NotChanged)
					{
						__instance.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, FPSInputModule.ConstructPressState(buttonDown, buttonUp), leftData);
					}
					buttonDown = GameInput.GetButtonDown(uGUI.button1);
					buttonUp = GameInput.GetButtonUp(uGUI.button1);
					if (__instance.m_MouseState.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonState == PointerEventData.FramePressState.NotChanged)
					{
						__instance.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, FPSInputModule.ConstructPressState(buttonDown, buttonUp), pointerEventData);
					}
				}
				return false;
			}
		}
	}
}

