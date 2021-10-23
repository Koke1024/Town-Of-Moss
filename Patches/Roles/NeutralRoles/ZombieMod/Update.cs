using HarmonyLib;
using Hazel;
using Il2CppSystem;
using Il2CppSystem.Web.Util;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches.NeutralRoles.ZombieMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.ZombieMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    static class ZombieUpdate {
        public static void Postfix(PlayerControl __instance) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Zombie)) return;
            Zombie zombie = Role.GetRole<Zombie>(PlayerControl.LocalPlayer);
            if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                zombie.KilledBySeer = true;
                zombie.deadTime = null;
                return;
            }
            if (!PlayerControl.LocalPlayer.Data.IsDead || zombie.KilledBySeer) {
                zombie.deadTime = null;
                return;
            }

            if (zombie.Player.Data.IsDead && zombie.deadTime == null) {
                zombie.deadTime = DateTime.UtcNow;
                zombie.Player.NetTransform.Halt();
                zombie.Player.moveable = false;
            }

            if (zombie.deadTime != null && (DateTime.UtcNow - zombie.deadTime).Value.TotalMilliseconds >= CustomGameOptions.ZombieReviveTime * 1000.0f) {
                zombie.Player.moveable = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.ZombieRevive, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                ReviveSelf.ReviveBody(PlayerControl.LocalPlayer);
                zombie.deadTime = null;
            }
        }
    }
}