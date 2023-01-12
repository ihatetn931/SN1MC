/*
using HarmonyLib;

namespace SN1MC.Controls
{
	extern alias SteamVRRef;
	class PlayerInputPatches
    {
		[HarmonyPatch(typeof(Player), nameof(Player.GetLeftHandDown))]
		public static class Player_GetLeftHandDown__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result)
			{
				bool leftHandDown = false;
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					leftHandDown = vrInput.GetButtonDown("LeftHand", SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					leftHandDown = GameInput.GetButtonDown(GameInput.Button.LeftHand);
				__result = leftHandDown;
				return false;
			}
		}

		[HarmonyPatch(typeof(Player), nameof(Player.GetLeftHandUp))]
		public static class Player_GetLeftHandUp__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result)
			{
				bool leftHandDown = false;
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					leftHandDown = vrInput.GetButtonUp("LeftHand", SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					leftHandDown = GameInput.GetButtonDown(GameInput.Button.LeftHand);
				__result = leftHandDown;
				return false;
			}
		}


		[HarmonyPatch(typeof(Player), nameof(Player.GetRightHandDown))]
		public static class Player_GetRightHandDown__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result)
			{
				bool leftHandDown = false;
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					leftHandDown = vrInput.GetButtonDown("RightHand", SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					leftHandDown = GameInput.GetButtonDown(GameInput.Button.RightHand);
				__result = leftHandDown;
				return false;
			}
		}

		[HarmonyPatch(typeof(Player), nameof(Player.GetRightHandUp))]
		public static class Player_GetRightHandUp__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result)
			{
				bool leftHandDown = false;
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					leftHandDown = vrInput.GetButtonUp("RightHand", SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					leftHandDown = GameInput.GetButtonUp(GameInput.Button.RightHand);
				__result = leftHandDown;
				return false;
			}
		}

		[HarmonyPatch(typeof(Player), nameof(Player.GetReloadUp))]
		public static class Player_GetReloadUp__Patch
		{
			[HarmonyPrefix]
			static bool Prefix(ref bool __result)
			{
				bool leftHandDown = false;
				VRInputManager vrInput = new VRInputManager();
				if (SN1MC.UsingSteamVR)
					leftHandDown = vrInput.GetButtonUp("Reload", SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
				else
					leftHandDown = GameInput.GetButtonUp(GameInput.Button.Reload);
				__result = leftHandDown;
				return false;
			}
		}
	}
}*/
