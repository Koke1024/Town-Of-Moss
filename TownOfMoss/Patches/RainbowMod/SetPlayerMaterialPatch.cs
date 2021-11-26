﻿using HarmonyLib;
using UnityEngine;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetPlayerMaterialColors), typeof(int), typeof(Renderer))]
    public class SetPlayerMaterialPatch
    {
        public static bool Prefix([HarmonyArgument(0)] int colorId, [HarmonyArgument(1)] Renderer rend)
        {
            var r = rend.gameObject.GetComponent<RainbowBehaviour>();
            if (r == null)
            {
                r = rend.gameObject.AddComponent<RainbowBehaviour>();
            }

            r.AddRend(rend, colorId);
            return !RainbowUtils.IsRainbow(colorId);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetPlayerMaterialColors), typeof(Color), typeof(Renderer))]
    public class SetPlayerMaterialPatch2
    {
        public static bool Prefix([HarmonyArgument(1)] Renderer rend)
        {
            var r = rend.gameObject.GetComponent<RainbowBehaviour>();
            if (r == null)
            {
                r = rend.gameObject.AddComponent<RainbowBehaviour>();
            }

            r.AddRend(rend, 0);
            return true;
        }
    }
}
