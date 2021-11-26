using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class DeadBodyClean
    {
        private static void Postfix(ExileController __instance)
        {
            foreach (var body in Object.FindObjectsOfType<DeadBody>()) {
                Object.Destroy(body.gameObject);
            }
        }
    }
}