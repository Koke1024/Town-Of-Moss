using HarmonyLib;

namespace TownOfUs.NeutralRoles.AssassinMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive))]
    public static class HudManagerSetHudActive
    {
        
        public static void Postfix(HudManager __instance, [HarmonyArgument(0)]bool isActive)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return;
            if (!CustomGameOptions.MadMateOn) return;

            __instance.KillButton.gameObject.SetActive(isActive);
            __instance.SabotageButton.gameObject.SetActive(isActive);
        }
    }

    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public static class SabotageClickPatch {
        private static bool amMad;
        public static void Prefix(SabotageButton __instance) {
            amMad = false;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return;
            if (!CustomGameOptions.MadMateOn) return;

            amMad = true;
            PlayerControl.LocalPlayer.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }

        public static void Postfix(SabotageButton __instance) {
            if (amMad) {
                PlayerControl.LocalPlayer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            }
        }
    }
}
