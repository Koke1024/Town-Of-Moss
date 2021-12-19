using System;
using System.Collections;
using Hazel;
using TownOfUs.NeutralRoles.ZombieMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.SeerMod
{
    public class SeerCoroutine
    {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator Investigating(PlayerControl target, Seer seer) {
            seer.Player.moveable = false;
            seer.Player.NetTransform.Halt();
            seer.LastInvestigated = DateTime.UtcNow.AddSeconds(-CustomGameOptions.SeerCd + CustomGameOptions.SeerInvestigateTime);
            var start = DateTime.UtcNow;
            while (true) {
                var distBetweenPlayers = Utils.getDistBetweenPlayers(PlayerControl.LocalPlayer, target);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[0];
                if (!flag3) {
                    seer.LastInvestigated = DateTime.UtcNow.AddSeconds(-CustomGameOptions.SeerInvestigateTime / 2.0f);
                    seer.Player.moveable = true;
                    seer.LastInvestigated = DateTime.UtcNow;
                    yield break;
                }
                yield return null;
                
                if ((DateTime.UtcNow - start).TotalMilliseconds >= CustomGameOptions.SeerInvestigateTime * 1000.0f) {
                    break;
                }
            }
            
            seer.Player.moveable = true;
            seer.Investigated.Add(seer.ClosestPlayer.PlayerId);
            seer.LastInvestigated = DateTime.UtcNow;
            
            var playerId = target.PlayerId;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Investigate, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            if (target.Is(RoleEnum.Zombie) && CustomGameOptions.ZombieKilledBySeer) {
                Utils.RpcMurderPlayer(target, target);
                Role.GetRole<Zombie>(target).KilledBySeer = true;
            }
        }
    }
}