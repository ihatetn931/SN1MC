
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SN1MC.Controls
{
	public static class CustomUpdateMoveDirection
	{
		public static Vector3 moveDirection;
		public static void UpdateMoveDirection(Transform rightHand, Transform leftHand, string controllerAttached)
		{
			if (controllerAttached != null)
			{
				float num = 0f;
				num += GameInput.GetAnalogValueForButton(GameInput.Button.MoveForward);
				num -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveBackward);
				//float num2 = 0f;

				float PitchSpeed = 0.8f;
				float RotationSpeed = 3.0f;

				Vector3 YawRight = Vector3.zero;
				Vector3 PitchRight = Vector3.zero;

				Vector3 YawLeft = Vector3.zero;
				Vector3 PitchLeft = Vector3.zero;

				Vector3 rightPos = VRHandsController.rightController.transform.localPosition;
				Vector3 leftPos = VRHandsController.leftController.transform.localPosition;

				Quaternion rightRot = VRHandsController.rightController.transform.localRotation;
				Quaternion leftRot = VRHandsController.leftController.transform.localRotation;

				PitchRight = rightHand.transform.localPosition + new Vector3(0, 0, -rightRot.x) * PitchSpeed;
				YawRight = rightHand.transform.localPosition + new Vector3(rightRot.x, rightRot.y, 0) * RotationSpeed;

				PitchLeft = leftHand.transform.localPosition + new Vector3(0, 0, -leftRot.x) * PitchSpeed;
				YawLeft = leftHand.transform.localPosition + new Vector3(-leftRot.x, -leftRot.y, 0) * RotationSpeed;

				//num2 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveLeft);
				//num2 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveRight);
				//float num3 = 0f;
				//num3 += GameInput.GetAnalogValueForButton(GameInput.Button.MoveUp);
				//num3 -= GameInput.GetAnalogValueForButton(GameInput.Button.MoveDown);

				float xRotation = 0;
				float yRotation = 0;

				if (controllerAttached == "Left")
				{
					if (Mathf.Clamp(YawLeft.x, -1, 1) > 0.1f || Mathf.Clamp(YawLeft.x, -1, 1) < -0.1f)
						xRotation = Mathf.Clamp(YawLeft.x, -1, 1);
					if (Mathf.Clamp(PitchLeft.z, -1, 1) > 0.1f || Mathf.Clamp(PitchLeft.z, -1, 1) < -0.1f)
						yRotation = Mathf.Clamp(PitchLeft.z, -1, 1);
				}
				if (controllerAttached == "Both")
				{
					if (Mathf.Clamp(YawRight.x + YawLeft.x, -1, 1) > 0.1f || Mathf.Clamp(YawRight.x + YawLeft.x, -1, 1) < -0.1f)
						xRotation = Mathf.Clamp(YawRight.x + YawLeft.x, -1, 1);
					if (Mathf.Clamp(PitchRight.z + PitchLeft.z, -1, 1) > 0.1f || Mathf.Clamp(PitchRight.z + PitchLeft.z, -1, 1) < -0.1f)
						yRotation = Mathf.Clamp(PitchRight.z + PitchLeft.z, -1, 1);
				}
				if (controllerAttached == "Right")
				{
					if (Mathf.Clamp(YawRight.x, -1, 1) > 0.1f || Mathf.Clamp(YawRight.x, -1, 1) < -0.1f)
						xRotation = Mathf.Clamp(YawRight.x, -1, 1);
					if (Mathf.Clamp(PitchRight.z, -1, 1) > 0.1f || Mathf.Clamp(PitchRight.z, -1, 1) < -0.1f)
						yRotation = Mathf.Clamp(PitchRight.z, -1, 1);
				}



				if (GameInput.autoMove && num * num + xRotation * xRotation > 0.010000001f)
				{
					GameInput.autoMove = false;
				}
				if (GameInput.autoMove)
				{
					moveDirection.Set(0f, yRotation, 1f);
				}
				else
				{
					moveDirection.Set(xRotation, yRotation, num);
				}
				if (GameInput.IsPrimaryDeviceGamepad())
				{
					if (GameInput.autoMove)
					{
						GameInput.isRunningMoveThreshold = false;
						return;
					}
					GameInput.isRunningMoveThreshold = (moveDirection.sqrMagnitude > 0.80999994f);
					if (!GameInput.isRunningMoveThreshold)
					{
						moveDirection /= 0.9f;
					}
				}
			}
		}


		public static Vector3 GetMoveDirection()
		{
			return moveDirection;
		}
	}
}
