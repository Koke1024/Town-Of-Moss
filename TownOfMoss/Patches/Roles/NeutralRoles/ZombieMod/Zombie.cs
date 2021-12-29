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
        
        public override bool DidWin(GameOverReason gameOverReason) {
            return CompleteZombieTasks && !Player.Data.IsDead;
        }

        public override void Outro(EndGameManager __instance) {
            base.Outro(__instance);
            NeutralOutro(__instance);
        }
    }
}