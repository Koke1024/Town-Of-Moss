using System;
using HarmonyLib;
using TheOtherRoles.Objects;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDCloseButtons
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;

            CustomButton.MeetingEndedUpdate();
        }
    }
}