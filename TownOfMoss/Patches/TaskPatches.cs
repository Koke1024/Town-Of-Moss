using HarmonyLib;

namespace TownOfUs {
    internal static class TaskPatches {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class GameData_RecomputeTaskCounts {
            private static bool Prefix(GameData __instance) {
                if (LobbyBehaviour.Instance) {
                    return true;
                }
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                if (__instance.AllPlayers == null) {
                    return false;
                }
                for (var i = 0; i < __instance.AllPlayers.Count; i++) {
                    var playerInfo = __instance.AllPlayers.ToArray()[i];
                    if (playerInfo == null || playerInfo.Role == null || playerInfo.Object == null || playerInfo.Tasks == null) {
                        continue;
                    }

                    if (!playerInfo.Disconnected &&
                        (PlayerControl.GameOptions.GhostsDoTasks || !playerInfo.IsDead) &&
                        playerInfo.Role.TeamType != RoleTeamTypes.Impostor &&
                        !playerInfo._object.Is(RoleEnum.Assassin) &&
                        !playerInfo._object.Is(Faction.Neutral) &&
                        !playerInfo._object.Is(RoleEnum.Zombie)) {
                        for (var j = 0; j < playerInfo.Tasks.Count; j++) {
                            __instance.TotalTasks++;
                            if (playerInfo.Tasks.ToArray()[j].Complete) __instance.CompletedTasks++;
                        }
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class Console_CanUse {
            private static bool Prefix(Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo,
                ref float __result) {
                var playerControl = playerInfo.Object;

                var flag = (playerControl.Is(Faction.Neutral) && !playerControl.Is(RoleEnum.Phantom) && !playerControl.Is(RoleEnum.Zombie)) ||
                           playerControl.Is(RoleEnum.Assassin);

                // If the console is not a sabotage repair console
                if (flag && !__instance.AllowImpostor) {
                    __result = float.MaxValue;
                    return false;
                }

                return true;
            }
        }
    }
}