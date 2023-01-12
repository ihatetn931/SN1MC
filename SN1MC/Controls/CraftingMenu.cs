using HarmonyLib;
//This fixes the issue when selecting fab icons so the blue quare box does not show up and you can just point and click
namespace SN1MC.Controls
{
	class CraftingMenu
	{
		[HarmonyPatch(typeof(uGUI_CraftingMenu), "uGUI_IIconManager.OnPointerClick")]
		public static class CraftingMenuOnPointerClick
		{
			public static bool Prefix(uGUI_ItemIcon icon, int button, ref bool __result, uGUI_CraftingMenu __instance)
			{
				bool flag = false;//!VRCustomOptionsMenu.EnableMotionControls;//GameInput.GetPrimaryDevice() == GameInput.Device.Controller;
				if (__instance.interactable)
				{
					uGUI_CraftingMenu.Node node = __instance.GetNode(icon);
					if (node != null)
					{
						if (flag)
						{
							switch (button)
							{
								case 0:
									if (node.action == TreeAction.Craft)
									{
										__instance.Action(node);
									}
									else
									{
										((uGUI_INavigableIconGrid)__instance).SelectItemInDirection(1, 0);
									}
									break;
								case 1:
									__instance.Out(node.parent as uGUI_CraftingMenu.Node);
									break;
								case 2:
									if (node.action == TreeAction.Craft)
									{
										TechType techType = node.techType;
										if (CrafterLogic.IsCraftRecipeUnlocked(techType))
										{
											PinManager.TogglePin(techType);
										}
									}
									break;
							}
						}
						else if (button != 0)
						{
							if (button == 1)
							{
								if (node.action == TreeAction.Craft)
								{
									TechType techType2 = node.techType;
									if (CrafterLogic.IsCraftRecipeUnlocked(techType2))
									{
										PinManager.TogglePin(techType2);
									}
								}
							}
						}
						else
						{
							__instance.Action(node);
						}
					}
					__result = true;
				}
				if (flag && button == 1)
				{
					__instance.Deselect();
					return true;
				}
				__result = false;


				return true;
			}

		}
	}
}