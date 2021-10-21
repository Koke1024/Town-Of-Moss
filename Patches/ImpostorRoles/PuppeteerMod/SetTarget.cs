using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetTarget))]
    public class SetTarget
    {
        public static void Postfix(KillButtonManager __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Puppeteer);
            if (!flag) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var role = Role.GetRole<Puppeteer>(PlayerControl.LocalPlayer);
            if (role.PossessPlayer != null) {
                target = null;
            }
        }
    }
}