using HarmonyLib;
using TownOfUs;

namespace TheOtherRoles.Patches {
    public class GameStartManagerPatch {
        private static string lobbyCodeText = "";
        
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class GameStartManagerStartPatch {
            public static void Postfix(GameStartManager __instance) {
                lobbyCodeText = __instance.GameRoomName.text;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch {
            public static void Prefix(GameStartManager __instance) {
            }

            public static void Postfix(GameStartManager __instance) {
                if (Utils.IsStreamMode) {
                    __instance.GameRoomName.text = "******";
                }
                else {
                    __instance.GameRoomName.text = lobbyCodeText;
                }
            }
        }
    }
}