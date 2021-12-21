using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class DeadBodyClean
    {
        private static void Postfix(ExileController __instance)
        {
            foreach (var body in Utils.KilledPlayers) {
                if (body.Value.Body != null) {
                    Object.Destroy(body.Value.Body.gameObject);                    
                }
            }

            Utils.KilledPlayers.Clear();
        }
    }
}