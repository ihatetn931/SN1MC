
using HarmonyLib;
using RootMotion.FinalIK;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;


namespace SN1MC.Controls
{
    public class VRHandsController : MonoBehaviour
    {
        public static GameObject rightController;
        public static GameObject leftController;
        public static ArmsController armsController;
        public static Player player = Player.main;
        public static FullBodyBipedIK ik;
        public static FingerRig finger;
        private static VRHandsController _main;
        public static LaserPointer laserPointer;
        public static LaserPointer laserPointertest;
        public static LaserPointerLeft laserPointerLeft;
        public static float rotationX = 0;
        public static float rotationY = 0;
        public static float rotationZ = 0;
        public static float rotationW = 0;
        public static bool Started = false;
        public static Transform rightHand;
        public static Transform leftHand;

        public static VRHandsController main
        {
            get
            {
                if (_main == null)
                {
                    _main = new VRHandsController();
                }
                return _main;
            }
        }

        public void Initialize(ArmsController controller)
        {
            player = global::Utils.GetLocalPlayerComp();
            VRMenuController.inMainMenu = false;
            if (VRCustomOptionsMenu.EnableMotionControls)
            {
                ik = controller.GetComponent<FullBodyBipedIK>();
                armsController = controller;

                rightController = new GameObject("rightController");
                leftController = new GameObject("leftController");

                rightHand = new GameObject().transform;
                leftHand = new GameObject().transform;

                if (laserPointer == null)
                rightController.AddComponent<LaserPointer>();
                 laserPointer = rightController.GetComponent<LaserPointer>();

                if (laserPointerLeft == null)
                    leftController.AddComponent<LaserPointerLeft>();
                laserPointerLeft = leftController.GetComponent<LaserPointerLeft>();
            }
            var cam = Player.main.camRoot.transform;
            rightController.transform.parent = player.camRoot.transform;
            leftController.transform.parent = player.camRoot.transform;
            laserPointer.transform.parent = rightController.transform;
            laserPointerLeft.transform.parent = leftController.transform;
            Started = true;

        }

