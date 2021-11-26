using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.TimeLordMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.CrewmateRoles.MayorMod
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.PerformKill))]
    public class PerformKill
    {
        public static bool Prefix(ActionButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return true;

            var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
            if (__instance != role.ButtonButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.ButtonUsed) return false;
            if (PlayerControl.LocalPlayer.RemainingEmergencies <= 0) return false;
            if (!__instance.enabled) return false;

            MayorEmergencyMeeting(role);
            
            return false;
        }

        public static void MayorEmergencyMeeting(Mayor role) {
            if (RecordRewind.rewinding) {
                return;
            }
            role.ButtonUsed = true;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.BarryButton, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.reporter = PlayerControl.LocalPlayer;
                MeetingRoomManager.Instance.target = null;
                AmongUsClient.Instance.DisconnectHandlers.AddUnique(
                    MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                if (ShipStatus.Instance.CheckTaskCompletion()) return;
                DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.RpcStartMeeting(null);
            }
            
        }
    }
}