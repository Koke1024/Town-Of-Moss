using System.Linq;
using System.Text;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TownOfUs.NeutralRoles.ScavengerMod;
using TownOfUs.NeutralRoles.SniperMod;
using TownOfUs.NeutralRoles.ZombieMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs {
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsClient_OnGameEnd {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult) {
            Utils.potentialWinners.Clear();

            foreach (var row in PlayerControl.AllPlayerControls) {
                if (row.Is(RoleEnum.Assassin)) {
                    row.Data.Role.TeamType = RoleTeamTypes.Impostor;
                }
            }

            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.potentialWinners.Add(new WinningPlayerData(player.Data));


            if (endGameResult.GameOverReason == GameOverReason.HumansByTask) {
                foreach (var role in Role.GetRoles(RoleEnum.Zombie)) {
                    var zombie = (Zombie)role;
                    zombie.CompleteZombieTasks = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class EndGameManager_SetEverythingUp {
        public static void Prefix() {
            var jester = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Jester && ((Jester)x).VotedOut);
            if (Role.NobodyWins) {
                TempData.winners = new List<WinningPlayerData>();
                return;
            }
            
            if (jester != null){
                var winners = Utils.potentialWinners.Where(x => x.PlayerName == jester.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners)
                {
                    win.IsDead = false;
                    TempData.winners.Add(win);
                }
                return;
            }

            var executioner = Role.AllRoles.FirstOrDefault(x =>
                x.RoleType == RoleEnum.Executioner && ((Executioner)x).TargetVotedOut);

            var toRemoveColorIds = Role.AllRoles.Where(o => o.LostByRPC).Select(o => o.Player.Data.DefaultOutfit.ColorId).ToArray();
            var toRemoveWinners = TempData.winners.ToArray().Where(o => toRemoveColorIds.Contains(o.ColorId)).ToArray();
            for (int i = 0; i < toRemoveWinners.Count(); i++)
            {
                TempData.winners.Remove(toRemoveWinners[i]);
            }
            
            var zombie = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Zombie && !x.Player.Data.IsDead && ((Zombie)x).CompleteZombieTasks);
            if (zombie != null) {
                var winners = Utils.potentialWinners.Where(x => x._playerName == zombie.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) {
                    win.IsDead = false;
                    TempData.winners.Add(win);
                }
                return;
            }
            
            var scavenger = Role.AllRoles.FirstOrDefault(x =>
                x.RoleType == RoleEnum.Scavenger && ((Scavenger) x).eatCount >= CustomGameOptions.ScavengerWinCount);
            if (scavenger != null) {
                var winners = Utils.potentialWinners.Where(x => x._playerName == scavenger.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) {
                    win.IsDead = false;
                    TempData.winners.Add(win);
                }
                return;
            }

            var lover = Role.AllRoles
                .Where(x => x.RoleType == RoleEnum.Lover || x.RoleType == RoleEnum.LoverImpostor)
                .FirstOrDefault(x => ((Lover)x).LoveCoupleWins);
            if (lover != null) {
                var lover1 = (Lover)lover;
                var lover2 = lover1.OtherLover;
                var winners = Utils.potentialWinners
                    .Where(x => x.PlayerName == lover1.PlayerName || x.PlayerName == lover2.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) TempData.winners.Add(win);
                return;
            }

            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch && ((Glitch)x).GlitchWins);
            if (glitch != null) {
                var winners = Utils.potentialWinners.Where(x => x._playerName == glitch.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) TempData.winners.Add(win);
                return;
            }

            var sniper = Role.AllRoles.FirstOrDefault(x =>
                x.RoleType == RoleEnum.Sniper && ((Sniper)x).KilledCount >= CustomGameOptions.SniperWinCnt);
            if (sniper != null) {
                var winners = Utils.potentialWinners.Where(x => x._playerName == sniper.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) TempData.winners.Add(win);
                return;
            }

            var arsonist =
                Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist && ((Arsonist)x).ArsonistWins);
            if (arsonist != null) {
                var winners = Utils.potentialWinners.Where(x => x._playerName == arsonist.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                winners.First().IsDead = false;
                foreach (var win in winners) TempData.winners.Add(win);
                return;
            }

            var phantom =
                Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Phantom && ((Phantom)x).CompletePhantomTasks);
            if (phantom != null) {
                var winners = Utils.potentialWinners.Where(x => x._playerName == phantom.PlayerName).ToList();
                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) TempData.winners.Add(win);
                return;
            }

            if (CustomGameOptions.MadMateOn) {
                if (GameData.Instance.TotalTasks > GameData.Instance.CompletedTasks && Role.GetImpRoles()
                    .Any(x => !x.Player.Data.IsDead && !x.Player.Is(RoleEnum.Assassin))) {
                    // AmongUsExtensions.Log($"Impostor Win");
                    var winners = Utils.potentialWinners.Where(x => x.IsImpostor).ToList();
                    TempData.winners = new List<WinningPlayerData>();
                    foreach (var win in winners) {
                        TempData.winners.Add(win);
                        // AmongUsExtensions.Log($"{win.Name}");
                    }
                }
                else {
                    // AmongUsExtensions.Log($"Crew Win");
                    var winners = Utils.potentialWinners.Where(x => !x.IsImpostor).ToList();
                    TempData.winners = new List<WinningPlayerData>();
                    foreach (var win in winners) {
                        TempData.winners.Add(win);
                        // AmongUsExtensions.Log($"{win.Name}");
                    }
                }
            }
        }
    }


    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class SetRoleString {
        public static void Postfix(EndGameManager __instance) {
            if (Camera.main is not null) {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f,
                    position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                roleSummaryText.AppendLine("Players and roles at the end of the game:");

                TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
                roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1.5f;
                roleSummaryTextMesh.fontSizeMax = 1.5f;
                roleSummaryTextMesh.fontSize = 1.5f;
                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = Utils.roleString;
            }
        }
    }
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class CreateRoleString {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult) {
            var roleSummaryText = new StringBuilder();
            roleSummaryText.AppendLine("Players and roles at the end of the game:");
            foreach (var player in PlayerControl.AllPlayerControls) {
                if (Role.GetRole(player) == null) {
                    continue;
                }
                var roles = "<color=#"+Role.GetRole(player).Color.ToHtmlStringRGBA()+">" + 
                    Role.GetRole(player).Name + "</color>";
                var taskInfo = "";
                if (!player.Is(RoleEnum.Assassin) && (player.Is(Faction.Crewmates) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Zombie))) {
                    taskInfo = $" - <color=#FAD934FF>({player.Data.Tasks.ToArray().Count(x => x.Complete)}/{player.Data.Tasks.ToArray().Count()})</color>";
                }
                if (player.Is(RoleEnum.Arsonist)) {
                    var livingPlayer = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && x != player).ToList();
                    taskInfo = $" - <color=#{Role.GetRole(player).Color.ToHtmlStringRGBA()}>({livingPlayer.Count(x => Role.GetRole<Arsonist>(player).DousedPlayers.Contains(x.PlayerId))}/{livingPlayer.Count})</color>";
                    taskInfo += $" - <color=#{Role.GetRole(player).Color.ToHtmlStringRGBA()}>({Role.GetRole<Arsonist>(player).DousedPlayers.Count}/{PlayerControl.AllPlayerControls.Count - 1})</color>";
                }
                roleSummaryText.AppendLine($"{player.Data.PlayerName} - {roles}{taskInfo}");
            }
            Utils.roleString = roleSummaryText.ToString();
        }
    }

    // [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    // public static class OnGameEndPatch {
    //     public static void Prefix(AmongUsClient __instance) {
    //         AmongUsExtensions.Log($"pre OnGameEnd");
    //     }
    //
    //     public static void Postfix(AmongUsClient __instance) {
    //         AmongUsExtensions.Log($"post OnGameEnd");
    //     }
    // }
}