using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    public static class CanMove
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
        internal static class CanMovePatch
        {
            public static bool Prefix(PlayerControl __instance, ref bool __result)
            {
                __result = __instance.moveable
                           && !Minigame.Instance
                           && (!DestroyableSingleton<HudManager>.InstanceExists
                               || !DestroyableSingleton<HudManager>.Instance.Chat.IsOpen
                               && !DestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen
                               && !DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen)
                           /*&& (!ControllerManager.Instance || !ControllerManager.Instance.IsUiControllerActive)*/
                           && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped)
                           && !MeetingHud.Instance
                           && !CustomPlayerMenu.Instance
                           && !ExileController.Instance
                           && !IntroCutscene.Instance
                           && !MyBodyExists();

                return false;
            }
            public static bool MyBodyExists() {
                var body = Object.FindObjectsOfType<DeadBody>()
                    .FirstOrDefault(b => b.ParentId == PlayerControl.LocalPlayer.PlayerId);
                return body != null;
            }
        }
    }
}