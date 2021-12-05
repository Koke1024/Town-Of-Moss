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
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class multiKillerCd2
    {
        public static void Postfix() {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
                return;
            }
        }
    }
    
    
    
    
    
}
