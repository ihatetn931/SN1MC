
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using SN1MC.Controls.Tools;

namespace SN1MC.Controls
{
	extern alias SteamVRRef;
	class HandGUI
	{
		public static GameObject activeTargetLeft;
		[HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
		public static class GUIHand_OnUpdate__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GUIHand __instance)
			{
				__instance.usedToolThisFrame = false;
				__instance.usedAltAttackThisFrame = false;
				__instance.suppressTooltip = false;
				if (__instance.player.IsFreeToInteract() && AvatarInputHandler.main.IsEnabled())
				{
					string text = string.Empty;
					PlayerTool tool = __instance.GetTool();
					EnergyMixin energyMixin = null;
					if (tool != null)
					{
						text = tool.GetCustomUseText();
						energyMixin = tool.GetComponent<EnergyMixin>();
					}
					if (energyMixin != null && energyMixin.allowBatteryReplacement)
					{
						int num = Mathf.FloorToInt(energyMixin.GetEnergyScalar() * 100f);
						if (__instance.cachedTextEnergyScalar != num)
						{
							if (num <= 0)
							{
								__instance.cachedEnergyHudText = LanguageCache.GetButtonFormat("ExchangePowerSource", GameInput.Button.Reload);
							}
							else
							{
								__instance.cachedEnergyHudText = Language.main.GetFormat<float>("PowerPercent", energyMixin.GetEnergyScalar());
							}
							__instance.cachedTextEnergyScalar = num;
						}
						HandReticle.main.SetTextRaw(HandReticle.TextType.Use, text);
						HandReticle.main.SetTextRaw(HandReticle.TextType.UseSubscript, __instance.cachedEnergyHudText);
					}
					else if (!string.IsNullOrEmpty(text))
					{
						HandReticle.main.SetTextRaw(HandReticle.TextType.Use, text);
					}
					if (!__instance.IsPDAInUse())
					{
						if (__instance.grabMode == GUIHand.GrabMode.None)
						{
							__instance.UpdateActiveTarget();
						}
						HandReticle.main.SetTargetDistance(__instance.activeHitDistance);
						if (__instance.activeTarget != null && !__instance.suppressTooltip)
						{
							TechType techType = CraftData.GetTechType(__instance.activeTarget);
							if (techType != TechType.None)
							{
								HandReticle.main.SetText(HandReticle.TextType.Hand, techType.AsString(false), true, GameInput.Button.None);
							}
							GUIHand.Send(__instance.activeTarget, HandTargetEventType.Hover, __instance);
						}
						bool flag = false;
						bool buttonHeld = false;
						bool buttonUp = false;
						bool flag2 = false;
						bool buttonHeld2 = false;
						bool buttonUp2 = false;
						bool buttonDown = false;
						bool buttonDown2 = false;
						bool buttonDown3 = false;
						bool buttonHeld3 = false;
						bool buttonUp3 = false;

						flag = GameInput.GetButtonDown(GameInput.Button.LeftHand);
						buttonHeld = GameInput.GetButtonHeld(GameInput.Button.LeftHand);
						buttonUp = GameInput.GetButtonUp(GameInput.Button.LeftHand);
						flag2 = GameInput.GetButtonDown(GameInput.Button.RightHand);
						buttonHeld2 = GameInput.GetButtonHeld(GameInput.Button.RightHand);
						buttonUp2 = GameInput.GetButtonUp(GameInput.Button.RightHand);
						buttonDown = GameInput.GetButtonDown(GameInput.Button.Reload);
						buttonDown2 = GameInput.GetButtonDown(GameInput.Button.Exit);
						buttonDown3 = GameInput.GetButtonDown(GameInput.Button.AltTool);
						buttonHeld3 = GameInput.GetButtonHeld(GameInput.Button.AltTool);
						buttonUp3 = GameInput.GetButtonUp(GameInput.Button.AltTool);

						PDAScanner.UpdateTarget(8f, buttonDown3 || buttonHeld3);
						PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
						if (scanTarget.isValid && Inventory.main.container.Contains(TechType.Scanner) && PDAScanner.CanScan() == PDAScanner.Result.Scan && !PDAScanner.scanTarget.isPlayer)
						{
							uGUI_ScannerIcon.main.Show();
						}
						if (tool != null)
						{
							if (flag2)
							{
								if (tool.OnRightHandDown())
								{
									__instance.usedToolThisFrame = true;
									tool.OnToolActionStart();
									flag2 = false;
								}
							}
							else if (buttonHeld2)
							{
								if (tool.OnRightHandHeld())
								{
									flag2 = false;
								}
							}
							else if (!buttonUp2 || tool.OnRightHandUp())
							{
							}
							if (flag)
							{
								if (tool.OnLeftHandDown())
								{
									tool.OnToolActionStart();
									flag = false;
								}
							}
							else if (buttonHeld)
							{
								if (tool.OnLeftHandHeld())
								{
									flag = false;
								}
							}
							else if (!buttonUp || tool.OnLeftHandUp())
							{
							}

							if (buttonDown3)
							{
								if (tool.OnAltDown())
								{
									__instance.usedAltAttackThisFrame = true;
									tool.OnToolActionStart();
								}
							}

							else if (buttonHeld3)
							{
								if (tool.OnAltHeld())
								{
								}
							}

							else if (!buttonUp3 || tool.OnAltUp())
							{
							}
							if (!buttonDown || tool.OnReloadDown())
							{
							}
							if (!buttonDown2 || tool.OnExitDown())
							{
							}
						}
						if (tool == null && flag2)
						{
							Inventory.main.DropHeldItem(true);
						}
						if (__instance.player.IsFreeToInteract() && !__instance.usedToolThisFrame && __instance.activeTarget != null && flag)
						{
							GUIHand.Send(__instance.activeTarget, HandTargetEventType.Click, __instance);
						}
					}
				}
				if (AvatarInputHandler.main.IsEnabled() && GameInput.GetButtonDown(GameInput.Button.AutoMove))
				{
					GameInput.SetAutoMove(!GameInput.GetAutoMove());
				}
				if (AvatarInputHandler.main.IsEnabled() && GameInput.GetButtonDown(GameInput.Button.PDA) && !uGUI.isIntro && !IntroLifepodDirector.IsActive)
				{
					__instance.player.GetPDA().Open(PDATab.None, null, null);
				}
				return false;
			}
		}


