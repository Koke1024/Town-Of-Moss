using Hazel;
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

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            if (!Player.Data.IsDead) {
                return;
            }
            
            if (KilledBySeer) {
                deadTime = DateTime.MaxValue;
                return;
            }
            if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                KilledBySeer = true;
                deadTime = DateTime.MaxValue;
                return;
            }
            
            if (deadTime != DateTime.MaxValue && deadTime.AddSeconds(CustomGameOptions.ZombieReviveTime) < DateTime.UtcNow) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.ZombieRevive, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                ReviveSelf.ReviveBody(PlayerControl.LocalPlayer);
                deadTime = DateTime.MaxValue;
            }
        }
    }
}