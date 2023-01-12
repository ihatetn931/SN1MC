

using System.Collections.Generic;
using UnityEngine;

//Targeting for tools that require you to aim like propulsion cannon
namespace SN1MC.Controls
{
    class CustomTargeting
    {

		// Token: 0x06003E15 RID: 15893 RVA: 0x0015AD79 File Offset: 0x00158F79
		public static void AddToIgnoreList(GameObject ignoreGameObject)
		{
			if (ignoreGameObject != null)
			{
				AddToIgnoreList(ignoreGameObject.transform);
			}
		}

		// Token: 0x06003E16 RID: 15894 RVA: 0x0015AD8F File Offset: 0x00158F8F
		public static void AddToIgnoreList(Transform ignoreTransform)
		{
			if (ignoreTransform != null && !ignoreList.Contains(ignoreTransform))
			{
				ignoreList.Add(ignoreTransform);
			}
		}

		// Token: 0x06003E17 RID: 15895 RVA: 0x0015ADB4 File Offset: 0x00158FB4
		public static bool GetRoot(GameObject candidate, out TechType techType, out GameObject gameObject)
		{
			techType = TechType.None;
			gameObject = null;
			if (candidate == null)
			{
				return false;
			}
			GameObject gameObject2;
			TechType techType2 = CraftData.GetTechType(candidate, out gameObject2);
			if (techType2 == TechType.None || gameObject2 == null)
			{
				Pickupable componentInParent = candidate.GetComponentInParent<Pickupable>();
				if (componentInParent != null)
				{
					techType2 = componentInParent.GetTechType();
					gameObject2 = componentInParent.gameObject;
				}
			}
			if (techType2 != TechType.None && gameObject2 != null)
			{
				techType = techType2;
				gameObject = gameObject2;
				return true;
			}
			return false;
		}

		// Token: 0x06003E18 RID: 15896 RVA: 0x0015AE1A File Offset: 0x0015901A
		public static bool GetTarget(GameObject ignoreObj, float maxDistance, out GameObject result, out float distance)
		{
			if (ignoreObj != null)
			{
				AddToIgnoreList(ignoreObj.transform);
			}
			return GetTarget(maxDistance, out result, out distance);
		}

		public static bool GetTargetLeft(GameObject ignoreObj, float maxDistance, out GameObject result, out float distance)
		{
			if (ignoreObj != null)
			{
				AddToIgnoreList(ignoreObj.transform);
			}
			return GetTargetLeft(maxDistance, out result, out distance);
		}

		// Token: 0x06003E19 RID: 15897 RVA: 0x0015AE38 File Offset: 0x00159038
		public static bool GetTarget(float maxDistance, out GameObject result, out float distance)
		{
			bool flag = false;
			Transform transform = VRHandsController.rightController.transform;
			Vector3 position = transform.position;
			Vector3 forward = transform.forward;
			Ray ray = new Ray(position, forward);
			int layerMask = ~(1 << LayerID.Trigger | 1 << LayerID.OnlyVehicle);
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide;
			int numHits = UWE.Utils.RaycastIntoSharedBuffer(ray, maxDistance, layerMask, queryTriggerInteraction);
			DebugTargetConsoleCommand.radius = -1f;
			RaycastHit raycastHit;
			if (Filter(UWE.Utils.sharedHitBuffer, numHits, out raycastHit))
			{
				flag = true;
			}
			if (!flag)
			{
				foreach (float num in GameInput.IsPrimaryDeviceGamepad() ? gamepadRadiuses : standardRadiuses)
				{
					DebugTargetConsoleCommand.radius = num;
					ray.origin = position + forward * num;
					numHits = UWE.Utils.SpherecastIntoSharedBuffer(ray, num, maxDistance, layerMask, queryTriggerInteraction);
					if (Filter(UWE.Utils.sharedHitBuffer, numHits, out raycastHit))
					{
						flag = true;
						break;
					}
				}
			}
			Reset();
			DebugTargetConsoleCommand.Stop();
			result = ((raycastHit.collider != null) ? raycastHit.collider.gameObject : null);
			distance = raycastHit.distance;
			return flag;
		}

		public static bool GetTargetLeft(float maxDistance, out GameObject result, out float distance)
		{
			bool flag = false;
			Transform transform = VRHandsController.leftController.transform;
			Vector3 position = transform.position;
			Vector3 forward = transform.forward;
			Ray ray = new Ray(position, forward);
			int layerMask = ~(1 << LayerID.Trigger | 1 << LayerID.Player | 1 << LayerID.OnlyVehicle);
			QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide;
			int numHits = UWE.Utils.RaycastIntoSharedBuffer(ray, maxDistance, layerMask, queryTriggerInteraction);
			DebugTargetConsoleCommand.radius = -1f;
			RaycastHit raycastHit;
			if (Filter(UWE.Utils.sharedHitBuffer, numHits, out raycastHit))
			{
				flag = true;
			}
			if (!flag)
			{
				foreach (float num in GameInput.IsPrimaryDeviceGamepad() ? gamepadRadiuses : standardRadiuses)
				{
					DebugTargetConsoleCommand.radius = num;
					ray.origin = position + forward * num;
					numHits = UWE.Utils.SpherecastIntoSharedBuffer(ray, num, maxDistance, layerMask, queryTriggerInteraction);
					if (Filter(UWE.Utils.sharedHitBuffer, numHits, out raycastHit))
					{
						flag = true;
						break;
					}
				}
			}
			Reset();
			DebugTargetConsoleCommand.Stop();
			result = ((raycastHit.collider != null) ? raycastHit.collider.gameObject : null);
			distance = raycastHit.distance;
			return flag;
		}

