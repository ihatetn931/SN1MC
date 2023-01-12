
using FMODUnity;
using HarmonyLib;
using System;
using UnityEngine;


namespace SN1MC.Controls.Tools
{
	extern alias SteamVRRef;
	class BuilderToolHandleInput
	{
		[HarmonyPatch(typeof(BuilderTool), nameof(BuilderTool.HandleInput))]
		public static class BuilderTool_HandleInput__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(BuilderTool __instance)
			{
				if (__instance.handleInputFrame == Time.frameCount)
				{
					return false;
				}
				__instance.handleInputFrame = Time.frameCount;
				if (!__instance.isDrawn || Builder.isPlacing || !AvatarInputHandler.main.IsEnabled())
				{
					return false;
				}
				if (__instance.TryDisplayNoPowerTooltip())
				{
					return false;
				}
				CustomTargeting.AddToIgnoreList(Player.main.gameObject);
				GameObject gameObject;
				float num;
				CustomTargeting.GetTarget(30f, out gameObject, out num);
				bool buttonHeld = false;
				bool buttonDown = false;
				bool buttonHeld2 = false;
				if (gameObject == null)
				{
					return false;
				}


				buttonHeld = GameInput.GetButtonHeld(GameInput.Button.LeftHand);
				buttonDown = GameInput.GetButtonDown(GameInput.Button.Deconstruct);
				buttonHeld2 = GameInput.GetButtonHeld(GameInput.Button.Deconstruct);
				float buttonHeldtime = GameInput.GetButtonHeldTime(GameInput.Button.Deconstruct);


				Constructable constructable = gameObject.GetComponentInParent<Constructable>();
				if (constructable != null && num > constructable.placeMaxDistance)
				{
					constructable = null;
				}
				if (constructable != null)
				{
					__instance.OnHover(constructable);
					if (buttonHeld)
					{
						__instance.Construct(constructable, true, false);
						return false;
					}
					string text;
					if (constructable.DeconstructionAllowed(out text))
					{
						if (buttonHeld2)
						{
							if (constructable.constructed)
							{
								Builder.ResetLast();
								constructable.SetState(false, false);
								return false;
							}
						}
						__instance.Construct(constructable, false, buttonDown);
						return false;

					}
					else if (buttonDown && !string.IsNullOrEmpty(text))
					{
						ErrorMessage.AddMessage(text);
						return false;
					}
				}
				else
				{
					BaseDeconstructable baseDeconstructable = gameObject.GetComponentInParent<BaseDeconstructable>();
					if (baseDeconstructable == null)
					{
						BaseExplicitFace componentInParent = gameObject.GetComponentInParent<BaseExplicitFace>();
						if (componentInParent != null)
						{
							baseDeconstructable = componentInParent.parent;
						}
					}
					if (baseDeconstructable != null && num <= 11f)
					{
						string text;
						if (baseDeconstructable.DeconstructionAllowed(out text))
						{
							__instance.OnHover(baseDeconstructable);
							if (buttonDown)
							{
								Builder.ResetLast();
								baseDeconstructable.Deconstruct();
								return false;
							}
						}
						else if (buttonDown && !string.IsNullOrEmpty(text))
						{
							ErrorMessage.AddMessage(text);
						}
					}
				}
				return false;
			}
		}
		//public static readonly GameInput.Button buttonRotateCW = GameInput.Button.CyclePrev;
		//public static readonly GameInput.Button buttonRotateCCW = GameInput.Button.CycleNext;
		/*		[HarmonyPatch(typeof(Builder), nameof(Builder.ShowRotationControlsHint))]
				public static class Builder_ShowRotationControlsHint__Patch
				{
					[HarmonyPrefix]
					static bool Prefix()
					{
						ErrorMessage.AddError(Language.main.GetFormat<string, string>("GhostRotateInputHint", uGUI.FormatButton(GameInput.Button.MoveDown,
						true, "InputSeparator", false), uGUI.FormatButton(GameInput.Button.MoveUp, true, "InputSeparator", false)));
						ErrorMessage.AddError(Language.main.GetFormat<string, string>("GhostRotateInputHint", uGUI.FormatButton(Builder.buttonRotateCW,
						true, "InputSeparator", false), uGUI.FormatButton(Builder.buttonRotateCCW, true, "InputSeparator", false)));
						return false;
					}
				}*/

		[HarmonyPatch(typeof(Builder), nameof(Builder.UpdateRotation))]
		public static class Builder_UpdateRotation__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(int max, ref bool __result)
			{
				if (GameInput.GetButtonDown(Builder.buttonRotateCW))
				{
					Builder.lastRotation = (Builder.lastRotation + max - 1) % max;
					__result = true;
				}
				if (GameInput.GetButtonDown(Builder.buttonRotateCCW))
				{
					Builder.lastRotation = (Builder.lastRotation + 1) % max;
					__result = true;
				}

				__result = false;
				return false;
			}
		}

		[HarmonyPatch(typeof(Builder), nameof(Builder.CalculateAdditiveRotationFromInput))]
		public static class Builder_CalculateAdditiveRotationFromInput__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(float additiveRotation, ref float __result)
			{
				if (GameInput.GetButtonHeld(Builder.buttonRotateCW))
				{
					additiveRotation = MathExtensions.RepeatAngle(additiveRotation - Builder.GetDeltaTimeForAdditiveRotation() * Builder.additiveRotationSpeed);
				}
				else if (GameInput.GetButtonHeld(Builder.buttonRotateCCW))
				{
					additiveRotation = MathExtensions.RepeatAngle(additiveRotation + Builder.GetDeltaTimeForAdditiveRotation() * Builder.additiveRotationSpeed);
				}

				//return additiveRotation;
				__result = additiveRotation;
				return false;
			}
		}
	}
}
