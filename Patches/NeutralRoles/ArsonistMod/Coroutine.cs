using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using Reactor;
using Reactor.Extensions;
using SteamKit2.GC.Dota.Internal;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    public class ArsonistCoroutine
    {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator Dousing(PlayerControl target, Arsonist arso)
        {
            var start = DateTime.UtcNow;
            arso.LastDoused = DateTime.UtcNow.AddSeconds(-CustomGameOptions.DouseCd + CustomGameOptions.ArsonistDouseTime);
            while (true) {
                var distBetweenPlayers = Utils.getDistBetweenPlayers(PlayerControl.LocalPlayer, target);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                if (!flag3) {
                    arso.LastDoused = DateTime.UtcNow;
                    yield break;
                }
                yield return new WaitForSeconds(0.016f);
                
                if ((DateTime.UtcNow - start).TotalMilliseconds >= CustomGameOptions.ArsonistDouseTime * 1000.0f) {
                    // arso.LastDoused = DateTime.UtcNow.AddSeconds(-CustomGameOptions.DouseCd);
                    break;
                }
            }

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Douse, SendOption.Reliable, -1);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            arso.DousedPlayers.Add(target.PlayerId);
            arso.LastDoused = DateTime.UtcNow;
            
            SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
            yield break;
        }

        public static IEnumerator Ignite(Arsonist role) {
            Coroutines.Start(Utils.FlashCoroutine(new Color(1.0f, 0, 0, 1.0f), 3.0f));
            yield return new WaitForSeconds(3.0f);
            foreach (var playerId in role.DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                if (
                    player == null ||
                    player.Data.Disconnected ||
                    player.Data.IsDead
                ) continue;
                Utils.MurderPlayer(player, player);
            }

            Utils.MurderPlayer(role.Player, role.Player);


            role.IgniteUsed = true;
        }
    }
}