
using UnityEngine;

namespace SN1MC.Controls.Tools
{
	public class Example : MonoBehaviour
	{
		Animator animator;

		void Start()
		{
			animator = GetComponent<Animator>();
			ErrorMessage.AddDebug("Animator: " + animator);
		}

		void OnAnimatorIK(int layerIndex)
		{
			ErrorMessage.AddDebug("OnAnimatorIK");
			float reach = animator.GetFloat("RightHandReach");
			animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
			animator.SetIKPosition(AvatarIKGoal.RightHand, VRHandsController.rightController.transform.position);
		}
	}
}

/*using HarmonyLib;
using UnityEngine;

namespace SN1MC.Controls.Tools
{
    class LightFlash
    {
		[HarmonyPatch(typeof(FlashLight), nameof(FlashLight.onLightsToggled))]
		public static class FlashLight_onLightsToggled__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(bool active,FlashLight __instance)
			{
				
				return false;
			}
		}
/*
		[HarmonyPatch(typeof(ToggleLights), nameof(ToggleLights.CheckLightToggle))]
		public static class ToggleLights_CheckLightToggle__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ToggleLights __instance)
			{
				if (Player.main.GetRightHandDown() && !Player.main.GetPDA().isInUse /*&& AvatarInputHandler.main.IsEnabled() && Time.time > __instance.timeLastPlayerModeChange + 1f && __instance.timeLastLightToggle + 0.25f < Time.time)
				{
					__instance.SetLightsActive(!__instance.lightsActive);
					__instance.timeLastLightToggle = Time.time;
					__instance.lightState++;
					if (__instance.lightState == __instance.maxLightStates)
					{
						__instance.lightState = 0;
					}
				}
				return false;
			}
		}
	}
}*/
