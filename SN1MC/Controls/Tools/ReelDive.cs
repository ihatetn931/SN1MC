
using HarmonyLib;
using UnityEngine;
//For aiming the dive reel
namespace SN1MC.Controls.Tools
{
    class ReelDive
    {
		[HarmonyPatch(typeof(DiveReel), nameof(DiveReel.CreateNewNode))]
		public static class DiveReel_CreateNewNode__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(Vector3 createPos, bool isFirst, bool loadingNode, DiveReel __instance)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.nodePrefab, createPos, MainCamera.camera.transform.rotation);
				gameObject.transform.Rotate(new Vector3(90f, 0f, 0f), Space.Self);
				DiveReelNode component = gameObject.GetComponent<DiveReelNode>();
				if (!loadingNode)
				{
					component.rb.AddForce(VRHandsController.rightController.transform.forward * 800f);
				}
				if (isFirst)
				{
					component.firstArrow = true;
				}
				else if (__instance.lastNodeTransform && component)
				{
					component.previousArrowPos = __instance.lastNodeTransform;
				}
				if (!loadingNode)
				{
					__instance.nodePositions.Add(createPos);
				}
				__instance.lastNodePos = createPos;
				__instance.nodes.Add(component);
				__instance.lastNodeTransform = gameObject.transform;
				if (__instance.nodes.Count == __instance.maxNodes)
				{
					__instance.SetDiveReelOutOfAmmoAnimator(true);
					return false;
				}
				__instance.SetDiveReelOutOfAmmoAnimator(false);
				return false;
			}
		}

		[HarmonyPatch(typeof(DiveReel), nameof(DiveReel.OnToolUseAnim))]
		public static class DiveReel_OnToolUseAnim__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GUIHand guiHand, DiveReel __instance)
			{
				if (Player.main.currentSub != null)
				{
					return false;
				}
				__instance.cooldown = true;
				__instance.Invoke("ResetCooldown", 3f);
				if (Player.main.IsBleederAttached())
				{
					return false;
				}
				if (__instance.energyMixin.IsDepleted())
				{
					return false;
				}
				if (__instance.nodes.Count < __instance.maxNodes)
				{
					Vector3 vector = __instance.deployPosLate;
					Vector3 position = VRHandsController.rightController.transform.position;//Player.main.transform.position;
					Vector3 vector2 = position - vector;
					RaycastHit raycastHit;
					if (Physics.Raycast(position, -vector2.normalized, out raycastHit, vector2.magnitude, ~(1 << LayerID.Player), QueryTriggerInteraction.Ignore))
					{
						vector = raycastHit.point + vector2.normalized * __instance.nodeRadius;
					}
					__instance.CreateNewNode(vector, __instance.nodePositions.Count == 0, false);
					__instance.energyMixin.ConsumeEnergy(__instance.energyCostPerDisc);
					__instance.fireSFX.Play();
					__instance.animationController.SetTrigger("divereel_fire");
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(DiveReel), nameof(DiveReel.UpdateEquipped))]
		public static class DiveReel_UpdateEquipped__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameObject sender, string slot, DiveReel __instance)
			{
				if (SN1MC.UsingSteamVR)
				{
					if (__instance.usingPlayer != null && __instance.nodes.Count > 0 && !__instance.cooldown && GameInput.GetButtonHeld(GameInput.Button.AltTool))
					{
						__instance.resetNodesSFX.Play();
						__instance.ResetNodes();
						Player.main.playerAnimator.SetTrigger("divereel_reset");
						__instance.animationController.SetTrigger("divereel_reset");
						__instance.SetDiveReelOutOfAmmoAnimator(false);
					}
				}
				else
				{
					if (GameInput.GetButtonHeld(GameInput.Button.MoveDown) && GameInput.GetButtonHeld(GameInput.Button.MoveDown))
					{
						if (__instance.usingPlayer != null && __instance.nodes.Count > 0 && !__instance.cooldown && GameInput.GetButtonHeld(GameInput.Button.AltTool))
						{
							__instance.resetNodesSFX.Play();
							__instance.ResetNodes();
							Player.main.playerAnimator.SetTrigger("divereel_reset");
							__instance.animationController.SetTrigger("divereel_reset");
							__instance.SetDiveReelOutOfAmmoAnimator(false);
						}
					}
				}
				return false;
			}
		}
	}
}
