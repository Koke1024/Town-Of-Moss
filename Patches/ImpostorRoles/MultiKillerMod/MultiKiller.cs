using System;
using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class MultiKiller : Assassin {
        public bool killedOnce = false;
        public System.DateTime? firstKillTime = null;
        public bool isFirstTime = true;
        public MultiKiller(PlayerControl player) : base(player)
        {
            Name = "MultiKiller";
            ImpostorText = () => "kill crews at once";
            TaskText = () => "you can kill continuous";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.MultiKiller;
            Faction = Faction.Impostors;
            
            killedOnce = false;
            firstKillTime = System.DateTime.UtcNow.AddSeconds(5.0f);
        }

        public float MaxTimer() => PlayerControl.GameOptions.KillCooldown * CustomGameOptions.MultiKillerCdRate / 100.0f;
    }
    
    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.CoShowIntro))]
    public class multiKillerCd
    {
        // public static void Postfix() {
        //     if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
        //         return;
        //     }
        //     AmongUsExtensions.Log($"CoShowIntro");
        //     PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 2.0f - 10.0f);
        // }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
    public class multiKillerCd2
    {
        public static void Postfix() {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
                return;
            }

            MultiKiller mk = Role.GetRole<MultiKiller>(PlayerControl.LocalPlayer);
            mk.Player.killTimer = mk.MaxTimer() - 10.0f;
            DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(mk.Player.killTimer, mk.MaxTimer());
        }
    }
    
    
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetCoolDown))]
    public class SetCoolDownBefore
    {
        public static bool Prefix(KillButtonManager __instance, [HarmonyArgument(0)]float timer, [HarmonyArgument(1)]float maxTimer) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
                return true;
            }
            // if ((int)timer != 0 && (int)timer < PlayerControl.GameOptions.KillCooldown) {
            //     return false;
            // }

            return true;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public class SetCoolDownBefore2
    {
        public static bool Prefix(KillButtonManager __instance, [HarmonyArgument(0)]float time) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
                return true;
            }

            return true;
        }
    }
    
    
    
    
    
}
