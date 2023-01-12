using UnityEngine;

namespace SN1MC.Controls.Tools
{
    class CannonCustom
    {
		public static GameObject _grabbedObjectLeft;
		public static GameObject lastValidTargetLeft;
		public static Vector3 targetPositionLeft = Vector3.zero;
		public static Vector3 grabbedObjectCenterLeft = Vector3.zero;

		/*public static bool canGrabRight
		{
			get
			{
				return lastValidTargetRight != null;
			}
		}*/

		public static bool canGrabLeft
		{
			get
			{
				return lastValidTargetLeft != null;
			}
		}

		/*public static GameObject grabbedObjectRight
		{
			get
			{
				PropulsionCannon cannon = GameObject.FindObjectOfType<PropulsionCannon>();
				return cannon._grabbedObject;
			}
			set
			{
				PropulsionCannon cannon = GameObject.FindObjectOfType<PropulsionCannon>();
				cannon._grabbedObject = value;
				if (cannon != null)
				{
					InventoryItem storedItem = cannon.storageSlot.storedItem;
					Pickupable x = (storedItem == null) ? null : storedItem.item;
					Pickupable pickupable = (cannon._grabbedObject == null) ? null : cannon._grabbedObject.GetComponent<Pickupable>();
					if (x != null)
					{
						if (pickupable != null)
						{
							if (x != pickupable)
							{
								cannon.storageSlot.RemoveItem();
								InventoryItem item = new InventoryItem(pickupable);
								cannon.storageSlot.AddItem(item);
							}
						}
						else
						{
							cannon.storageSlot.RemoveItem();
						}
					}
					else if (pickupable != null)
					{
						InventoryItem item2 = new InventoryItem(pickupable);
						cannon.storageSlot.AddItem(item2);
					}
					if (_grabbedObjectRight != null)
					{
						cannon.grabbingSound.Play();
						cannon.grabbedEffect.SetActive(true);
						cannon.fxBeam.SetActive(true);
						cannon.grabbedEffect.transform.parent = null;
						cannon.grabbedEffect.transform.position = cannon._grabbedObject.transform.position;
						cannon.UpdateTargetPosition();
						cannon.timeGrabbed = Time.time;
					}
					else
					{
						cannon.grabbingSound.Stop();
						cannon.grabbedEffect.SetActive(false);
						cannon.fxBeam.SetActive(false);
						cannon.grabbedEffect.transform.parent = cannon.transform;
					}
					if (MainGameController.Instance != null)
					{
						if (cannon._grabbedObject != null)
						{
							MainGameController.Instance.RegisterHighFixedTimestepBehavior(cannon);
							return;
						}
						MainGameController.Instance.DeregisterHighFixedTimestepBehavior(cannon);
					}
				}
			}
		}*/

		public static GameObject grabbedObjectLeft
		{
			get
			{
				return _grabbedObjectLeft;
			}
			set
			{
				_grabbedObjectLeft = value;
				PropulsionCannon cannon = GameObject.FindObjectOfType<PropulsionCannon>();
				if (cannon != null)
				{
					InventoryItem storedItem = cannon.storageSlot.storedItem;
					Pickupable x = (storedItem == null) ? null : storedItem.item;
					Pickupable pickupable = (_grabbedObjectLeft == null) ? null : _grabbedObjectLeft.GetComponent<Pickupable>();
					if (x != null)
					{
						if (pickupable != null)
						{
							if (x != pickupable)
							{
								cannon.storageSlot.RemoveItem();
								InventoryItem item = new InventoryItem(pickupable);
								cannon.storageSlot.AddItem(item);
							}
						}
						else
						{
							cannon.storageSlot.RemoveItem();
						}
					}
					else if (pickupable != null)
					{
						InventoryItem item2 = new InventoryItem(pickupable);
						cannon.storageSlot.AddItem(item2);
					}
					if (_grabbedObjectLeft != null)
					{
						cannon.grabbingSound.Play();
						cannon.grabbedEffect.SetActive(true);
						cannon.fxBeam.SetActive(true);
						cannon.grabbedEffect.transform.parent = null;
						cannon.grabbedEffect.transform.position = _grabbedObjectLeft.transform.position;
						UpdateTargetPositionLeft(cannon);
						cannon.timeGrabbed = Time.time;
					}
					else
					{
						cannon.grabbingSound.Stop();
						cannon.grabbedEffect.SetActive(false);
						cannon.fxBeam.SetActive(false);
						cannon.grabbedEffect.transform.parent = cannon.transform;
					}
					if (MainGameController.Instance != null)
					{
						if (_grabbedObjectLeft != null)
						{
							MainGameController.Instance.RegisterHighFixedTimestepBehavior(cannon);
							return;
						}
						MainGameController.Instance.DeregisterHighFixedTimestepBehavior(cannon);
					}
				}
			}
		}

