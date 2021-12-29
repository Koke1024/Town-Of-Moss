using HarmonyLib;
using Il2CppSystem;
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

            if (mk.FirstKillTime == null) {
                return;
            }

            if (mk.FirstKillTime.Value.AddSeconds(CustomGameOptions.MultiKillEnableTime) < DateTime.UtcNow) {
                mk.KilledOnce = false;
                mk.FirstKillTime = null;
                mk.Player.SetKillTimer(mk.MaxTimer);
            }
        }
    }
}