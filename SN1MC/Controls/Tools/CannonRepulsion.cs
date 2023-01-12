
using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;
//More cannon aiming
namespace SN1MC.Controls.Tools
{
    class CannonRepulsion
    {
		[HarmonyPatch(typeof(PlayerTool), "OnToolUseAnim")]
		public static class PlayerToolPatchOnToolUseAnim
		{
			[HarmonyReversePatch]
			[MethodImpl(MethodImplOptions.NoInlining)]
			public static void OnToolUseAnim(GUIHand guiHand,PlayerTool instance)
			{
				ErrorMessage.AddDebug("Instance: " + instance);
			}
		}

		[HarmonyPatch(typeof(RepulsionCannon), nameof(RepulsionCannon.OnToolUseAnim))]
		public static class RepulsionCannon_OnToolUseAnim__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GUIHand guiHand, RepulsionCannon __instance)
			{
				PlayerToolPatchOnToolUseAnim.OnToolUseAnim(guiHand,__instance);
				if (__instance.energyMixin.charge > 0f)
				{
					float d = Mathf.Clamp01(__instance.energyMixin.charge / 4f);
					Vector3 forward = VRHandsController.rightController.transform.forward;//MainCamera.camera.transform.forward;
					Vector3 position = VRHandsController.rightController.transform.position;// MainCamera.camera.transform.position;
					int num = UWE.Utils.SpherecastIntoSharedBuffer(position, 1f, forward, 35f, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.UseGlobal);
					float num2 = 0f;
					for (int i = 0; i < num; i++)
					{
						RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
						Vector3 point = raycastHit.point;
						float magnitude = (position - point).magnitude;
						float d2 = 1f - Mathf.Clamp01((magnitude - 1f) / 35f);
						GameObject gameObject = UWE.Utils.GetEntityRoot(raycastHit.collider.gameObject);
						if (gameObject == null)
						{
							gameObject = raycastHit.collider.gameObject;
						}
						Rigidbody component = gameObject.GetComponent<Rigidbody>();
						if (component != null)
						{
							num2 += component.mass;
							bool flag = true;
							gameObject.GetComponents<IPropulsionCannonAmmo>(__instance.iammo);
							for (int j = 0; j < __instance.iammo.Count; j++)
							{
								if (!__instance.iammo[j].GetAllowedToShoot())
								{
									flag = false;
									break;
								}
							}
							__instance.iammo.Clear();
							if (flag && !(raycastHit.collider is MeshCollider) && (gameObject.GetComponent<Pickupable>() != null || gameObject.GetComponent<Living>() != null || (component.mass <= 1300f && UWE.Utils.GetAABBVolume(gameObject) <= 400f)))
							{
								float d3 = 1f + component.mass * 0.005f;
								Vector3 velocity = forward * d2 * d * 70f / d3;
								__instance.ShootObject(component, velocity);
							}
						}
					}
					__instance.energyMixin.ConsumeEnergy(4f);
					__instance.fxControl.Play();
					__instance.callBubblesFX = true;
					global::Utils.PlayFMODAsset(__instance.shootSound, __instance.transform, 20f);
					float d4 = Mathf.Clamp(num2 / 100f, 0f, 15f);
					Player.main.GetComponent<Rigidbody>().AddForce(-forward * d4, ForceMode.VelocityChange);
				}
				return false;
			}
		}
	}
}
