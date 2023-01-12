/*using System;
using UnityEngine;

public class AimIKTargetVerticalRotationLimit : MonoBehaviour, IManagedLateUpdateBehaviour, IManagedBehaviour
{
	public string GetProfileTag()
	{
		return "AimIKTargetVerticalRotationLimit";
	}

	public int managedLateUpdateIndex { get; set; }

	public void SetRotationLimit(float minRotation, float maxRotation)
	{
		if (minRotation <= -90f && maxRotation >= 90f)
		{
			this.ResetRotationLimit();
			return;
		}
		this.min = minRotation;
		this.max = maxRotation;
		BehaviourUpdateUtils.Register(this);
	}

	public void ResetRotationLimit()
	{
		this.min = -90f;
		this.max = 90f;
		base.transform.localEulerAngles = Vector3.zero;
		BehaviourUpdateUtils.Deregister(this);
	}

	public void ManagedLateUpdate()
	{
		float cameraPitch = this.cameraControl.GetCameraPitch();
		float num = 0f;
		if (cameraPitch < this.min)
		{
			num = this.min - cameraPitch;
		}
		else if (cameraPitch > this.max)
		{
			num = this.max - cameraPitch;
		}
		base.transform.localEulerAngles = new Vector3(-num, 0f, 0f);
	}

	private void OnDestroy()
	{
		BehaviourUpdateUtils.Deregister(this);
	}

	[SerializeField]
	[AssertNotNull]
	private MainCameraControl cameraControl;

	private float min;

	private float max;
}*/
