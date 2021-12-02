using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
    public class PlayerTabPatch
    {
        public static void Postfix(PlayerTab __instance) {
            int i = 0;
            foreach(var chip in __instance.ColorChips){
                chip.transform.localScale *= 0.8f;
                var x = __instance.XRange.Lerp((i % 4) / 4f) + 0.25f;
                var y = __instance.YStart - (i / 4) * 0.55f;
                chip.transform.localPosition = new Vector3(x, y, -1f);
                ++i;
            }

        }
    }
}
