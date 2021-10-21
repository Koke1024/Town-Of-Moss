using System.Collections.Generic;
using Il2CppSystem;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Zombie : Role
    {
        public float dieDelay;
        public bool CompleteZombieTasks;
        public DateTime? deadTime = null;
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

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
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
            Player.Data.IsImpostor = true;
        }
    }
}