		[HarmonyPatch(typeof(GUIHand), nameof(GUIHand.GetGrabbingHandPosition))]
		public static class GUIHand_GetGrabbingHandPosition__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref Vector3 __result, GUIHand __instance)
			{
				//Camera playerCamera = __instance.GetPlayerCamera();
				__result = VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * __instance.activeHitDistance;
				if(Player.main != null)
					if(Player.main.inExosuit)
						__result = VRHandsController.leftController.transform.position + VRHandsController.leftController.transform.forward * __instance.activeHitDistance;
				return false;
			}
		}

		[HarmonyPatch(typeof(GUIHand), nameof(GUIHand.UpdateActiveTarget))]
		public static class GUIHand_UpdateActiveTarget__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GUIHand __instance)
			{
				PlayerTool tool = __instance.GetTool();
				if (tool != null && tool.GetComponent<PropulsionCannon>() != null && tool.GetComponent<PropulsionCannon>().IsGrabbingObject())
				{
					if (tool.GetComponent<PropulsionCannon>().GetInteractableGrabbedObject() != null)
					{
						__instance.activeTarget = tool.GetComponent<PropulsionCannon>().GetInteractableGrabbedObject();
					}
					__instance.suppressTooltip = true;
					return false;
				}
				if (tool != null && tool.GetComponent<PropulsionCannon>() != null && CannonCustom.IsGrabbingObjectLeft())
				{
					if (CannonCustom.GetInteractableGrabbedObjectLeft() != null)
					{
						activeTargetLeft = CannonCustom.GetInteractableGrabbedObjectLeft();
					}
					__instance.suppressTooltip = true;
					return false;
				}
				if (tool != null && tool.DoesOverrideHand())
				{
					__instance.activeTarget = null;
					__instance.activeHitDistance = 0f;
					activeTargetLeft = null;
					return false;
				}
				DebugTargetConsoleCommand.RecordNext();
				if (CustomTargeting.GetTarget(Player.main.gameObject, 2f, out __instance.activeTarget, out __instance.activeHitDistance))
				{
					IHandTarget handTarget = null;
					Transform transform = __instance.activeTarget.transform;
					while (transform != null)
					{
						handTarget = transform.GetComponent<IHandTarget>();
						if (handTarget != null)
						{
							__instance.activeTarget = transform.gameObject;
							break;
						}
						transform = transform.parent;
					}
					if (handTarget == null)
					{
						HarvestType harvestType = CraftData.GetHarvestTypeFromTech(CraftData.GetTechType(__instance.activeTarget));
						if (harvestType == HarvestType.Pick)
						{
							if (global::Utils.FindAncestorWithComponent<Pickupable>(__instance.activeTarget) == null)
							{
								LargeWorldEntity largeWorldEntity = global::Utils.FindAncestorWithComponent<LargeWorldEntity>(__instance.activeTarget);
								largeWorldEntity.gameObject.AddComponent<Pickupable>();
								largeWorldEntity.gameObject.AddComponent<WorldForces>().useRigidbody = largeWorldEntity.GetComponent<Rigidbody>();
								return false;
							}
						}
						else if (harvestType == HarvestType.None)
						{
							__instance.activeTarget = null;
							return false;
						}
					}
				}
				else if (CustomTargeting.GetTargetLeft(Player.main.gameObject, 2f, out activeTargetLeft, out __instance.activeHitDistance) && Player.main.inExosuit)
				{
					IHandTarget handTarget = null;
					Transform transform = activeTargetLeft.transform;
					while (transform != null)
					{
						handTarget = transform.GetComponent<IHandTarget>();
						if (handTarget != null)
						{
							activeTargetLeft = transform.gameObject;
							break;
						}
						transform = transform.parent;
					}
					if (handTarget == null)
					{
						HarvestType harvestType = CraftData.GetHarvestTypeFromTech(CraftData.GetTechType(activeTargetLeft));
						if (harvestType == HarvestType.Pick)
						{
							if (global::Utils.FindAncestorWithComponent<Pickupable>(activeTargetLeft) == null)
							{
								LargeWorldEntity largeWorldEntity = global::Utils.FindAncestorWithComponent<LargeWorldEntity>(activeTargetLeft);
								largeWorldEntity.gameObject.AddComponent<Pickupable>();
								largeWorldEntity.gameObject.AddComponent<WorldForces>().useRigidbody = largeWorldEntity.GetComponent<Rigidbody>();
								return false;
							}
						}
						else if (harvestType == HarvestType.None)
						{
							activeTargetLeft = null;
							return false;
						}
					}
				}
				else
				{
					__instance.activeTarget = null;
					activeTargetLeft = null;
					__instance.activeHitDistance = 0f;
				}
				return false;
			}

		}
	}
}
