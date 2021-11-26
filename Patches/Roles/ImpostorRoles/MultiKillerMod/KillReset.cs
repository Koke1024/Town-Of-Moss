using HarmonyLib;
using Il2CppSystem;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.MultiKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class CheckMultiKill
    {
        public static void Postfix(HudManager __instance) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
                return;
            }
            
            MultiKiller mk = Role.GetRole<MultiKiller>(PlayerControl.LocalPlayer);

            if (mk.firstKillTime == null) {
                return;
            }

            if ((System.DateTime.UtcNow - mk.firstKillTime).Value.TotalMilliseconds >
                CustomGameOptions.MultiKillEnableTime * 1000.0f) {
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