using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.DollMakerMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CoStartMeeting))]
    class StartMeetingPatch {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget) {
            if (__instance == null) {
                return;
            }
            foreach (var role in Role.GetRoles(RoleEnum.DollMaker)) {
                if (((DollMaker)role).DollList.Count <= 0) continue;
                foreach (var (key, _) in ((DollMaker)role).DollList) {
                    GameData.Instance.GetPlayerById(key)._object.MurderPlayer(GameData.Instance.GetPlayerById(key)._object);
                }
                ((DollMaker)role).DollList.Clear();
            }
        }
    }
}