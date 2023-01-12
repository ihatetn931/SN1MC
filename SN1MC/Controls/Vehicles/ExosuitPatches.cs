
using FMOD.Studio;
using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;
//All the magic to control the exo suit arms by the vr controllers
namespace SN1MC.Controls.Vehicles
{
	public static class ExosuitPatches
	{
		public static GameObject activeTarget1;
		[HarmonyPatch(typeof(Exosuit), nameof(Exosuit.UpdateActiveTarget))]
		class CameraDrone_OnEnable_Patch
		{
			static bool Prefix(Exosuit __instance)
			{
				GameObject target = null;
				GameObject target1 = null;
				float num;
				float num1;
				CustomTargeting.GetTarget(__instance.gameObject, 6f, out target, out num);
				CustomTargeting.GetTargetLeft(__instance.gameObject, 6f, out target1, out num1);
				__instance.activeTarget = __instance.GetInteractableRoot(target);
				activeTarget1 = __instance.GetInteractableRoot(target1);
				if (__instance.activeTarget != null)
				{
					GUIHand component = Player.main.GetComponent<GUIHand>();
					GUIHand.Send(__instance.activeTarget, HandTargetEventType.Hover, component);
				}
				if (activeTarget1 != null)
				{
					GUIHand component = Player.main.GetComponent<GUIHand>();
					GUIHand.Send(activeTarget1, HandTargetEventType.Hover, component);
				}
				return false;
			}

			public static Pickupable component;
			public static PickPrefab component2;
			[HarmonyPatch(typeof(ExosuitClawArm), nameof(ExosuitClawArm.OnPickup))]
			class ExosuitClawArm_OnPickup_Patch
			{
				static bool Prefix(ExosuitClawArm __instance)
				{
					//Exosuit componentInParent = __instance.exosuit;//__instance.GetComponentInParent<Exosuit>();
					Exosuit componentInParent = __instance.GetComponentInParent<Exosuit>();
					if (componentInParent.GetActiveTarget())
					{
						component = componentInParent.GetActiveTarget().GetComponent<Pickupable>();
						component2 = componentInParent.GetActiveTarget().GetComponent<PickPrefab>();
						__instance.StartCoroutine(__instance.OnPickupAsync(component, component2, componentInParent));
					}
					if (GetActiveTarget())
					{
						component = GetActiveTarget().GetComponent<Pickupable>();
						component2 = GetActiveTarget().GetComponent<PickPrefab>();
						__instance.StartCoroutine(__instance.OnPickupAsync(component, component2, componentInParent));
					}
					//__instance.StartCoroutine(__instance.OnPickupAsync(component, component2, componentInParent));
					return false;
				}
			}
			public static Transform GetAimingTransform()
			{
				return VRHandsController.leftController.transform;
			}

			public static Transform aimingTransform;
			[HarmonyPatch(typeof(Vehicle), nameof(Vehicle.TorpedoShot))]
			class Vehicle_TorpedoShot_Patch
			{
				static bool Prefix(ItemsContainer container, TorpedoType torpedoType, Transform muzzle, ref bool __result,Vehicle __instance)
				{
					if (torpedoType != null && container.DestroyItem(torpedoType.techType))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(torpedoType.prefab);
						gameObject.GetComponent<Transform>();
						Bullet component = gameObject.GetComponent<SeamothTorpedo>();
						Vector3 zero = Vector3.zero;
						Exosuit exoSuit = GameObject.FindObjectOfType<Exosuit>();
						if (VRHandsController.leftController != null || VRHandsController.rightController != null)
						{
							if (exoSuit != null)
							{
								if (exoSuit.currentLeftArmType == TechType.ExosuitTorpedoArmModule && GameInput.GetButtonHeld(GameInput.Button.LeftHand))
								{
									aimingTransform = GetAimingTransform();
								}
								if (exoSuit.currentRightArmType == TechType.ExosuitTorpedoArmModule && GameInput.GetButtonHeld(GameInput.Button.RightHand))
								{
									aimingTransform = Player.main.camRoot.GetAimingTransform();
								}
							}
						}
						else
                        {
							aimingTransform = MainCamera.camera.transform;
                        }
						Rigidbody componentInParent = muzzle.GetComponentInParent<Rigidbody>();
						Vector3 rhs = (componentInParent != null) ? componentInParent.velocity : Vector3.zero;
						float speed = Vector3.Dot(aimingTransform.forward, rhs);
						component.Shoot(muzzle.position, aimingTransform.rotation, speed, -1f);
						__result = true;
						return false;
					}
					__result = false;
					return false;
				}
			}