		public static GameObject GetInteractableGrabbedObjectLeft()
		{
			if (!(grabbedObjectLeft != null) || (grabbedObjectLeft.transform.position - targetPositionLeft).magnitude >= 1.4f)
			{
				return null;
			}
			PickupableStorage componentInChildren = grabbedObjectLeft.GetComponentInChildren<PickupableStorage>();
			if (!(componentInChildren != null))
			{
				return grabbedObjectLeft;
			}
			return componentInChildren.gameObject;
		}

		public static GameObject TraceForGrabTargetRight(PropulsionCannon cannon)
		{
			Vector3 position = VRHandsController.rightController.transform.position;
			int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
			int num = UWE.Utils.SpherecastIntoSharedBuffer(position, 1.2f, VRHandsController.rightController.transform.forward, cannon.pickupDistance, layerMask, QueryTriggerInteraction.UseGlobal);
			GameObject result = null;
			float num2 = float.PositiveInfinity;
			PropulsionCannon.checkedObjects.Clear();
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
				if (!raycastHit.collider.isTrigger || raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable"))
				{
					GameObject entityRoot = UWE.Utils.GetEntityRoot(raycastHit.collider.gameObject);
					if (entityRoot != null && !PropulsionCannon.checkedObjects.Contains(entityRoot))
					{
						if (!cannon.launchedObjects.Contains(entityRoot))
						{
							float sqrMagnitude = (raycastHit.point - position).sqrMagnitude;
							if (sqrMagnitude < num2 && ValidateNewObjectRight(entityRoot, raycastHit.point, cannon, true))
							{
								result = entityRoot;
								num2 = sqrMagnitude;
							}
						}
						PropulsionCannon.checkedObjects.Add(entityRoot);
					}
				}
			}
			return result;
		}

		public static GameObject TraceForGrabTargetLeft(PropulsionCannon cannon)
		{
			Vector3 position = VRHandsController.leftController.transform.position;
			int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
			int num = UWE.Utils.SpherecastIntoSharedBuffer(position, 1.2f, VRHandsController.leftController.transform.forward, cannon.pickupDistance, layerMask, QueryTriggerInteraction.UseGlobal);
			GameObject result = null;
			float num2 = float.PositiveInfinity;
			PropulsionCannon.checkedObjects.Clear();
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
				if (!raycastHit.collider.isTrigger || raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable"))
				{
					GameObject entityRoot = UWE.Utils.GetEntityRoot(raycastHit.collider.gameObject);
					if (entityRoot != null && !PropulsionCannon.checkedObjects.Contains(entityRoot))
					{
						if (!cannon.launchedObjects.Contains(entityRoot))
						{
							float sqrMagnitude = (raycastHit.point - position).sqrMagnitude;
							if (sqrMagnitude < num2 && ValidateNewObjectLeft(entityRoot, raycastHit.point, cannon, true))
							{
								result = entityRoot;
								num2 = sqrMagnitude;
							}
						}
						PropulsionCannon.checkedObjects.Add(entityRoot);
					}
				}
			}
			return result;
		}

