

using HarmonyLib;
using SN1MC.Controls;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;
//using VREnhancementsBZ;

namespace SN1MC
{
	//VR Options Menu for all the mods settings so It does not require SML Helper and make it so i can do it my own way.
	public class VRCustomOptionsMenu
	{
		public static uGUI_OptionsPanel optionsPanel;
		public static Toggle pilotShowHideVehicles;
		//public static Toggle controlPanel;
		public static Color headerColor = new Color(255, 140, 0);
		public static int tabIndex = -1;
		public static bool EnableMotionControls = true;
		public static float ikSmoothing = 0.4f;
		public static Color laserPointerColor = Color.cyan;
		public static bool EnableVRPiloting = true;
		public static bool CyclopsPilot = true;
		public static bool enableHandsWithoutTools = false;

		[HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTabs))]
		class SubtitlesPosition_Patch
		{
			static void Postfix(uGUI_OptionsPanel __instance)
			{
				optionsPanel = __instance;
				if (XRSettings.enabled)
				{
					AddVrOptionsTab();
				}
			}
		}

		[HarmonyPatch(typeof(GameSettings), nameof(GameSettings.SerializeVRSettings))]
		class SerializeVRSettings_Patch
		{
			static bool PreFix(GameSettings.ISerializer serializer)
			{
				XRSettings.eyeTextureResolutionScale = serializer.Serialize("VR/RenderScale", XRSettings.eyeTextureResolutionScale);
				VROptions.gazeBasedCursor = serializer.Serialize("VR/GazeBasedCursor", VROptions.gazeBasedCursor);
				EnableMotionControls = serializer.Serialize("VR/EnableMotionControls", EnableMotionControls);
				ikSmoothing = serializer.Serialize("VR/Smoothing", ikSmoothing);
				laserPointerColor = serializer.Serialize("VR/LaserPointerColor", laserPointerColor);
				EnableVRPiloting = serializer.Serialize("VR/EnableVRPiloting", EnableVRPiloting);
				CyclopsPilot = serializer.Serialize("VR/CyclopsPilot", CyclopsPilot);
				enableHandsWithoutTools = serializer.Serialize("VR/EnableHandsWithoutTools", enableHandsWithoutTools);
				return false;
			}
		}


		public static void AddVrOptionsTab()
		{
			//ShowHideMenus();
			tabIndex = optionsPanel.AddTab("VR Options");

			optionsPanel.AddHeading(tabIndex, ColorString("Motion Control Options", headerColor));
			optionsPanel.AddToggleOption(tabIndex, "Motion Controls Toggle", EnableMotionControls, delegate (bool v)
			{
				EnableMotionControls = v;
			}, "Toggles Motion Controls");
			optionsPanel.AddToggleOption(tabIndex, "Enable Hands Without Tools", enableHandsWithoutTools, delegate (bool v)
			{
				enableHandsWithoutTools = v;
			}, "Enables The Player Hands All the time, not just with tools (Warning May Look Funny)");

			optionsPanel.AddToggleOption(tabIndex, "VR Controled Piloting", EnableVRPiloting, delegate (bool v)
			{
				EnableVRPiloting = v;
				if (v)
					pilotShowHideVehicles.gameObject.SetActive(true);
				else
					pilotShowHideVehicles.gameObject.SetActive(false);
			}, "Toggles Driving With VR Controls");

			pilotShowHideVehicles = optionsPanel.AddToggleOption(tabIndex, "Cyclops VR Control", CyclopsPilot, delegate (bool v)
			{
				CyclopsPilot = v;
			}, "Enable Driving With Cyclops with motion controls");

			optionsPanel.AddToggleOption(tabIndex, "Disable Y-Axis Input", VROptions.disableInputPitch, (bool v) => VROptions.disableInputPitch = v, "Disable Up And Down With Mouse/Controller");

			pilotShowHideVehicles.gameObject.SetActive(EnableVRPiloting);
			optionsPanel.AddSliderOption(tabIndex, "Vr Controller Smoothing", ikSmoothing, 0.1f, 1.0f, ikSmoothing, 0.1f, (float v) =>
			{
				ikSmoothing = v;

			}, SliderLabelMode.Float, "0.0", "VR Controller smoothing, Higher Values Are Less Smooth.");
			optionsPanel.AddToggleOption(tabIndex, "Aim Right Arm With Head", VROptions.aimRightArmWithHead, (bool v) => VROptions.aimRightArmWithHead = v, "Aim tools with Head");
			UnityAction callback = delegate ()
			{
				Binding.SetControlsforOculus();
			};
			optionsPanel.AddButton(tabIndex, "Setup DPad On Quest",callback);

			optionsPanel.AddHeading(tabIndex, ColorString("Misc.", headerColor));
			optionsPanel.AddColorOption(tabIndex, "Header Color (Changes On Options Reopen)", headerColor, delegate (Color c)
			{
				headerColor = c;
			});
			optionsPanel.AddColorOption(tabIndex, "Laser Pointer Color", laserPointerColor, delegate (Color c)
			{
				laserPointerColor = c;
			});
		}



		public static string ColorString(string text, Color color)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
		}
	}
}