			[HarmonyPatch(typeof(ExosuitClawArm), nameof(ExosuitClawArm.TryUse))]
			class ExosuitClawArm_TryUse_Patch
			{
				static bool Prefix(out float cooldownDuration, ref bool __result, ExosuitClawArm __instance)
				{
					if (Time.time - __instance.timeUsed >= __instance.cooldownTime)
					{
						Pickupable pickupable = null;
						PickPrefab x = null;
						if (__instance.exosuit.GetActiveTarget() && !GetActiveTarget())
						{
							pickupable = __instance.exosuit.GetActiveTarget().GetComponent<Pickupable>();
							x = __instance.exosuit.GetActiveTarget().GetComponent<PickPrefab>();
						}
						if (pickupable != null && pickupable.isPickupable)
						{
							if (__instance.exosuit.storageContainer.container.HasRoomFor(pickupable))
							{
								__instance.animator.SetTrigger("use_tool");
								__instance.cooldownTime = (cooldownDuration = __instance.cooldownPickup);
								__instance.shownNoRoomNotification = false;
								__result = true;
								return false;
							}
							if (!__instance.shownNoRoomNotification)
							{
								ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
								__instance.shownNoRoomNotification = true;
							}
						}

						Pickupable pickupable1 = null;
						PickPrefab x1 = null;
						if (GetActiveTarget() && !__instance.exosuit.GetActiveTarget())
						{
							pickupable1 = GetActiveTarget().GetComponent<Pickupable>();
							x1 = GetActiveTarget().GetComponent<PickPrefab>();
						}
						if (pickupable1 != null && pickupable1.isPickupable)
						{
							if (__instance.exosuit.storageContainer.container.HasRoomFor(pickupable1))
							{
								__instance.animator.SetTrigger("use_tool");
								__instance.cooldownTime = (cooldownDuration = __instance.cooldownPickup);
								__instance.shownNoRoomNotification = false;
								__result = true;
								return false;
							}
							if (!__instance.shownNoRoomNotification)
							{
								ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
								__instance.shownNoRoomNotification = true;
							}
						}
						else
						{
							if (x != null)
							{
								__instance.animator.SetTrigger("use_tool");
								__instance.cooldownTime = (cooldownDuration = __instance.cooldownPickup);
								__result = true;
								return false;
							}
							if (x1 != null)
							{
								__instance.animator.SetTrigger("use_tool");
								__instance.cooldownTime = (cooldownDuration = __instance.cooldownPickup);
								__result = true;
								return false;
							}
							__instance.animator.SetTrigger("bash");
							__instance.cooldownTime = (cooldownDuration = __instance.cooldownPunch);
							__instance.fxControl.Play(0);
							__result = true;
							return false;
						}
					}
					cooldownDuration = 0f;
					//result = false;
					__result = false;
					return false;
				}
			}
			public static Quaternion quaternion;

			public static GameObject GetActiveTarget()
			{
				GameObject Go = null;
				if (activeTarget1 != null)
				{
					Go = activeTarget1;
				}
				return Go;
			}

			[HarmonyPatch(typeof(ExosuitDrillArm), "IExosuitArm.Update")]
			class ExosuitDrillArm_IExosuitArm_Update_Patch
			{
				public static bool Prefix(ref Quaternion aimDirection, ExosuitDrillArm __instance)
				{

					if (__instance.drillTarget != null)
					{
						if (__instance.exosuit.currentLeftArmType == TechType.ExosuitDrillArmModule)
						{
							quaternion = Quaternion.LookRotation(Vector3.Normalize(UWE.Utils.GetEncapsulatedAABB(__instance.drillTarget, -1).center - VRHandsController.leftController.transform.position), Vector3.up);
						}
						if (__instance.exosuit.currentRightArmType == TechType.ExosuitDrillArmModule)
						{
							quaternion = Quaternion.LookRotation(Vector3.Normalize(UWE.Utils.GetEncapsulatedAABB(__instance.drillTarget, -1).center - VRHandsController.rightController.transform.position), Vector3.up);
						}
						aimDirection = quaternion;
					}
					return false;
				}

