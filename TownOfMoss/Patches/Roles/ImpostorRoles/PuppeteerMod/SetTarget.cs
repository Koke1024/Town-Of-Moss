using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class SetTarget
    {
        public static void Postfix(KillButton __instance, [HarmonyArgument(0)] PlayerControl target)
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
    
    [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
    public class CantUsePlatformOnPossess
    {
        public static bool Prefix(PlatformConsole __instance)
        {
            var playerControl = PlayerControl.LocalPlayer;
            if (playerControl.Is(RoleEnum.Puppeteer) && Role.GetRole<Puppeteer>(playerControl).possessStarting) {
                return false;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(PlatformConsole), nameof(Ladder.Use))]
    public class CantUseOnPossess
    {
        public static bool Prefix(PlatformConsole __instance)
        {
            var playerControl = PlayerControl.LocalPlayer;
            if (playerControl.Is(RoleEnum.Puppeteer) && Role.GetRole<Puppeteer>(playerControl).possessStarting) {
                return false;
            }
            return true;
        }
    }
}


