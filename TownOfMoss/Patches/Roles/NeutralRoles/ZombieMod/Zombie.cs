using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ZombieMod
{
    public class Zombie : Role
    {
        public float dieDelay;
        public bool CompleteZombieTasks;
        public DateTime deadTime = DateTime.MaxValue;
        public bool KilledBySeer = false;

        public Zombie(PlayerControl player) : base(player)
        {
            Name = "Zombie";
            ImpostorText = () => "You won't be killed!";
            TaskText = () => "You will revive after killed";
            Color = new Color(0.47f, 0.22f, 0f);
            RoleType = RoleEnum.Zombie;
            Faction = Faction.Neutral;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            var sniperTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            sniperTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = sniperTeam;
        }

        public void Wins() {
            CompleteZombieTasks = true;
        }

        public void Loses()
        {
            Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }
    }

    // [HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.DidWin), typeof(GameOverReason))]
    // public static class WinPatch {
    //     public static bool Prefix(RoleBehaviour __instance, out bool __result) {
    //         if (!__instance.Player.Is(RoleEnum.Zombie)) {
    //             __result = true;
    //             return true;
    //         }
    //
    //         Zombie role = Role.GetRole<Zombie>(__instance.Player);
    //         __result = role.CompleteZombieTasks;
    //         return false;
    //     }
    // }
}