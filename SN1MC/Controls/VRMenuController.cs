using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace SN1MC.Controls
{
    extern alias SteamVRActions;
    class VRMenuController : MonoBehaviour
    {
        private static VRMenuController _main;
        public static VRMenuController main
        {
            get
            {
                if (_main == null)
                {
                    _main = new VRMenuController();
                }
                return _main;
            }
        }
        public static GameObject rightController;
        public static GameObject leftController;
        public static bool inMainMenu = false;
        public static LaserPointerMenu laserPointer;
        public void InitializeMenu()
        {
            if(!VRCustomOptionsMenu.EnableMotionControls)
            {
                VRCustomOptionsMenu.EnableMotionControls = true;
            }
            if (VRCustomOptionsMenu.EnableMotionControls)
            {
                if (!VROptions.gazeBasedCursor)
                {
                    VROptions.gazeBasedCursor = true;
                }
                GameInput.SetupDefaultControllerBindings();
                rightController = new GameObject("rightController");
                leftController = new GameObject("leftController");

                if (laserPointer == null)
                    rightController.AddComponent<LaserPointerMenu>();
                laserPointer = rightController.GetComponent<LaserPointerMenu>();


            }
            inMainMenu = true;
            var cam = Camera.current.transform;
            rightController.transform.parent = cam.transform;
            leftController.transform.parent = cam.transform;
            laserPointer.transform.parent = cam.transform;
            Debug.Log("InitializeMenu Done");

        }
        public static Vector3 MainMenuRaycast()
        {
            var pos = rightController.transform.position + rightController.transform.forward * FPSInputModule.current.maxInteractionDistance;
            laserPointer.LaserPointerSet(Camera.current.WorldToScreenPoint(Camera.current.transform.position + Camera.current.transform.forward * FPSInputModule.current.maxInteractionDistance));
            return Camera.current.WorldToScreenPoint(pos);
        }

        public void UpdateMenuPositions()
        {
            SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRUI.Activate();
            SteamVRActions.Valve.VR.SteamVR_Actions.SubnauticaVRMain.Deactivate();
            if (SN1MC.UsingSteamVR)
            {
                rightController.transform.localPosition = Vector3.Lerp(rightController.transform.localPosition, VRInputManager.RightControllerPosition, VRCustomOptionsMenu.ikSmoothing);
                rightController.transform.localRotation = Quaternion.Lerp(rightController.transform.localRotation, VRInputManager.RightControllerRotation, VRCustomOptionsMenu.ikSmoothing);

                leftController.transform.localPosition = Vector3.Lerp(leftController.transform.localPosition, VRInputManager.LeftControllerPosition, VRCustomOptionsMenu.ikSmoothing);
                leftController.transform.localRotation = Quaternion.Lerp(leftController.transform.localRotation, VRInputManager.LeftControllerRotation, VRCustomOptionsMenu.ikSmoothing);
            }
            else
            {
                //ErrorMessage.AddDebug("Track: " + SN1MC.vrSystem.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0, allPoses));
                XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos);
                XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRot);

                rightController.transform.localPosition = Vector3.Lerp(rightController.transform.localPosition, rightPos, VRCustomOptionsMenu.ikSmoothing);
                rightController.transform.localRotation = Quaternion.Lerp(rightController.transform.localRotation, rightRot, VRCustomOptionsMenu.ikSmoothing);

                XRInputManager.GetXRInputManager().leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos);
                XRInputManager.GetXRInputManager().leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRot);

                leftController.transform.localPosition = Vector3.Lerp(leftController.transform.localPosition, leftPos, VRCustomOptionsMenu.ikSmoothing);
                leftController.transform.localRotation = Quaternion.Lerp(leftController.transform.localRotation, leftRot, VRCustomOptionsMenu.ikSmoothing);
            }
        }

        [HarmonyPatch(typeof(uGUI_MainMenu), nameof(uGUI_MainMenu.Awake))]
        public static class uGUI_MainMenu_Awake_Patch
        {
            [HarmonyPostfix]
            public static void Postffix(ArmsController __instance)
            {
                if (!XRSettings.enabled || !VRCustomOptionsMenu.EnableMotionControls)
                {
                    return;
                }
                main.InitializeMenu();
            }
        }

        [HarmonyPatch(typeof(uGUI_MainMenu), nameof(uGUI_MainMenu.Update))]
        public static class uGUI_Update_Patch
        {
            [HarmonyPostfix]
            public static void Postffix(uGUI_MainMenu __instance)
            {
                if (!XRSettings.enabled || !VRCustomOptionsMenu.EnableMotionControls)
                {
                    return;
                }
                main.UpdateMenuPositions();
            }
        }
    }
}
