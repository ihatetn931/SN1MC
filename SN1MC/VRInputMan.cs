using System.Collections.Generic;
using UnityEngine;

namespace SN1MC
{
    //Steamvr Input
    extern alias SteamVRRef;
    extern alias SteamVRActions;
    public class VRInputManager
    {
        static VRInputManager()
        {
            SetUpListeners();
        }

        public static Vector2 LeftAxis = Vector2.zero;
        public static Vector2 RightAxis = Vector2.zero;

        public static Vector3 RightControllerPosition;
        public static Quaternion RightControllerRotation;

        public static Vector3 LeftControllerPosition;
        public static Quaternion LeftControllerRotation;

        public bool GetButtonDown(GameInput.Button buttonName, SteamVRRef.Valve.VR.SteamVR_Input_Sources source)
        {
            GameInput.InputStateFlags inputStateFlags = (GameInput.InputStateFlags)0U;
            inputStateFlags |= GameInput.InputStateFlags.Down;
            return SteamVRRef.Valve.VR.SteamVR_Input.GetStateDown(buttonName.ToString(), source);
        }

        public bool GetButtonUp(GameInput.Button buttonName, SteamVRRef.Valve.VR.SteamVR_Input_Sources source)
        {
            GameInput.InputStateFlags inputStateFlags = (GameInput.InputStateFlags)0U;
            inputStateFlags |= GameInput.InputStateFlags.Up;
            return SteamVRRef.Valve.VR.SteamVR_Input.GetStateUp(buttonName.ToString(), source);
        }

        public bool GetButton(GameInput.Button buttonName, SteamVRRef.Valve.VR.SteamVR_Input_Sources source)
        {
            GameInput.InputStateFlags inputStateFlags = (GameInput.InputStateFlags)0U;
            inputStateFlags |= GameInput.InputStateFlags.Held;
            return SteamVRRef.Valve.VR.SteamVR_Input.GetState(buttonName.ToString(), source);
        }

        public static void SetUpListeners()
        {
            // BOOLEANS
            /*SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_LeftHand.AddOnStateDownListener(OnLeftHandDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.LeftHand);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_LeftHand.AddOnStateUpListener(OnLeftHandUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.LeftHand);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_RightHand.AddOnStateDownListener(OnRightHandDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.RightHand);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_RightHand.AddOnStateUpListener(OnRightHandUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.RightHand);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_PDA.AddOnStateDownListener(OnPdaPauseDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_PDA.AddOnStateUpListener(OnPdaPauseUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_LeftHand.AddOnStateDownListener(OnADown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_LeftHand.AddOnStateUpListener(OnAUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Exit.AddOnStateDownListener(OnBDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Exit.AddOnStateUpListener(OnBUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Reload.AddOnStateDownListener(OnXDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Reload.AddOnStateUpListener(OnXUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Jump.AddOnStateDownListener(OnYDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Jump.AddOnStateUpListener(OnYUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_MoveUp.AddOnStateDownListener(OnMoveUpDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_MoveUp.AddOnStateUpListener(OnMoveUpUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_MoveDown.AddOnStateDownListener(OnMoveDownDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_MoveDown.AddOnStateUpListener(OnMoveDownUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Sprint.AddOnStateDownListener(OnLeftJoystickPressedDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.LeftHand);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Sprint.AddOnStateUpListener(OnLeftJoystickPressedUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.LeftHand);

            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_TakePicture.AddOnStateDownListener(OnRightJoystickPressedDown, SteamVRRef.Valve.VR.SteamVR_Input_Sources.RightHand);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_TakePicture.AddOnStateUpListener(OnRightJoystickPressedUp, SteamVRRef.Valve.VR.SteamVR_Input_Sources.RightHand);*/


            // VECTOR 2Ds
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Move.AddOnUpdateListener(OnLeftJoystickUpdate, SteamVRRef.Valve.VR.SteamVR_Input_Sources.LeftHand);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_Look.AddOnUpdateListener(OnRightJoystickUpdate, SteamVRRef.Valve.VR.SteamVR_Input_Sources.RightHand);


            // POSES
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_RightHandPose.AddOnUpdateListener(SteamVRRef.Valve.VR.SteamVR_Input_Sources.RightHand, UpdateRightHand);
            SteamVRActions.Valve.VR.SteamVR_Actions.subnauticaVRMain_LeftHandPose.AddOnUpdateListener(SteamVRRef.Valve.VR.SteamVR_Input_Sources.LeftHand, UpdateLeftHand);
        }

        // VECTOR 2Ds
        //Left Joystick
        public static void OnLeftJoystickUpdate(SteamVRRef.Valve.VR.SteamVR_Action_Vector2 fromAction, SteamVRRef.Valve.VR.SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            LeftAxis.x = axis.x;
            LeftAxis.y = axis.y;
        }
        //Right Joystick
        public static void OnRightJoystickUpdate(SteamVRRef.Valve.VR.SteamVR_Action_Vector2 fromAction, SteamVRRef.Valve.VR.SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            RightAxis.x = axis.x;
            RightAxis.y = axis.y;
        }


        // POSES
        public static void UpdateRightHand(SteamVRRef.Valve.VR.SteamVR_Action_Pose fromAction, SteamVRRef.Valve.VR.SteamVR_Input_Sources fromSource)
        {
           // if (fromAction.poseIsValid)
           // {
                RightControllerPosition = fromAction.GetLocalPosition(fromSource);
                RightControllerRotation = fromAction.GetLocalRotation(fromSource);
           // }
        }

        public static void UpdateLeftHand(SteamVRRef.Valve.VR.SteamVR_Action_Pose fromAction, SteamVRRef.Valve.VR.SteamVR_Input_Sources fromSource)
        {
            //if (fromAction.poseIsValid)
            //{
                LeftControllerPosition = fromAction.GetLocalPosition(fromSource);
                LeftControllerRotation = fromAction.GetLocalRotation(fromSource);
            //}
        }
    }
}