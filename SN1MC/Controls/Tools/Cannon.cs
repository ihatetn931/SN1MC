using HarmonyLib;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
//Making the Cannon aim with vr controller
namespace SN1MC.Controls.Tools
{
	class Cannon
	{
		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.OnShoot))]
		public static class PropulsionCannon_OnShoot__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result, PropulsionCannon __instance)
			{
				if (__instance.grabbedObject != null)
				{
					float num;
					float num2;
					__instance.energyInterface.GetValues(out num, out num2);
					float d = Mathf.Min(1f, num / 4f);
					Rigidbody component = __instance.grabbedObject.GetComponent<Rigidbody>();
					float d2 = 1f + component.mass * __instance.massScalingFactor;
					Vector3 rightController = VRHandsController.rightController.transform.forward * __instance.shootForce * d / d2;

					Vector3 velocity = component.velocity;
					if (Vector3.Dot(velocity, rightController) < 0f)
					{
						component.velocity = rightController;
					}
					else
					{
						component.velocity = velocity * 0.3f + rightController;
					}

					__instance.grabbedObject.GetComponent<PropulseCannonAmmoHandler>().OnShot(false);
					__instance.launchedObjects.Add(__instance.grabbedObject);
					__instance.grabbedObject = null;
					__instance.energyInterface.ConsumeEnergy(4f);
					Utils.PlayFMODAsset(__instance.shootSound, __instance.transform, 20f);
					__instance.fxControl.Play(0);
				}
				else
				{
					GameObject rightController = CannonCustom.TraceForGrabTargetRight(__instance);
					if (rightController != null && GameInput.GetButtonHeld(GameInput.Button.RightHand))
					{
						__instance.GrabObject(rightController);
					}
					else
					{
						Utils.PlayFMODAsset(__instance.grabFailSound, __instance.transform, 20f);
					}
				}
				if (CannonCustom.grabbedObjectLeft != null && GameInput.GetButtonHeld(GameInput.Button.LeftHand))
				{
					float num;
					float num2;
					__instance.energyInterface.GetValues(out num, out num2);
					float d = Mathf.Min(1f, num / 4f);
					Rigidbody componentLeft = CannonCustom.grabbedObjectLeft.GetComponent<Rigidbody>();
					float d21 = 1f + componentLeft.mass * __instance.massScalingFactor;
					Vector3 leftController = VRHandsController.leftController.transform.forward * __instance.shootForce * d / d21;
					Vector3 velocity1 = componentLeft.velocity;
					if (Vector3.Dot(velocity1, leftController) < 0f)
					{
						componentLeft.velocity = leftController;
					}
					else
					{
						componentLeft.velocity = velocity1 * 0.3f + leftController;
					}
					CannonCustom.grabbedObjectLeft.GetComponent<PropulseCannonAmmoHandler>().OnShot(false);
					__instance.launchedObjects.Add(CannonCustom.grabbedObjectLeft);
					CannonCustom.grabbedObjectLeft = null;
					__instance.energyInterface.ConsumeEnergy(4f);
					Utils.PlayFMODAsset(__instance.shootSound, __instance.transform, 20f);
					__instance.fxControl.Play(0);
				}
				else
				{
					GameObject leftController = CannonCustom.TraceForGrabTargetLeft(__instance);
					if (leftController != null && GameInput.GetButtonHeld(GameInput.Button.LeftHand))
					{
						CannonCustom.GrabObjectLeft(leftController, __instance);
					}
					else
					{
						Utils.PlayFMODAsset(__instance.grabFailSound, __instance.transform, 20f);
					}
				}
				__result = true;
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.UpdateActive))]
		public static class PropulsionCannon_UpdateActive__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(PropulsionCannon __instance)
			{
				if (__instance.grabbedObject == null)
				{
					GameObject rightController = CannonCustom.TraceForGrabTargetRight(__instance);
					if (__instance.lastValidTarget != rightController && rightController != null && __instance.timeLastValidTargetSoundPlayed + 2f <= Time.time)
					{
						Utils.PlayFMODAsset(__instance.validTargetSound, __instance.transform, 20f);
						__instance.timeLastValidTargetSoundPlayed = Time.time;
					}
					__instance.lastValidTarget = rightController;
				}
				if (CannonCustom.grabbedObjectLeft == null)
				{
					GameObject leftController = CannonCustom.TraceForGrabTargetLeft(__instance);
					if (CannonCustom.lastValidTargetLeft != leftController && leftController != null && __instance.timeLastValidTargetSoundPlayed + 2f <= Time.time)
					{
						Utils.PlayFMODAsset(__instance.validTargetSound, __instance.transform, 20f);
						__instance.timeLastValidTargetSoundPlayed = Time.time;
					}
					CannonCustom.lastValidTargetLeft = leftController;
				}
				if (__instance.fpCannonModelRenderer != null)
				{
					if (__instance.grabbedObject != null || CannonCustom.grabbedObjectLeft != null)
					{
						__instance.cannonGlow = 1f;
					}
					else
					{
						__instance.cannonGlow -= Time.deltaTime;
					}
					__instance.fpCannonModelRenderer.material.SetFloat(__instance.shaderParamID, Mathf.Clamp01(__instance.cannonGlow));
				}
				__instance.animator.SetBool("use_tool", __instance.usingCannon);
				__instance.animator.SetBool("cangrab_propulsioncannon", __instance.canGrab || __instance.grabbedObject != null);
				HandReticle.main.SetIcon(HandReticle.IconType.Default, (__instance.canGrab && __instance.grabbedObject == null) ? 1.5f : 1f);

				__instance.animator.SetBool("cangrab_propulsioncannon", CannonCustom.canGrabLeft || CannonCustom.grabbedObjectLeft != null);
				HandReticle.main.SetIcon(HandReticle.IconType.Default, (CannonCustom.canGrabLeft && CannonCustom.grabbedObjectLeft == null) ? 1.5f : 1f);
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.ReleaseGrabbedObject))]
		public static class PropulsionCannon_ReleaseGrabbedObject__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(PropulsionCannon __instance)
			{
				if (__instance.grabbedObject != null)
				{
					PropulseCannonAmmoHandler component = __instance.grabbedObject.GetComponent<PropulseCannonAmmoHandler>();
					if (component != null && component.IsGrabbedBy(__instance))
					{
						component.UndoChanges();
						UnityEngine.Object.Destroy(component);
					}
					__instance.grabbedObject = null;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.UpdateTargetPosition))]
		public static class PropulsionCannon_UpdateTargetPosition__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(PropulsionCannon __instance)
			{
				__instance.targetPosition = __instance.GetObjectPosition(__instance.grabbedObject);
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.ValidateObject))]
		public static class PropulsionCannon_ValidateObject__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameObject go, ref bool __result, PropulsionCannon __instance)
			{

				if (!go.activeSelf || !go.activeInHierarchy)
				{
					Debug.Log("object is inactive");
					__result = false;
					return false;
				}
				Rigidbody component = go.GetComponent<Rigidbody>();

				if (component == null || component.mass > __instance.maxMass)
				{

					__result = false;
					return false;
				}
				Pickupable component2 = go.GetComponent<Pickupable>();
				bool flag = false;
				if (component2 != null)
				{
					flag = component2.attached;
				}
				__result = __instance.IsAllowedToGrab(go) && __instance.energyInterface.hasCharge && !flag;
				return false;
			}
		}

		public static Vector3 objectPos;

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.GetObjectPosition))]
		public static class PropulsionCannon_GetObjectPosition__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameObject go, ref Vector3 __result, PropulsionCannon __instance)
			{
				GameObject rightController = VRHandsController.rightController;
				Vector3 b = Vector3.zero;
				float num = 0f;
				if (go != null)
				{
					Bounds aabb = __instance.GetAABB(go);
					b = go.transform.position - aabb.center;
					Ray ray = new Ray(aabb.center, rightController.transform.forward);
					float f = 0;
					if (aabb.IntersectRay(ray, out f))
					{
						num = Mathf.Abs(f);
					}
					__instance.grabbedObjectCenter = aabb.center;
				}
				Vector3 position = Vector3.forward * (2.5f + num) + __instance.localObjectOffset;
				objectPos = rightController.transform.TransformPoint(position) + b;
				__result = objectPos;
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.IsGrabbingObject))]
		public static class PropulsionCannon_IsGrabbingObject__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result, PropulsionCannon __instance)
			{
				__result = __instance.grabbedObject != null;
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.Update))]
		public static class PropulsionCannon_Update__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(PropulsionCannon __instance)
			{
				if (__instance.grabbedObject != null)
				{
					if (__instance.grabbedObject.GetComponent<Rigidbody>() != null)
					{
						for (int i = 0; i < __instance.elecLines.Length; i++)
						{
							VFXElectricLine vfxelectricLine = __instance.elecLines[i];
							vfxelectricLine.origin = __instance.muzzle.position;
							vfxelectricLine.target = __instance.grabbedObjectCenter;
							vfxelectricLine.originVector = __instance.muzzle.forward;
						}
					}
					__instance.energyInterface.ConsumeEnergy(Time.deltaTime * 0.7f);
					__instance.UpdateTargetPosition();
					if (__instance.grabbedEffect)
					{
						__instance.grabbedEffect.transform.position = __instance.grabbedObjectCenter;
					}
				}
				if (CannonCustom.grabbedObjectLeft != null)
				{
					if (CannonCustom.grabbedObjectLeft.GetComponent<Rigidbody>() != null)
					{
						for (int d = 0; d < __instance.elecLines.Length; d++)
						{
							VFXElectricLine vfxelectricLine = __instance.elecLines[d];
							vfxelectricLine.origin = __instance.GetComponentInParent<Exosuit>().leftArm.GetGameObject().GetComponent<PropulsionCannon>().muzzle.transform.position;
							vfxelectricLine.target = CannonCustom.grabbedObjectCenterLeft;
							vfxelectricLine.originVector = __instance.GetComponentInParent<Exosuit>().leftArm.GetGameObject().GetComponent<PropulsionCannon>().muzzle.transform.forward;
						}
					}
					__instance.energyInterface.ConsumeEnergy(Time.deltaTime * 0.7f);
					CannonCustom.UpdateTargetPositionLeft(__instance);
					if (__instance.grabbedEffect)
					{
						__instance.grabbedEffect.transform.position = CannonCustom.grabbedObjectCenterLeft;
					}
				}
				if (__instance.firstUseGrabbedObject != null)
				{
					for (int j = 0; j < __instance.elecLines.Length; j++)
					{
						VFXElectricLine vfxelectricLine2 = __instance.elecLines[j];
						vfxelectricLine2.origin = __instance.muzzle.position;
						vfxelectricLine2.target = __instance.firstUseGrabbedObject.transform.position;
						vfxelectricLine2.originVector = __instance.muzzle.forward;
					}
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.GetText))]
		public static class PropulsionCannon_GetText__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(InventoryItem item, ref string __result, PropulsionCannon __instance)
			{
				string returned;
				if (item != null)
				{
					Pickupable item2 = item.item;
					returned = Language.main.Get(item2.GetTechName());
				}
				if (__instance.grabbedObject != null)
				{
					Pickupable component = __instance.grabbedObject.GetComponent<Pickupable>();
					if (component != null)
					{
						string arg = Language.main.Get(component.GetTechName());
						returned = Language.main.GetFormat<string>("PropulsionCannonUnload", arg);
					}
				}
				if (CannonCustom.grabbedObjectLeft != null)
				{
					Pickupable component = CannonCustom.grabbedObjectLeft.GetComponent<Pickupable>();
					if (component != null)
					{
						string arg = Language.main.Get(component.GetTechName());
						returned = Language.main.GetFormat<string>("PropulsionCannonUnload", arg);
					}
				}
				returned = Language.main.Get("PropulsionCannonUnloaded");
				__result = returned;
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.GetInteractableGrabbedObject))]
		public static class PropulsionCannon_GetInteractableGrabbedObject__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref GameObject __result, PropulsionCannon __instance)
			{
				if (!(__instance.grabbedObject != null) || (__instance.grabbedObject.transform.position - __instance.targetPosition).magnitude >= 1.4f)
				{
					__result = null;
					return false;
				}
				PickupableStorage componentInChildren = __instance.grabbedObject.GetComponentInChildren<PickupableStorage>();
                if (!(componentInChildren != null))
				{
					__result = __instance.grabbedObject;
					return false;
				}
				__result = componentInChildren.gameObject;
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.FixedUpdate))]
		public static class PropulsionCannon_FixedUpdate__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(PropulsionCannon __instance)
			{
				if (__instance.grabbedObject != null)
				{
					if (!__instance.ValidateObject(__instance.grabbedObject) || __instance.pickupDistance * 1.5f < (__instance.grabbedObject.transform.position - VRHandsController.rightController.transform.position).magnitude)
					{
						__instance.ReleaseGrabbedObject();
					}
					else
					{
						Rigidbody component = __instance.grabbedObject.GetComponent<Rigidbody>();
						Vector3 value = __instance.targetPosition - __instance.grabbedObject.transform.position;
						float magnitude = value.magnitude;
						float d = Mathf.Clamp(magnitude, 1f, 4f);
						Vector3 vector = component.velocity + Vector3.Normalize(value) * __instance.attractionForce * d * Time.deltaTime / (1f + component.mass * __instance.massScalingFactor);
						Vector3 amount = vector * (10f + Mathf.Pow(Mathf.Clamp01(1f - magnitude), 1.75f) * 40f) * Time.deltaTime;
						vector = UWE.Utils.SlerpVector(vector, Vector3.zero, amount);
						component.velocity = vector;
					}
				}
				if (CannonCustom.grabbedObjectLeft != null)
				{
					if (!__instance.ValidateObject(CannonCustom.grabbedObjectLeft) || __instance.pickupDistance * 1.5f < (CannonCustom.grabbedObjectLeft.transform.position - VRHandsController.leftController.transform.position).magnitude)
					{
						CannonCustom.ReleaseGrabbedObjectLeft(__instance);
					}
					else
					{
						Rigidbody component = CannonCustom.grabbedObjectLeft.GetComponent<Rigidbody>();
						Vector3 value = CannonCustom.targetPositionLeft - CannonCustom.grabbedObjectLeft.transform.position;
						float magnitude = value.magnitude;
						float d = Mathf.Clamp(magnitude, 1f, 4f);
						Vector3 vector = component.velocity + Vector3.Normalize(value) * __instance.attractionForce * d * Time.deltaTime / (1f + component.mass * __instance.massScalingFactor);
						Vector3 amount = vector * (10f + Mathf.Pow(Mathf.Clamp01(1f - magnitude), 1.75f) * 40f) * Time.deltaTime;
						vector = UWE.Utils.SlerpVector(vector, Vector3.zero, amount);
						component.velocity = vector;
					}
				}

				if (__instance.firstUseGrabbedObject != null)
				{
					__instance.grabbedEffect.transform.position = __instance.firstUseGrabbedObject.transform.position;
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.GrabObject))]
		public static class PropulsionCannon_GrabObject__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(GameObject target, PropulsionCannon __instance)
			{
				__instance.grabbedObject = target;
				UWE.Utils.SetIsKinematicAndUpdateInterpolation(__instance.grabbedObject.GetComponent<Rigidbody>(), false, false);
				PropulseCannonAmmoHandler propulseCannonAmmoHandler = target.GetComponent<PropulseCannonAmmoHandler>();
				if (propulseCannonAmmoHandler == null)
				{
					propulseCannonAmmoHandler = target.AddComponent<PropulseCannonAmmoHandler>();
					propulseCannonAmmoHandler.fxTrailPrefab = __instance.fxTrailPrefab;
				}
				propulseCannonAmmoHandler.ResetHandler(false, false);
				propulseCannonAmmoHandler.SetCannon(__instance);
				return false;
			}
		}


		[HarmonyPatch(typeof(PropulsionCannonWeapon), nameof(PropulsionCannonWeapon.OnAltDown))]
		public static class PropulsionCannonWeapon_OnAltDown__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result, PropulsionCannonWeapon __instance)
			{
				if (__instance.usingPlayer != null && __instance.usingPlayer.IsInside() && __instance.usingPlayer.IsInSub())
				{
					return false;
				}
				if (__instance.firstUseAnimationStarted)
				{
					__instance.OnFirstUseAnimationStop();
				}
				if (__instance.propulsionCannon.IsGrabbingObject())
				{
					__instance.propulsionCannon.ReleaseGrabbedObject();
				}
				if (CannonCustom.IsGrabbingObjectLeft())
				{
					CannonCustom.ReleaseGrabbedObjectLeft(__instance.propulsionCannon);
				}
				else if (__instance.propulsionCannon.HasChargeForShot() && !__instance.propulsionCannon.OnReload(new List<IItemsContainer>
				{
					Inventory.main.container
				}))
				{
					ErrorMessage.AddMessage(Language.main.Get("PropulsionCannonNoItems"));
				}
				__result = true;
				return false;
			}
		}


		[HarmonyPatch(typeof(PropulsionCannon), nameof(PropulsionCannon.Select))]
		public static class PropulsionCannon_Select__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(InventoryItem newItem, PropulsionCannon __instance)
			{
				bool flag = false;
				InventoryItem storedItem = __instance.storageSlot.storedItem;
				if (storedItem != null)
				{
					if (newItem != null)
					{
						IItemsContainer container = newItem.container;
						if (storedItem != newItem)
						{
							__instance.storageSlot.RemoveItem();
							if (__instance.storageSlot.AddItem(newItem))
							{
								if (__instance.AddToAnyContainer(__instance.srcContainers, storedItem))
								{
									flag = true;
								}
								else
								{
									__instance.storageSlot.RemoveItem();
									if (container != null)
									{
										container.AddItem(newItem);
									}
									__instance.storageSlot.AddItem(storedItem);
								}
							}
							else
							{
								if (container != null)
								{
									container.AddItem(newItem);
								}
								__instance.storageSlot.AddItem(storedItem);
							}
						}
					}
					else
					{
						__instance.storageSlot.RemoveItem();
						if (__instance.AddToAnyContainer(__instance.srcContainers, storedItem))
						{
							if(storedItem.item == __instance.grabbedObject)
								__instance.grabbedObject = null;
							if (storedItem.item == CannonCustom.grabbedObjectLeft)
								CannonCustom.grabbedObjectLeft = null;
						}
						else
						{
							__instance.storageSlot.AddItem(storedItem);
						}
					}
				}
				else if (newItem != null)
				{
					IItemsContainer container2 = newItem.container;
					if (__instance.storageSlot.AddItem(newItem))
					{
						flag = true;
					}
					else if (container2 != null)
					{
						container2.AddItem(newItem);
					}
				}
				if (flag)
				{
					Pickupable item = newItem.item;
					GameObject gameObject = item.gameObject;
					gameObject.SetActive(true);
					__instance.targetPosition = __instance.GetObjectPosition(gameObject);
					CannonCustom.targetPositionLeft = CannonCustom.GetObjectPositionLeft(gameObject, __instance);
					item.Drop(__instance.targetPosition, Vector3.zero, true);
					item.Drop(CannonCustom.targetPositionLeft, Vector3.zero, true);
					__instance.GrabObject(gameObject);
					CannonCustom.GrabObjectLeft(gameObject,__instance);
				}
				__instance.srcContainers = null;
				return false;
			}
		}

		/*[HarmonyPatch(typeof(PlayerTool), "OnToolUseAnim")]
		public static class PlayerToolReversePatchUpdate
		{
			[HarmonyReversePatch]
			[MethodImpl(MethodImplOptions.NoInlining)]
			public static void OnToolUseAnim(GUIHand instance)
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
				PlayerToolReversePatchUpdate.OnToolUseAnim(guiHand);
				if (__instance.energyMixin.charge > 0f && VRHandsController.rightController != null)
				{
					float d = Mathf.Clamp01(__instance.energyMixin.charge / 4f);
					Vector3 forward = VRHandsController.rightController.transform.forward;
					Vector3 position = VRHandsController.rightController.transform.position;
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
		}*/
	}
}
