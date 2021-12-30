using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        private static KillButton killButton;

        public static void Postfix(HudManager __instance)
        {
            UpdateKillButton(__instance);
        }

        private static void UpdateKillButton(HudManager __instance)
        {
            killButton = __instance.KillButton;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag7 = PlayerControl.AllPlayerControls.Count > 1;
            if (!flag7) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
            {
                var isImpostor = PlayerControl.LocalPlayer.Data.IsImpostor();
                if (!isImpostor) return;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Assassin) && CustomGameOptions.MadMateOn) {
                    return;
                }
                var isDead2 = PlayerControl.LocalPlayer.Data.IsDead;
                if (isDead2)
                {
                    killButton.gameObject.SetActive(false);
                    // killButton.graphic.enabled = false;
                }
                else
                {
                    __instance.KillButton.gameObject.SetActive(!MeetingHud.Instance);
                 //   __instance.KillButton.isActive = !MeetingHud.Instance;
                }
            }
        }
    }
}