				[HarmonyPatch(typeof(ExosuitClawArm), nameof(ExosuitClawArm.OnHit))]
				class ExosuitClawArm_OnHit_Patch
				{
					public static bool Prefix(ExosuitClawArm __instance)
					{
						Exosuit componentInParent = __instance.GetComponentInParent<Exosuit>();
						if (componentInParent.CanPilot() && componentInParent.GetPilotingMode())
						{
							Vector3 position = default(Vector3);
							GameObject gameObject = null;
							Vector3 vector;

							Vector3 position1 = default(Vector3);
							GameObject gameObject1 = null;
							Vector3 vector1;

							CustomTargeting.TraceFPSTargetPosition(componentInParent.gameObject, 6.5f, ref gameObject, ref position, out vector, true);
							if (gameObject == null)
							{
								InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
								if (component != null && component.GetMostRecent() != null)
								{
									gameObject = component.GetMostRecent().gameObject;
								}
							}
							if (gameObject && __instance.exosuit.currentRightArmType == TechType.ExosuitClawArmModule && GameInput.GetButtonHeld(GameInput.Button.RightHand))
							{
								LiveMixin liveMixin = gameObject.FindAncestor<LiveMixin>();
								if (liveMixin)
								{
									liveMixin.TakeDamage(50f, position, DamageType.Normal, null);
								}
								if (gameObject.FindAncestor<Creature>())
								{
									global::Utils.PlayFMODAsset(__instance.hitFishSound, __instance.front, 50f);
								}
								else
								{
									global::Utils.PlayFMODAsset(__instance.hitTerrainSound, __instance.front, 50f);
								}
								VFXSurface component2 = gameObject.GetComponent<VFXSurface>();
								Vector3 euler = VRHandsController.rightController.transform.eulerAngles + new Vector3(300f, 90f, 0f);
								VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, position, Quaternion.Euler(euler), componentInParent.gameObject.transform);

								gameObject.SendMessage("BashHit", __instance, SendMessageOptions.DontRequireReceiver);
							}

							CustomTargeting.TraceFPSTargetPositionLeft(componentInParent.gameObject, 6.5f, ref gameObject1, ref position1, out vector1, true);
							if (gameObject1 == null)
							{
								InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
								if (component != null && component.GetMostRecent() != null)
								{
									gameObject1 = component.GetMostRecent().gameObject;
								}
							}
							if (gameObject1 && __instance.exosuit.currentLeftArmType == TechType.ExosuitClawArmModule && GameInput.GetButtonHeld(GameInput.Button.LeftHand))
							{
								LiveMixin liveMixin = gameObject1.FindAncestor<LiveMixin>();
								if (liveMixin)
								{
									liveMixin.TakeDamage(50f, position1, DamageType.Normal, null);
								}
								if (gameObject1.FindAncestor<Creature>())
								{
									global::Utils.PlayFMODAsset(__instance.hitFishSound, __instance.front, 50f);
								}
								else
								{
									global::Utils.PlayFMODAsset(__instance.hitTerrainSound, __instance.front, 50f);
								}
								VFXSurface component2 = gameObject1.GetComponent<VFXSurface>();
								Vector3 euler = VRHandsController.leftController.transform.eulerAngles + new Vector3(300f, 90f, 0f);
								VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, position1, Quaternion.Euler(euler), componentInParent.gameObject.transform);
								gameObject1.SendMessage("BashHit", __instance, SendMessageOptions.DontRequireReceiver);
							}
						}
						return false;
					}
				}

