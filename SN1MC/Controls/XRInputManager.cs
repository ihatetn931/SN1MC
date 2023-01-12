using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using HarmonyLib;
using System;
using Platform.Utils;
using FMODUnity;
using UnityEngine.Events;

namespace SN1MC
{
    extern alias SteamVRRef;
    extern alias SteamVRActions;
    public enum VRControllerLayout
    {
        Automatic,
        Xbox360,
        XboxOne,
        PS4,
        Oculus,
        Valve,
        HTC,
        HP
    }
    enum Controller
    {
        Left,
        Right
    }



    internal class XRInputManager
    {

        private static readonly XRInputManager _instance = new XRInputManager();
        private readonly List<InputDevice> xrDevices = new List<InputDevice>();
        public InputDevice leftController;
        public InputDevice rightController;


        private XRInputManager()
        {
            GetDevices();
        }

        public static XRInputManager GetXRInputManager()
        {
            return _instance;
        }

        void GetDevices()
        {
            InputDevices.GetDevices(xrDevices);

            foreach (InputDevice device in xrDevices)
            {
                if (device.name.Contains("Left"))
                {
                    leftController = device;
                }
                if (device.name.Contains("Right"))
                {
                    rightController = device;
                }
            }
        }

        InputDevice GetDevice(Controller name)
        {
            switch (name)
            {
                case Controller.Left:
                    return leftController;
                case Controller.Right:
                    return rightController;
                default: throw new Exception();
            }
        }