		public static bool ValidateNewObjectRight(GameObject go, Vector3 hitPos, PropulsionCannon cannon, bool checkLineOfSight = true)
		{
			if (go.GetComponent<PropulseCannonAmmoHandler>() != null)
			{
				return false;
			}
			if (!cannon.ValidateObject(go))
			{
				return false;
			}
			if (checkLineOfSight && !cannon.CheckLineOfSight(go, VRHandsController.rightController.transform.position, hitPos))
			{
				return false;
			}
			if (go.GetComponent<Pickupable>() != null)
			{
				return true;
			}
			Bounds aabb = cannon.GetAABB(go);
			return aabb.size.x * aabb.size.y * aabb.size.z <= cannon.maxAABBVolume;
		}

		public static bool ValidateNewObjectLeft(GameObject go, Vector3 hitPos, PropulsionCannon cannon, bool checkLineOfSight = true)
		{
			if (go.GetComponent<PropulseCannonAmmoHandler>() != null)
			{
				return false;
			}
			if (!cannon.ValidateObject(go))
			{
				return false;
			}
			if (checkLineOfSight && !cannon.CheckLineOfSight(go, VRHandsController.leftController.transform.position, hitPos))
			{
				return false;
			}
			if (go.GetComponent<Pickupable>() != null)
			{
				return true;
			}
			Bounds aabb = cannon.GetAABB(go);
			return aabb.size.x * aabb.size.y * aabb.size.z <= cannon.maxAABBVolume;
		}

		public static void GrabObjectLeft(GameObject target, PropulsionCannon cannon)
		{
			grabbedObjectLeft = target;
			GameObject fxTrailPrefab = new GameObject();
			UWE.Utils.SetIsKinematicAndUpdateInterpolation(grabbedObjectLeft.GetComponent<Rigidbody>(), false, false);
			PropulseCannonAmmoHandler propulseCannonAmmoHandler = target.GetComponent<PropulseCannonAmmoHandler>();
			if (propulseCannonAmmoHandler == null)
			{
				fxTrailPrefab = cannon.fxTrailPrefab;
				propulseCannonAmmoHandler = target.AddComponent<PropulseCannonAmmoHandler>();
				propulseCannonAmmoHandler.fxTrailPrefab = fxTrailPrefab;
			}
			propulseCannonAmmoHandler.ResetHandler(false, false);
			propulseCannonAmmoHandler.SetCannon(cannon);
		}

		public static void ReleaseGrabbedObjectLeft(PropulsionCannon cannon)
		{
			if (grabbedObjectLeft != null)
			{
				PropulseCannonAmmoHandler component = grabbedObjectLeft.GetComponent<PropulseCannonAmmoHandler>();
				if (component != null && component.IsGrabbedBy(cannon))
				{
					component.UndoChanges();
					UnityEngine.Object.Destroy(component);
				}
				grabbedObjectLeft = null;
			}
		}

		public static void UpdateTargetPositionLeft(PropulsionCannon cannon)
		{
			targetPositionLeft = GetObjectPositionLeft(grabbedObjectLeft,cannon);
		}

		public static bool IsGrabbingObjectLeft()
		{
			return grabbedObjectLeft != null;
		}

		public static Vector3 GetObjectPositionLeft(GameObject go, PropulsionCannon cannon)
		{
			GameObject leftController = VRHandsController.leftController;
			Vector3 b = Vector3.zero;
			float num = 0f;
			if (go != null)
			{
				Bounds aabb = cannon.GetAABB(go);
				b = go.transform.position - aabb.center;
				Ray ray = new Ray(aabb.center, leftController.transform.forward);
				float f = 0f;
				if (aabb.IntersectRay(ray, out f))
				{
					num = Mathf.Abs(f);
				}
				grabbedObjectCenterLeft = aabb.center;
			}
			Vector3 position = Vector3.forward * (2.5f + num) + cannon.localObjectOffset;
			return leftController.transform.TransformPoint(position) + b;
		}
	}
}
