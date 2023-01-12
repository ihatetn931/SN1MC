﻿using HarmonyLib;
using UnityEngine;


namespace SN1MC
{
    [HarmonyPatch(typeof(GameOptions), nameof(GameOptions.GetVrAnimationMode))]
    public static class VRAnimationsFixer
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            //Disables VRAnimations in order to fix cutscenes being skipped and seatruck controls not working
            __result = false;
        }
    }
}
