using HarmonyLib;
using Il2CppSystem;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.MultiKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class CheckMultiKill
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.MultiKiller))
            {
                MultiKiller mk = (MultiKiller) role;
                if (mk.firstKillTime == null) {
                    continue;
                }
                if (mk.firstKillTime != null && 
                    (System.DateTime.UtcNow - mk.firstKillTime).Value.TotalMilliseconds > CustomGameOptions.MultiKillEnableTime * 1000.0f) {
                    mk.killedOnce = false;
                    mk.firstKillTime = null;
                    if (mk.isFirstTime) {
                        mk.Player.SetKillTimer(mk.MaxTimer() - 15);
                        mk.isFirstTime = false;
                    }
                    else {
                        mk.Player.SetKillTimer(mk.MaxTimer());
                    }
                }
            }
        }
    }
}