        public void UpdateHandPositions()
        {

            if (!Input.GetKeyDown(KeyCode.LeftControl) && !Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    rotationX += 1f;
                }
                if (Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    rotationX -= 1f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    rotationY += 0.001f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad9))
                {
                    rotationY -= 0.001f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    rotationZ += 0.001f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    rotationZ -= 0.001f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    rotationW += 0.001f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    rotationW -= 0.001f;
                }
                if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    //GameOptions.enableVrAnimations = !GameOptions.enableVrAnimations;
                    //ErrorMessage.AddDebug("VRAnimations: " + GameOptions.enableVrAnimations);
                }
                if (Input.GetKeyDown(KeyCode.Keypad0))
                {
                    rotationX = 0;
                    rotationY = 0;
                    rotationZ = 0;
                    rotationW = 0;
                }
            }

            PlayerTool playerTool = armsController.guiHand.GetTool();

            //Get Right controller Position and Rotation
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

            if (Vehicles.CyclopsSubControl.isPilot)
            {
                if (Vehicles.CyclopsSubControl.rightHandAttached && !Vehicles.CyclopsSubControl.leftHandAttached)
                {
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation;
                }
                else
                {
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.006f, 0.005f, -0.007000001f, -0.008f);
                }
                if (Vehicles.CyclopsSubControl.leftHandAttached && !Vehicles.CyclopsSubControl.rightHandAttached)
                {
                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation;
                }
                else
                {
                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                }
                if (Vehicles.CyclopsSubControl.rightHandAttached && Vehicles.CyclopsSubControl.leftHandAttached)
                {
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation;
                }
                else
                {
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.006f, 0.005f, -0.007000001f, -0.008f);

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                }
                //ik.solver.rightHandEffector.positionWeight = 1;
                //ik.solver.rightHandEffector.rotationWeight = 1;
                //ik.solver.rightArmChain.bendConstraint.weight = 0;

                //ik.solver.leftHandEffector.positionWeight = 1;
                //ik.solver.leftHandEffector.rotationWeight = 1;
                //ik.solver.leftArmChain.bendConstraint.weight = 0;

                armsController.reconfigureWorldTarget = true;
                armsController.rightWorldTarget = rightHand;
                armsController.leftWorldTarget =  leftHand;

            }
            if (playerTool != null)
            {
                rightHand.position = playerTool.transform.position + rightController.transform.position;
                rightHand.rotation = playerTool.transform.rotation * rightController.transform.rotation;
                if (playerTool.GetComponent<Floater>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.006f, 0.005f, -0.007000001f, -0.008f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Creature>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.006f, 0.005f, -0.007000001f, -0.008f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<FlashLight>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.006f, 0.005f, -0.007000001f, -0.008f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Knife>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.02f, -0.006f, 0.014f, 0.01f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<AirBladder>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.014f, 0.004f, -0.013f, -0.013f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Flare>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.026f, -0.012f, 0.03699999f, 0.02f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<LaserCutter>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.023f, 0.009000001f, -0.017f, -0.015f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<DiveReel>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.013f, 0.006f, -0.013f, -0.013f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<ScannerTool>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.01f, -0.003f, 0.014f, 0.01f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<PropulsionCannon>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.01f, -0.007f, 0.005f, 0.006000001f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<RepulsionCannon>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.031f, 0.018f, -0.017f, -0.015f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<OxygenPipe>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.04799997f, 0.024f, -0.03599999f, -0.03899999f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<PipeSurfaceFloater>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    // ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    // ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.04799997f, 0.024f, -0.03599999f, -0.03899999f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Welder>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.017f, -0.005f, 0.014f, 0.01f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<StasisRifle>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.02f, -0.006f, 0.014f, 0.01f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<FireExtinguisher>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.026f, -0.012f, 0.03699999f, 0.02f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<LEDLight>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.026f, -0.012f, 0.03699999f, 0.02f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Flare>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.026f, -0.012f, 0.03699999f, 0.02f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Beacon>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.026f, 0.013f, -0.027f, -0.026f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Gravsphere>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(rotationX, rotationY, rotationZ, rotationW);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Constructor>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(rotationX, rotationY, rotationZ, rotationW);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<DeployableStorage>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(-0.009f, -0.005000001f, -0.009000001f, -0.005000001f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }

                if (playerTool.GetComponent<BuilderTool>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.026f, -0.012f, 0.03699999f, 0.025f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;
                }
                if (playerTool.GetComponent<Seaglide>())
                {
                    //ErrorMessage.AddDebug("RotationX: " + rotationX);
                    //ErrorMessage.AddDebug("RotationY: " + rotationY);
                    //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                    //ErrorMessage.AddDebug("RotationW: " + rotationW);
                    rightHand.position = rightController.transform.position;
                    rightHand.rotation = rightController.transform.rotation * new Quaternion(0.028f, -0.016f, 0.04899997f, 0.025f);
                    ik.solver.rightHandEffector.positionWeight = 1;
                    ik.solver.rightHandEffector.rotationWeight = 1;
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightArmChain.bendConstraint.weight = 0;

                    leftHand.position = leftController.transform.position;
                    leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                    ik.solver.leftHandEffector.positionWeight = 1;
                    ik.solver.leftHandEffector.rotationWeight = 1;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftArmChain.bendConstraint.weight = 0;
                }
            }
            if (player.GetPDA().isOpen)
            {
                //ErrorMessage.AddDebug("RotationX: " + rotationX);
                //ErrorMessage.AddDebug("RotationY: " + rotationY);
                //ErrorMessage.AddDebug("RotationZ: " + rotationZ);
                //ErrorMessage.AddDebug("RotationW: " + rotationW);
                leftHand.position = leftController.transform.position;
                leftHand.rotation = leftController.transform.rotation * new Quaternion(0.006000001f, -0.011f, -0.004f, -0.013f);
                ik.solver.leftHandEffector.positionWeight = 1;
                ik.solver.leftHandEffector.rotationWeight = 1;
                ik.solver.leftHandEffector.target = leftHand;
                laserPointer.pointerDot.transform.localScale = new Vector3(0.009f, 0.009f, 0.009f);
                ik.solver.leftArmChain.bendConstraint.weight = 0;

                if (!laserPointer.gameObject.activeSelf)
                    laserPointer.gameObject.SetActive(true);
                if (!laserPointer.pointerDot.activeSelf)
                    laserPointer.pointerDot.SetActive(true);
            }
            else
            {
                laserPointer.pointerDot.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            }
            if (player.inSeamoth)
            {
                //laserPointer.gameObject.SetActive(false);
                //laserPointer.pointerDot.SetActive(false);
            }
            else
            {
                // laserPointer.gameObject.SetActive(true);
                // laserPointer.pointerDot.SetActive(true);
            }
            if (player.inExosuit)
            {
                laserPointerLeft.gameObject.SetActive(true);
                laserPointerLeft.pointerDot.SetActive(true);
            }
            else
            {
                laserPointerLeft.gameObject.SetActive(false);
                laserPointerLeft.pointerDot.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(ArmsController), nameof(ArmsController.Start))]
        public static class ArmsController_Start_Patch
        {
            [HarmonyPostfix]
            public static void Postffix(ArmsController __instance)
            {
                if (!XRSettings.enabled || !VRCustomOptionsMenu.EnableMotionControls)
                {
                    return;
                }
                main.Initialize(__instance);
            }
        }

        [HarmonyPatch(typeof(ArmsController), nameof(ArmsController.Update))]
        class ArmsController_LateUpdate_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ArmsController __instance)
            {
                if (!XRSettings.enabled || !VRCustomOptionsMenu.EnableMotionControls || Player.main == null || Started == false || uGUI.main.loading.isLoading)
                {
                    return;
                }
                main.UpdateHandPositions();
            }
        }
    }


    [HarmonyPatch(typeof(uGUI_GraphicRaycaster), nameof(uGUI_GraphicRaycaster.UpdateGraphicRaycasters))]
    class uGUI_GraphicRaycaster_UpdateGraphicRaycasters_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(uGUI_GraphicRaycaster __instance)
        {
            foreach (uGUI_GraphicRaycaster uGUI_GraphicRaycaster in uGUI_GraphicRaycaster.allRaycasters)
            {
                if (uGUI_GraphicRaycaster.updateRaycasterStatusDelegate != null)
                {
                    uGUI_GraphicRaycaster.updateRaycasterStatusDelegate(uGUI_GraphicRaycaster);
                }
                else if (uGUI_GraphicRaycaster.interactionDistance > 0f)
                {
                    if (Vector3.SqrMagnitude(VRHandsController.rightController.transform.position - uGUI_GraphicRaycaster.transform.position) < uGUI_GraphicRaycaster.interactionDistance * uGUI_GraphicRaycaster.interactionDistance)
                    {
                        uGUI_GraphicRaycaster.enabled = true;
                    }
                    else
                    {
                        uGUI_GraphicRaycaster.enabled = false;
                    }
                    if (Vector3.SqrMagnitude(VRHandsController.leftController.transform.position - uGUI_GraphicRaycaster.transform.position) < uGUI_GraphicRaycaster.interactionDistance * uGUI_GraphicRaycaster.interactionDistance)
                    {
                        uGUI_GraphicRaycaster.enabled = true;
                    }
                    else
                    {
                        uGUI_GraphicRaycaster.enabled = false;
                    }
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(ArmsController), nameof(ArmsController.Reconfigure))]
    class ArmsController_Reconfigure_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerTool tool, ArmsController __instance)
        {
            //__instance.ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).bendGoal = __instance.leftHandElbow;
            //__instance.ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).weight = 1f;
            if (tool == null)
            {
                __instance.leftAim.shouldAim = false;
                __instance.rightAim.shouldAim = false;
                __instance.ik.solver.leftHandEffector.target = null;
                __instance.ik.solver.rightHandEffector.target = null;
                if (!__instance.pda.isActiveAndEnabled)
                {
                    if (__instance.leftWorldTarget)
                    {
                        __instance.ik.solver.leftHandEffector.target = __instance.leftWorldTarget;
                        __instance.ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).bendGoal = null;
                        __instance.ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).weight = 0f;
                    }
                    if (__instance.rightWorldTarget)
                    {
                        __instance.ik.solver.rightHandEffector.target = __instance.rightWorldTarget;
                        return false;
                    }
                    __instance.reconfigureWorldTarget = false;
                }
            }
            else
            {
                if (!__instance.IsBleederAttached())
                {
                    __instance.leftAim.shouldAim = tool.ikAimLeftArm;
                    if (tool.useLeftAimTargetOnPlayer)
                    {
                        __instance.ik.solver.leftHandEffector.target = __instance.attachedLeftHandTarget;
                    }
                    else
                    {
                        __instance.ik.solver.leftHandEffector.target = tool.leftHandIKTarget;
                    }
                }
                else
                {
                    __instance.leftAim.shouldAim = tool.ikAimRightArm;
                    __instance.ik.solver.leftHandEffector.target = null;
                }
                __instance.rightAim.shouldAim = tool.ikAimRightArm;
                __instance.ik.solver.rightHandEffector.target = tool.rightHandIKTarget;
            }
            return false;
        }
    }
}