				[HarmonyPatch(typeof(ExosuitDrillArm), nameof(ExosuitDrillArm.OnHit))]
				class ExosuitDrillArm_OnHit_Patch
				{
					static bool Prefix(ExosuitDrillArm __instance)
					{
						if (__instance.exosuit.CanPilot() && __instance.exosuit.GetPilotingMode())
						{
							Vector3 zero = Vector3.zero;
							GameObject gameObject = null;
							Vector3 zero1 = Vector3.zero;
							GameObject gameObject1 = null;
							__instance.drillTarget = null;
							Vector3 vector;
							Vector3 vector1;
							CustomTargeting.TraceFPSTargetPosition(__instance.exosuit.gameObject, 5f, ref gameObject, ref zero, out vector, true);
							if (gameObject == null)
							{
								InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
								if (component != null && component.GetMostRecent() != null)
								{
									gameObject = component.GetMostRecent().gameObject;
								}
							}
							if (gameObject && __instance.drilling)
							{
								Drillable drillable = gameObject.FindAncestor<Drillable>();
								__instance.loopHit.Play();
								if (!drillable)
								{
									LiveMixin liveMixin = gameObject.FindAncestor<LiveMixin>();
									if (liveMixin)
									{
										liveMixin.IsAlive();
										liveMixin.TakeDamage(4f, zero, DamageType.Drill, null);
										__instance.drillTarget = gameObject;
									}
									VFXSurface component2 = gameObject.GetComponent<VFXSurface>();
									if (__instance.drillFXinstance == null)
									{
										__instance.drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, __instance.fxSpawnPoint.position, __instance.fxSpawnPoint.rotation, __instance.fxSpawnPoint);
									}
									else if (component2 != null && __instance.prevSurfaceType != component2.surfaceType)
									{
										__instance.drillFXinstance.GetComponent<VFXLateTimeParticles>().Stop();
										UnityEngine.Object.Destroy(__instance.drillFXinstance.gameObject, 1.6f);
										__instance.drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, __instance.fxSpawnPoint.position, __instance.fxSpawnPoint.rotation, __instance.fxSpawnPoint);
										__instance.prevSurfaceType = component2.surfaceType;
									}
									gameObject.SendMessage("BashHit", __instance, SendMessageOptions.DontRequireReceiver);
									return false;
								}
								GameObject gameObject2;
								drillable.OnDrill(__instance.fxSpawnPoint.position, __instance.exosuit, out gameObject2);
								__instance.drillTarget = gameObject2;
								if (__instance.fxControl.emitters[0].fxPS != null && !__instance.fxControl.emitters[0].fxPS.emission.enabled)
								{
									__instance.fxControl.Play(0);
									return false;
								}
							}
							CustomTargeting.TraceFPSTargetPositionLeft(__instance.exosuit.gameObject, 5f, ref gameObject1, ref zero1, out vector1, true);
							if (gameObject1 == null)
							{
								InteractionVolumeUser component = Player.main.gameObject.GetComponent<InteractionVolumeUser>();
								if (component != null && component.GetMostRecent() != null)
								{
									gameObject1 = component.GetMostRecent().gameObject;
								}
							}
							if (gameObject1 && __instance.drilling)
							{
								Drillable drillable = gameObject1.FindAncestor<Drillable>();
								__instance.loopHit.Play();
								if (!drillable)
								{
									LiveMixin liveMixin = gameObject1.FindAncestor<LiveMixin>();
									if (liveMixin)
									{
										liveMixin.IsAlive();
										liveMixin.TakeDamage(4f, zero1, DamageType.Drill, null);
										__instance.drillTarget = gameObject1;
									}
									VFXSurface component2 = gameObject1.GetComponent<VFXSurface>();
									if (__instance.drillFXinstance == null)
									{
										__instance.drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, __instance.fxSpawnPoint.position, __instance.fxSpawnPoint.rotation, __instance.fxSpawnPoint);
									}
									else if (component2 != null && __instance.prevSurfaceType != component2.surfaceType)
									{
										__instance.drillFXinstance.GetComponent<VFXLateTimeParticles>().Stop();
										UnityEngine.Object.Destroy(__instance.drillFXinstance.gameObject, 1.6f);
										__instance.drillFXinstance = VFXSurfaceTypeManager.main.Play(component2, __instance.vfxEventType, __instance.fxSpawnPoint.position, __instance.fxSpawnPoint.rotation, __instance.fxSpawnPoint);
										__instance.prevSurfaceType = component2.surfaceType;
									}
									gameObject1.SendMessage("BashHit", __instance, SendMessageOptions.DontRequireReceiver);
									return false;
								}
								GameObject gameObject22;
								drillable.OnDrill(__instance.fxSpawnPoint.position, __instance.exosuit, out gameObject22);
								__instance.drillTarget = gameObject22;
								if (__instance.fxControl.emitters[0].fxPS != null && !__instance.fxControl.emitters[0].fxPS.emission.enabled)
								{
									__instance.fxControl.Play(0);
									return false;
								}
							}
							else
							{
								__instance.StopEffects();
							}
						}
						return false;
					}
				}

				[HarmonyPatch(typeof(ExosuitGrapplingArm), nameof(ExosuitGrapplingArm.OnHit))]
				class ExosuitGrapplingArm_OnHit_Patch
				{
					static bool Prefix(ExosuitGrapplingArm __instance)
					{
						var defaultFrontpos = __instance.front.transform.position;
						__instance.hook.transform.parent = null;
						__instance.hook.transform.position = __instance.front.transform.position;
						__instance.hook.SetFlying(true);
						GameObject x = null;
						Vector3 a = default(Vector3);
						Vector3 vector;
						GameObject x1 = null;
						Vector3 a1 = default(Vector3);
						Vector3 vector1;
						CustomTargeting.TraceFPSTargetPositionLeft(__instance.exosuit.gameObject, 100f, ref x, ref a, out vector, false);
						if (__instance.exosuit.currentLeftArmType == TechType.ExosuitGrapplingArmModule && GameInput.GetButtonHeld(GameInput.Button.LeftHand))
						{
							if (x == null || x == __instance.hook.gameObject)
							{
								a = VRHandsController.leftController.transform.position + VRHandsController.leftController.transform.forward * 25f;
							}
							Vector3 a2 = Vector3.Normalize(a - __instance.hook.transform.position);
							__instance.front.transform.position = a;
							__instance.hook.rb.velocity = a2 * 25f;
						}
						CustomTargeting.TraceFPSTargetPosition(__instance.exosuit.gameObject, 100f, ref x1, ref a1, out vector1, false);
						if (__instance.exosuit.currentRightArmType == TechType.ExosuitGrapplingArmModule && GameInput.GetButtonHeld(GameInput.Button.RightHand))
						{
							if (x1 == null || x1 == __instance.hook.gameObject)
							{
								a1 = VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * 25f;
							}
							Vector3 a21 = Vector3.Normalize(a1 - __instance.hook.transform.position);
							__instance.front.transform.position = a1;
							__instance.hook.rb.velocity = a21 * 25f;
						}
						__instance.front.transform.position = defaultFrontpos;
						global::Utils.PlayFMODAsset(__instance.shootSound, __instance.front, 15f);
						__instance.grapplingStartPos = __instance.exosuit.transform.position;
						return false;
					}
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

				[HarmonyPatch(typeof(ExosuitClawArm), "IExosuitArm.OnUseDown")]
				public static class IExosuitArm_OnUseDown_Patch
				{
					[HarmonyPrefix]
					public static bool Prefix(out float cooldownDuration, ref bool __result, ExosuitClawArm __instance)
					{
						__result = __instance.TryUse(out cooldownDuration);
						return false;
					}
				}

				[HarmonyPatch(typeof(ExosuitClawArm), "IExosuitArm.Update")]
				public static class IExosuitArm_Update_Patch
				{
					[HarmonyPrefix]
					public static bool Prefix(ref Quaternion aimDirection, ExosuitClawArm __instance)
					{
						XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 rightControllerAngularVelocity);
						XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightControllerVelocity);
						XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.deviceAcceleration, out Vector3 rightControllerAccel);
						XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out Vector3 rightControllerAngularAccel);
						//ErrorMessage.AddDebug("AngularVelocity: " + rightControllerAngularVelocity.magnitude);
						//ErrorMessage.AddDebug("Velocity: " + rightControllerVelocity.magnitude);
						//ErrorMessage.AddDebug("Accel: " + rightControllerAccel.magnitude);
						//ErrorMessage.AddDebug("AngularAccel: " + rightControllerAngularAccel.magnitude);
						return false;
					}
				}

				[HarmonyPatch(typeof(ExosuitClawArm), "IExosuitArm.OnUseHeld")]
				public static class IExosuitArm_OnUseHeld_Patch
				{
					[HarmonyPrefix]
					public static bool Prefix(out float cooldownDuration, ref bool __result, ExosuitClawArm __instance)
					{
						//ErrorMessage.AddDebug("OnUseHeld");
						__result = __instance.TryUse(out cooldownDuration);
						return false;
					}
				}

				[HarmonyPatch(typeof(Exosuit), nameof(Exosuit.Update))]
				public static class Exosuit_Update_Patch
				{
					[HarmonyPrefix]
					public static bool Prefix(Exosuit __instance)
					{
						VehicleReversePatchUpdate.Update(__instance);
						__instance.UpdateThermalReactorCharge();
						if (__instance.storageContainer.GetOpen())
						{
							__instance.openedFraction = Mathf.Clamp01(__instance.openedFraction + Time.deltaTime * 2f);
						}
						else
						{
							__instance.openedFraction = Mathf.Clamp01(__instance.openedFraction - Time.deltaTime * 2f);
						}
						__instance.storageFlap.localEulerAngles = new Vector3(__instance.startFlapPitch + __instance.openedFraction * 80f, 0f, 0f);
						bool pilotingMode = __instance.GetPilotingMode();
						bool flag = __instance.onGround || Time.time - __instance.timeOnGround <= 0.5f;
						__instance.mainAnimator.SetBool("sit", !pilotingMode && flag && !__instance.IsUnderwater());
						bool flag2 = pilotingMode && !__instance.docked;
						if (pilotingMode)
						{
							Player.main.transform.localPosition = Vector3.zero;
							Player.main.transform.localRotation = Quaternion.identity;
							Vector3 vector;// = AvatarInputHandler.main.IsEnabled() ? GameInput.GetMoveDirection() : Vector3.zero;
							if (__instance.IsAutopilotEnabled)
							{
								vector = __instance.CalculateAutopilotLocalWishDir();
							}
							else
							{
								vector = (AvatarInputHandler.main.IsEnabled() ? GameInput.GetMoveDirection() : Vector3.zero);
							}
							bool flag3 = vector.y > 0f;
							bool flag4 = __instance.IsPowered() && __instance.liveMixin.IsAlive();
							if (flag3 && flag4)
							{
								__instance.thrustPower = Mathf.Clamp01(__instance.thrustPower - Time.deltaTime * __instance.thrustConsumption);
								if ((__instance.onGround || Time.time - __instance.timeOnGround <= 1f) && !__instance.jetDownLastFrame)
								{
									__instance.ApplyJumpForce();
								}
								__instance.jetsActive = true;
							}
							else
							{
								__instance.jetsActive = false;
								float num = Time.deltaTime * __instance.thrustConsumption * 0.7f;
								if (__instance.onGround)
								{
									num = Time.deltaTime * __instance.thrustConsumption * 4f;
								}
								__instance.thrustPower = Mathf.Clamp01(__instance.thrustPower + num);
							}
							__instance.jetDownLastFrame = flag3;
							if (__instance.timeJetsActiveChanged + 0.3f <= Time.time)
							{
								if (__instance.jetsActive && __instance.thrustPower > 0f)
								{
									__instance.loopingJetSound.Play();
									__instance.fxcontrol.Play(0);
									__instance.areFXPlaying = true;
								}
								else if (__instance.areFXPlaying)
								{
									__instance.loopingJetSound.Stop(STOP_MODE.ALLOWFADEOUT);
									__instance.fxcontrol.Stop(0);
									__instance.areFXPlaying = false;
								}
							}
							if (flag2 || vector.x != 0f || vector.z != 0f)
							{
								__instance.ConsumeEngineEnergy(0.083333336f * Time.deltaTime);
							}
							if (__instance.jetsActive)
							{
								__instance.thrustIntensity += Time.deltaTime / __instance.timeForFullVirbation;
							}
							else
							{
								__instance.thrustIntensity -= Time.deltaTime * 10f;
							}
							__instance.thrustIntensity = Mathf.Clamp01(__instance.thrustIntensity);
							if (AvatarInputHandler.main.IsEnabled() && !__instance.ignoreInput)
							{
								Vector3 eulerAngles = __instance.transform.eulerAngles;
								eulerAngles.x = VRHandsController.rightController.transform.eulerAngles.x;
								eulerAngles.y = VRHandsController.rightController.transform.eulerAngles.y;

								Vector3 eulerAngles1 = __instance.transform.eulerAngles;
								eulerAngles1.x = VRHandsController.leftController.transform.eulerAngles.x;
								eulerAngles1.y = VRHandsController.leftController.transform.eulerAngles.y;
								//var testRight = VRHandsController.rightController.transform.eulerAngles;
								//var testLeft = VRHandsController.leftController.transform.eulerAngles;
								Quaternion quaternion = Quaternion.Euler(eulerAngles1);
								Quaternion rotation = Quaternion.Euler(eulerAngles);
								__instance.leftArm.Update(ref quaternion);
								__instance.rightArm.Update(ref rotation);
								if (flag)
								{
									Vector3 b = VRHandsController.leftController.transform.position + quaternion * Vector3.forward * 100f;
									Vector3 b2 = VRHandsController.rightController.transform.position + rotation * Vector3.forward * 100f;
									__instance.aimTargetLeft.transform.position = Vector3.Lerp(__instance.aimTargetLeft.transform.position, b, Time.deltaTime * 15f);
									__instance.aimTargetRight.transform.position = Vector3.Lerp(__instance.aimTargetRight.transform.position, b2, Time.deltaTime * 15f);
								}
								bool hasPropCannon = __instance.rightArm is ExosuitPropulsionArm || __instance.leftArm is ExosuitPropulsionArm;
								__instance.UpdateUIText(hasPropCannon);
								if (GameInput.GetButtonDown(GameInput.Button.AltTool) && !__instance.rightArm.OnAltDown())
								{
									__instance.leftArm.OnAltDown();
								}
								if (GameInput.GetButtonDown(GameInput.Button.AltTool) && !__instance.leftArm.OnAltDown())
								{
									__instance.rightArm.OnAltDown();
								}
							}
							__instance.UpdateActiveTarget();
							__instance.UpdateSounds();
						}
						if (!flag2)
						{
							bool flag5 = false;
							bool flag6 = false;
							if (!Mathf.Approximately(__instance.aimTargetLeft.transform.localPosition.y, 0f))
							{
								float y = Mathf.MoveTowards(__instance.aimTargetLeft.transform.localPosition.y, 0f, Time.deltaTime * 50f);
								__instance.aimTargetLeft.transform.localPosition = new Vector3(__instance.aimTargetLeft.transform.localPosition.x, y, __instance.aimTargetLeft.transform.localPosition.z);
							}
							else
							{
								flag5 = true;
							}
							if (!Mathf.Approximately(__instance.aimTargetRight.transform.localPosition.y, 0f))
							{
								float y2 = Mathf.MoveTowards(__instance.aimTargetRight.transform.localPosition.y, 0f, Time.deltaTime * 50f);
								__instance.aimTargetRight.transform.localPosition = new Vector3(__instance.aimTargetRight.transform.localPosition.x, y2, __instance.aimTargetRight.transform.localPosition.z);
							}
							else
							{
								flag6 = true;
							}
							if (flag5 && flag6)
							{
								__instance.SetIKEnabled(false);
							}
						}
						__instance.UpdateAnimations();
						if (__instance.armsDirty)
						{
							__instance.UpdateExosuitArms();
						}
						return false;
					}
				}
			}
		}
	}
}
