using HarmonyLib;

namespace TownOfUs.NeutralRoles.AssassinMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return;
            if (!CustomGameOptions.MadMateOn) return;

            // Assassin role = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
            __instance.KillButton.gameObject.SetActive(false);
            // __instance.KillButton.graphic.enabled = false;
        }
    }
}
