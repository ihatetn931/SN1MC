using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SN1MC.Controls.Vehicles
{
	class SeaglideMap
	{
		[HarmonyPatch(typeof(Seaglide), nameof(Seaglide.UpdateActiveState))]
		class Seaglide_UpdateActiveState_Update_Patch
		{
			static bool Prefix(Seaglide __instance)
			{
				bool flag = __instance.activeState;
				__instance.activeState = false;
				if (__instance.energyMixin.charge > 0f)
				{
					if (__instance.screenEffectModel != null)
					{
						__instance.screenEffectModel.SetActive(__instance.usingPlayer != null);
					}
					if (__instance.usingPlayer != null && __instance.usingPlayer.IsSwimming())
					{
						CustomUpdateMoveDirection.UpdateMoveDirection(__instance.rightHandIKTarget, __instance.leftHandIKTarget, "Both");
						Vector3 moveDirection = CustomUpdateMoveDirection.GetMoveDirection();// GameInput.GetMoveDirection();
						__instance.activeState = (moveDirection.x != 0f || moveDirection.z != 0f);
					}
					if (__instance.powerGlideActive)
					{
						__instance.activeState = true;
					}
				}
				if (flag != __instance.activeState)
				{
					__instance.SetVFXActive(__instance.activeState);
				}
				return false;
			}
		}
		/*[HarmonyPatch(typeof(VehicleInterface_MapController), nameof(VehicleInterface_MapController.Update))]
		class VehicleInterface_MapController_Update_Patch
		{
			static bool Prefix(VehicleInterface_MapController __instance)
			{
				if (!__instance.seaglide.HasEnergy())
				{
					__instance.miniWorld.active = false;
				}
				else if (Player.main != null && (Player.main.currentSub != null || Player.main.currentEscapePod != null || !Player.main.IsUnderwater()))
				{
					__instance.miniWorld.active = false;
				}
				else if (AvatarInputHandler.main.IsEnabled() && GameInput.GetButtonDown(GameInput.Button.AltTool))
				{
					__instance.miniWorld.active = !__instance.miniWorld.active;
				}
				__instance.seaglideIllumRenderer.GetPropertyBlock(__instance.seaglideIllumPropertyBlock, 1);
				if (__instance.miniWorld.active)
				{
					__instance.playerDot.SetActive(true);
					__instance.lightVfx.SetActive(true);
					__instance.illumColor = Color.Lerp(__instance.seaglideIllumPropertyBlock.GetColor(ShaderPropertyID._GlowColor), Color.white, Time.deltaTime);
					__instance.seaglideIllumPropertyBlock.SetColor(ShaderPropertyID._GlowColor, __instance.illumColor);
				}
				else
				{
					__instance.playerDot.SetActive(false);
					__instance.lightVfx.SetActive(false);
					__instance.illumColor = Color.Lerp(__instance.seaglideIllumPropertyBlock.GetColor(ShaderPropertyID._GlowColor), Color.black, Time.deltaTime);
					__instance.seaglideIllumPropertyBlock.SetColor(ShaderPropertyID._GlowColor, __instance.illumColor);
				}
				__instance.seaglideIllumRenderer.SetPropertyBlock(__instance.seaglideIllumPropertyBlock, 1);
				return false;
			}
		}*/
	}
}