        public Vector2 Get(Controller controller, InputFeatureUsage<Vector2> usage)
        {
            InputDevice device = GetDevice(controller);
            Vector2 value = Vector2.zero;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public Vector3 Get(Controller controller, InputFeatureUsage<Vector3> usage)
        {
            InputDevice device = GetDevice(controller);
            Vector3 value = Vector3.zero;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public Quaternion Get(Controller controller, InputFeatureUsage<Quaternion> usage)
        {
            InputDevice device = GetDevice(controller);
            Quaternion value = Quaternion.identity;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public float Get(Controller controller, InputFeatureUsage<float> usage)
        {
            InputDevice device = GetDevice(controller);
            float value = 0f;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public bool Get(Controller controller, InputFeatureUsage<bool> usage)
        {
            InputDevice device = GetDevice(controller);
            bool value = false;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public bool hasControllers()
        {
            bool hasController = false;
            if (leftController != null && leftController.isValid)
            {
                hasController = true;
            }
            if (rightController != null && rightController.isValid)
            {
                hasController = true;
            }
            return hasController;
        }

        [HarmonyPatch(typeof(GameInput), nameof(GameInput.UpdateAxisValues))]
        internal class UpdateAxisValuesPatch
        {
            public static bool Prefix(bool useKeyboard, bool useController, GameInput ___instance)
            {
                XRInputManager xrInput = GetXRInputManager();

                for (int i = 0; i < GameInput.axisValues.Length; i++)
                {
                    GameInput.axisValues[i] = 0f;
                }
                if (SN1MC.UsingSteamVR)
                {
                    Vector2 vec = new Vector2(VRInputManager.LeftAxis.x, VRInputManager.LeftAxis.y);
                    GameInput.axisValues[2] = vec.x;
                    GameInput.axisValues[3] = -vec.y;
                    Vector2 vec2 = new Vector2(VRInputManager.RightAxis.x, VRInputManager.RightAxis.y);
                    GameInput.axisValues[0] = vec2.x;
                    GameInput.axisValues[1] = -vec2.y;
                    // TODO: Use deadzone?
                    GameInput.axisValues[4] = xrInput.Get(Controller.Left, CommonUsages.trigger).CompareTo(0.3f);
                    GameInput.axisValues[5] = xrInput.Get(Controller.Right, CommonUsages.trigger).CompareTo(0.3f);

                    //These axis I'm sure are used for something on other headsets
                    //axisValues[6] = xrInput.Get(Controller.Left, CommonUsages.secondary2DAxisTouch).CompareTo(0.1f);
                    //axisValues[7] = xrInput.Get(Controller.Right, CommonUsages.secondaryTouch).CompareTo(0.1f);
                    //  }
                }


                if (useController && !SN1MC.UsingSteamVR)
                {
                    if (GameInput.GetUseOculusInputManager())
                    {
                        Vector2 vector = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
                        GameInput.axisValues[2] = vector.x;
                        GameInput.axisValues[3] = -vector.y;
                        Vector2 vector2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
                        GameInput.axisValues[0] = vector2.x;
                        GameInput.axisValues[1] = -vector2.y;
                        GameInput.axisValues[4] = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.Active);
                        GameInput.axisValues[5] = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.Active);
                        GameInput.axisValues[6] = 0f;
                        if (OVRInput.Get(OVRInput.RawButton.DpadLeft, OVRInput.Controller.Active))
                        {
                            GameInput.axisValues[6] -= 1f;
                        }
                        if (OVRInput.Get(OVRInput.RawButton.DpadRight, OVRInput.Controller.Active))
                        {
                            GameInput.axisValues[6] += 1f;
                        }
                        GameInput.axisValues[7] = 0f;
                        if (OVRInput.Get(OVRInput.RawButton.DpadUp, OVRInput.Controller.Active))
                        {
                            GameInput.axisValues[7] += 1f;
                        }
                        if (OVRInput.Get(OVRInput.RawButton.DpadDown, OVRInput.Controller.Active))
                        {
                            GameInput.axisValues[7] -= 1f;
                        }
                    }
                    else
                    {
                        GameInput.ControllerLayout controllerLayout = GameInput.GetControllerLayout();
                        if (controllerLayout == GameInput.ControllerLayout.Xbox360 || controllerLayout == GameInput.ControllerLayout.XboxOne || Application.platform == RuntimePlatform.PS4)
                        {
                            GameInput.axisValues[2] = Input.GetAxis("ControllerAxis1");
                            GameInput.axisValues[3] = Input.GetAxis("ControllerAxis2");
                            GameInput.axisValues[0] = Input.GetAxis("ControllerAxis4");
                            GameInput.axisValues[1] = Input.GetAxis("ControllerAxis5");
                            if (Application.platform == RuntimePlatform.PS4)
                            {
                                GameInput.axisValues[4] = Mathf.Max(Input.GetAxis("ControllerAxis8"), 0f);
                            }
                            else
                            {
                                GameInput.axisValues[4] = Mathf.Max(Input.GetAxis("ControllerAxis3"), 0f);
                            }
                            GameInput.axisValues[5] = Mathf.Max(-Input.GetAxis("ControllerAxis3"), 0f);
                            GameInput.axisValues[6] = Input.GetAxis("ControllerAxis6");
                            GameInput.axisValues[7] = Input.GetAxis("ControllerAxis7");
                        }
                        else if (controllerLayout == GameInput.ControllerLayout.PS4)
                        {
                            GameInput.axisValues[2] = Input.GetAxis("ControllerAxis1");
                            GameInput.axisValues[3] = Input.GetAxis("ControllerAxis2");
                            GameInput.axisValues[0] = Input.GetAxis("ControllerAxis3");
                            GameInput.axisValues[1] = Input.GetAxis("ControllerAxis6");
                            GameInput.axisValues[4] = (Input.GetAxisRaw("ControllerAxis4") + 1f) * 0.5f;
                            GameInput.axisValues[5] = (Input.GetAxisRaw("ControllerAxis5") + 1f) * 0.5f;
                            GameInput.axisValues[6] = Input.GetAxis("ControllerAxis7");
                            GameInput.axisValues[7] = Input.GetAxis("ControllerAxis8");
                        }
                    }
                }
                if (useKeyboard)
                {
                    GameInput.axisValues[10] = Input.GetAxis("Mouse ScrollWheel");
                    GameInput.axisValues[8] = Input.GetAxisRaw("Mouse X");
                    GameInput.axisValues[9] = Input.GetAxisRaw("Mouse Y");
                }
                for (int j = 0; j < GameInput.axisValues.Length; j++)
                {
                    GameInput.AnalogAxis axis = (GameInput.AnalogAxis)j;
                    GameInput.Device deviceForAxis = ___instance.GetDeviceForAxis(axis);
                    float f = GameInput.lastAxisValues[j] - GameInput.axisValues[j];
                    GameInput.lastAxisValues[j] = GameInput.axisValues[j];
                    if (deviceForAxis != GameInput.lastDevice)
                    {
                        float num = 0.1f;
                        if (Mathf.Abs(f) > num)
                        {
                            if (!PlatformUtils.isConsolePlatform)
                            {
                                GameInput.lastDevice = deviceForAxis;
                            }
                        }
                        else
                        {
                            GameInput.axisValues[j] = 0f;
                        }
                    }
                }

                return false;
            }
        }

        public static string GetAllButtonPushesVR(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.JoystickButton0:
                    // ControllerButtonA
                    return "JoystickButton0 Down";
                case KeyCode.JoystickButton1:
                    // ControllerButtonB
                    return "JoystickButton1 Down";
                case KeyCode.JoystickButton2:
                    // ControllerButtonY
                    return "JoystickButton2 Down";
                case KeyCode.JoystickButton3:
                    // ControllerButtonX
                    return "JoystickButton3 Down";
                case KeyCode.JoystickButton4:
                    // ControllerButtonLeftBumper
                    return "JoystickButton4 Down";
                case KeyCode.JoystickButton5:
                    // ControllerButtonRightBumper
                    return "JoystickButton5 Down";
                case KeyCode.JoystickButton6:
                    return "JoystickButton6 Down";
                case KeyCode.JoystickButton7:
                    // ControllerButtonHome
                    return "JoystickButton7 Down";
                case KeyCode.JoystickButton8:
                    // ControllerButtonLeftStick
                    return "JoystickButton8 Down";
                case KeyCode.JoystickButton9:
                    // ControllerButtonRightStick
                    return "JoystickButton9 Down";
                case KeyCode.JoystickButton10:
                    // ControllerButtonRightStick
                    return "JoystickButton10 Down";
                case KeyCode.JoystickButton11:
                    // ControllerButtonRightStick
                    return "JoystickButton11 Down";
                case KeyCode.JoystickButton12:
                    // ControllerButtonRightStick
                    return "JoystickButton12 Down";
                case KeyCode.JoystickButton13:
                    // ControllerButtonRightStick
                    return "JoystickButton13 Down";
                case KeyCode.JoystickButton14:
                    // ControllerButtonRightStick
                    return "JoystickButton14 Down";
                case KeyCode.JoystickButton15:
                    // ControllerButtonRightStick
                    return "JoystickButton15 Down";
                case KeyCode.JoystickButton16:
                    // ControllerButtonRightStick
                    return "JoystickButton16 Down";
                case KeyCode.JoystickButton17:
                    // ControllerButtonRightStick
                    return "JoystickButton17 Down";
                case KeyCode.JoystickButton18:
                    // ControllerButtonRightStick
                    return "JoystickButton18 Down";
                case KeyCode.JoystickButton19:
                    // ControllerButtonRightStick
                    return "JoystickButton19 Down";
                case KeyCode.Joystick1Button0:
                    // ControllerButtonA
                    return "Joystick1Button0 Down";
                case KeyCode.Joystick1Button1:
                    // ControllerButtonB
                    return "Joystick1Button1 Down";
                case KeyCode.Joystick1Button2:
                    // ControllerButtonY
                    return "Joystick1Button2 Down";
                case KeyCode.Joystick1Button3:
                    // ControllerButtonX
                    return "Joystick1Button3 Down";
                case KeyCode.Joystick1Button4:
                    // ControllerButtonLeftBumper
                    return "Joystick1Button4 Down";
                case KeyCode.Joystick1Button5:
                    // ControllerButtonRightBumper
                    return "Joystick1Button5 Down";
                case KeyCode.Joystick6Button6:
                    return "Joystick1Button6 Down";
                case KeyCode.Joystick1Button7:
                    // ControllerButtonHome
                    return "Joystick1Button7 Down";
                case KeyCode.Joystick1Button8:
                    // ControllerButtonLeftStick
                    return "Joystick1Button8 Down";
                case KeyCode.Joystick1Button9:
                    // ControllerButtonRightStick
                    return "Joystick1Button9 Down";
                case KeyCode.Joystick1Button10:
                    // ControllerButtonRightStick
                    return "Joystick1Button10 Down";
                case KeyCode.Joystick1Button11:
                    // ControllerButtonRightStick
                    return "Joystick1Button11 Down";
                case KeyCode.Joystick1Button12:
                    // ControllerButtonRightStick
                    return "Joystick1Button12 Down";
                case KeyCode.Joystick1Button13:
                    // ControllerButtonRightStick
                    return "Joystick1Button13 Down";
                case KeyCode.Joystick1Button14:
                    // ControllerButtonRightStick
                    return "JoystickButton14 Down";
                case KeyCode.Joystick1Button15:
                    // ControllerButtonRightStick
                    return "Joystick1Button15 Down";
                case KeyCode.Joystick1Button16:
                    // ControllerButtonRightStick
                    return "Joystick1Button16 Down";
                case KeyCode.Joystick1Button17:
                    // ControllerButtonRightStick
                    return "JoystickButton17 Down";
                case KeyCode.Joystick1Button18:
                    // ControllerButtonRightStick
                    return "JoystickButton18 Down";
                case KeyCode.Joystick1Button19:
                    // ControllerButtonRightStick
                    return "Joystick1Button19 Down";
                default:
                    return "None Down";
            }
        }

       

        public enum SteamVRButtonCode
        {
            Jump,
            PDA,
            Deconstruct,
            Exit,
            LeftHand,
            RightHand,
            CycleNext,
            CyclePrev,
            Slot1,
            Slot2,
            Slot3,
            Slot4,
            Slot5,
            AltTool,
            TakePicture,
            Reload,
            Sprint,
            MoveUp,
            MoveDown,
            MoveForward,
            MoveBackward,
            MoveLeft,
            MoveRight,
            AutoMove,
            LookUp,
            LookDown,
            LookLeft,
            LookRight,
            UISubmit,
            UICancel,
            UIClear,
            UIAssign,
            UILeft,
            UIRight,
            UIUp,
            UIDown,
            UIMenu,
            UIAdjustLeft,
            UIAdjustRight,
            UINextTab,
            UIPrevTab,
            Feedback,
            UIRightStickAdjustLeft,
            UIRightStickAdjustRight,
            None
        }




        [HarmonyPatch(typeof(FPSInputModule), nameof(FPSInputModule.EscapeMenu))]
        class FPSInputModule_EscapeMenu_Patch
        {
            [HarmonyPrefix]
            static void Prefix(FPSInputModule __instance)
            {
                VRInputManager vrInput = new VRInputManager();
                if (__instance.lockPauseMenu)
                {
                    return;
                }
                if (GameInput.GetButtonDown(GameInput.Button.UIMenu) && IngameMenu.main != null && !IngameMenu.main.selected)
                {
                    IngameMenu.main.Open();
                    GameInput.ClearInput();
                }
            }
        }
    }
}



