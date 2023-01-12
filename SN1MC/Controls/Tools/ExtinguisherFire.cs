
using FMOD.Studio;
using HarmonyLib;
using UnityEngine;

namespace SN1MC.Controls.Tools
{
    class ExtinguisherFire
    {
		[HarmonyPatch(typeof(FireExtinguisher), nameof(FireExtinguisher.UpdateTarget))]
		public static class FireExtinguisher_UpdateTarget__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(FireExtinguisher __instance)
			{
				__instance.fireTarget = null;
				if (__instance.usingPlayer != null)
				{
					Vector3 vector = default(Vector3);
					GameObject gameObject = null;
					Vector3 vector2;
					CustomTargeting.TraceFPSTargetPosition(Player.main.gameObject, 8f, ref gameObject, ref vector, out vector2, true);
					if (gameObject)
					{
						Fire componentInHierarchy = UWE.Utils.GetComponentInHierarchy<Fire>(gameObject);
						if (componentInHierarchy)
						{
							__instance.fireTarget = componentInHierarchy;
						}
					}
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(FireExtinguisher), nameof(FireExtinguisher.Update))]
		public static class FireExtinguisher_Update__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(FireExtinguisher __instance)
			{
				if (AvatarInputHandler.main.IsEnabled() && Player.main.IsAlive() && Player.main.GetRightHandHeld() && __instance.isDrawn)
				{
					__instance.usedThisFrame = true;
				}
				else
				{
					__instance.usedThisFrame = false;
				}
				int num = Player.main.isUnderwater.value ? 1 : 0;
				if (num != __instance.lastUnderwaterValue)
				{
					__instance.lastUnderwaterValue = num;
					if (FMODUWE.IsInvalidParameterId(__instance.fmodIndexInWater))
					{
						__instance.fmodIndexInWater = __instance.soundEmitter.GetParameterIndex("in_water");
					}
					__instance.soundEmitter.SetParameterValue(__instance.fmodIndexInWater, (float)num);
				}
				__instance.UpdateTarget();
				if (__instance.usedThisFrame && __instance.fuel > 0f)
				{
					if (Player.main.IsUnderwater())
					{
						Player.main.GetComponent<UnderwaterMotor>().SetVelocity(-VRHandsController.rightController.transform.forward * 5f);
					}
					float douseAmount = __instance.fireDousePerSecond * Time.deltaTime;
					float expendAmount = __instance.expendFuelPerSecond * Time.deltaTime;
					__instance.UseExtinguisher(douseAmount, expendAmount);
					__instance.soundEmitter.Play();
				}
				else
				{
					__instance.soundEmitter.Stop(STOP_MODE.ALLOWFADEOUT);
					if (__instance.fxControl != null)
					{
						__instance.fxControl.Stop(0);
						__instance.fxIsPlaying = false;
					}
				}
				__instance.UpdateImpactFX();
				__instance.UpdateControllerLightbarToToolBarValue();
				return false;
			}
		}
	}
}
