using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Jester : Morphling
    {
        public bool VotedOut;

        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => "Get voted out";
            TaskText = () => "Get voted out!\nFake Tasks:";
            Color = new Color(1f, 0.75f, 0.8f, 1f);
            RoleType = RoleEnum.Jester;
            Faction = Faction.Neutral;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            var jesterTeam = new List<PlayerControl>();
            jesterTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = jesterTeam;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }

        public void Loses()
        {
            Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }

        public new bool DidWin(GameOverReason gameOverReason) {
            AmongUsExtensions.Log($"Jester did win {VotedOut}");
            return VotedOut;
        }
    }
}