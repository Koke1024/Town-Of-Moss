using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMeetingRoom), typeof(PlayerControl))]
    public class DeadBodyClean
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var body in Object.FindObjectsOfType<DeadBody>()) {
                Object.Destroy(body.gameObject);
            }
        }
    }
}