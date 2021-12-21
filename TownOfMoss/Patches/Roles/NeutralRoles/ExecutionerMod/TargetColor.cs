using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    // public enum OnTargetDead
    // {
    //     Jester,
    //     Crew
    // }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class TargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Executioner role)
        {
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.color = Color.black;
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) return;
            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (role.target == null)
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.ExecutionerToJester, SendOption.Reliable, -1);
                writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                ExeToJes(PlayerControl.LocalPlayer);
                return;
            }

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            role.target.nameText.color = Color.black;

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!role.target.Data.IsDead && !role.target.Data.Disconnected) return;
            if (role.TargetVotedOut) return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.ExecutionerToJester, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            ExeToJes(PlayerControl.LocalPlayer);
        }

        public static void ExeToJes(PlayerControl player)
        {
            if (player == null) {
                return;
            }

            if (player.myTasks != null && player.myTasks.Count > 0) {
                player.myTasks.RemoveAt(0);                
            }
            if (PlayerControl.LocalPlayer == player) {
                if (Role.GetRole<Executioner>(player) != null && Role.GetRole<Executioner>(player).Arrow != null&&
                    Role.GetRole<Executioner>(player).Arrow.gameObject != null) {
                    Role.GetRole<Executioner>(player).Arrow.gameObject.Destroy();                    
                }
            }
            Role.RoleDictionary.Remove(player.PlayerId);

            var jester = new Jester(player);
            var task = new GameObject("JesterTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text =
                $"{jester.ColorString}Role: {jester.Name}\nYour target was killed. Now you get voted out!\nFake Tasks:[]";
            player.myTasks.Insert(0, task);
        }
    }
}