		// Token: 0x06003E1A RID: 15898 RVA: 0x0015AF60 File Offset: 0x00159160
		private static bool Filter(RaycastHit[] hits, int numHits, out RaycastHit resultHit)
		{
			resultHit = default(RaycastHit);
			for (int i = 0; i < numHits; i++)
			{
				RaycastHit raycastHit = hits[i];
				Collider collider = raycastHit.collider;
				if (!(collider == null))
				{
					GameObject gameObject = collider.gameObject;
					Transform transform = collider.transform;
					if (!(gameObject == null) && !(transform == null))
					{
						int layer = gameObject.layer;
						Transform x = null;
						for (int j = 0; j < ignoreList.Count; j++)
						{
							Transform transform2 = ignoreList[j];
							if (transform.IsAncestorOf(transform2))
							{
								x = transform2;
								break;
							}
						}
						if (x != null)
						{
							DebugTargetConsoleCommand.Log(DebugTargetConsoleCommand.Reason.AncestorOfIgnoredParent, raycastHit);
						}
						else
						{
							if (collider.isTrigger)
							{
								if (layer != LayerID.Useable)
								{
									DebugTargetConsoleCommand.Log(DebugTargetConsoleCommand.Reason.TriggerNotUseable, raycastHit);
									goto IL_F8;
								}
							}
							else if (layer == LayerID.NotUseable)
							{
								DebugTargetConsoleCommand.Log(DebugTargetConsoleCommand.Reason.ColliderNotUseable, raycastHit);
								goto IL_F8;
							}
							if (resultHit.collider == null || raycastHit.distance < resultHit.distance)
							{
								resultHit = raycastHit;
							}
						}
					}
				}
			IL_F8:;
			}
			if (resultHit.collider != null)
			{
				DebugTargetConsoleCommand.Log(DebugTargetConsoleCommand.Reason.Accept, resultHit);
				return true;
			}
			return false;
		}
		public static bool TraceFPSTargetPosition(GameObject ignoreObj, float maxDist, ref GameObject closestObj, ref Vector3 position, bool includeUseableTriggers = true)
		{
			Vector3 vector;
			return TraceFPSTargetPosition(ignoreObj, maxDist, ref closestObj, ref position, out vector, includeUseableTriggers);
		}
		private static int traceFPSLayerMask = -69206529;
		public static bool TraceFPSTargetPosition(GameObject ignoreObj, float maxDist, ref GameObject closestObj, ref Vector3 position, out Vector3 normal, bool includeUseableTriggers = true)
		{
			bool result = false;
			normal = Vector3.up;
			Camera camera = MainCamera.camera;
			Vector3 position2 = VRHandsController.rightController.transform.position;
			int num =  UWE.Utils.RaycastIntoSharedBuffer(new Ray(position2, VRHandsController.rightController.transform.forward), maxDist, traceFPSLayerMask, QueryTriggerInteraction.UseGlobal);
			if (num == 0)
			{
				num = UWE.Utils.SpherecastIntoSharedBuffer(position2, 0.7f, VRHandsController.rightController.transform.forward, maxDist, traceFPSLayerMask, QueryTriggerInteraction.UseGlobal);
			}
			closestObj = null;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
				if ((!(ignoreObj != null) || !Utils.IsAncestorOf(ignoreObj, raycastHit.collider.gameObject)) && (!raycastHit.collider || !raycastHit.collider.isTrigger || (includeUseableTriggers && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable"))) && (closestObj == null || raycastHit.distance < num2))
				{
					closestObj = raycastHit.collider.gameObject;
					num2 = raycastHit.distance;
					position = raycastHit.point;
					normal = raycastHit.normal;
					result = true;
				}
			}
			return result;
		}

		public static bool TraceFPSTargetPositionLeft(GameObject ignoreObj, float maxDist, ref GameObject closestObj, ref Vector3 position, out Vector3 normal, bool includeUseableTriggers = true)
		{
			bool result = false;
			normal = Vector3.up;
			Transform camera = VRHandsController.leftController.transform;
			Vector3 position2 = camera.position;
			int num = UWE.Utils.RaycastIntoSharedBuffer(new Ray(position2, camera.right), maxDist, traceFPSLayerMask, QueryTriggerInteraction.UseGlobal);
			if (num == 0)
			{
				num = UWE.Utils.SpherecastIntoSharedBuffer(position2, 0.7f, camera.right, maxDist, traceFPSLayerMask, QueryTriggerInteraction.UseGlobal);
			}
			closestObj = null;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
				if ((!(ignoreObj != null) || !Utils.IsAncestorOf(ignoreObj, raycastHit.collider.gameObject)) && (!raycastHit.collider || !raycastHit.collider.isTrigger || (includeUseableTriggers && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Useable"))) && (closestObj == null || raycastHit.distance < num2))
				{
					closestObj = raycastHit.collider.gameObject;
					num2 = raycastHit.distance;
					position = raycastHit.point;
					normal = raycastHit.normal;
					result = true;
				}
			}
			return result;
		}


		// Token: 0x06003E1B RID: 15899 RVA: 0x0015B08D File Offset: 0x0015928D
		private static void Reset()
		{
			ignoreList.Clear();
		}

		// Token: 0x04003881 RID: 14465
		private static readonly float[] standardRadiuses = new float[]
		{
		0.15f,
		0.3f
		};

		// Token: 0x04003882 RID: 14466
		private static readonly float[] gamepadRadiuses = new float[]
		{
		0.15f,
		0.5f
		};

		// Token: 0x04003883 RID: 14467
		private static List<Transform> ignoreList = new List<Transform>();

		// Token: 0x02000D21 RID: 3361
		// (Invoke) Token: 0x060068CD RID: 26829
		public delegate bool FilterRaycast(RaycastHit hit);
	}
}
