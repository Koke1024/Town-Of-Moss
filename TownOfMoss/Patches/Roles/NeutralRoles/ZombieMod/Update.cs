using HarmonyLib;
using Hazel;
using Il2CppSystem;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ZombieMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    static class ZombieUpdate {

        public static void Postfix(PlayerControl __instance) {
            if (!__instance.AmOwner) {
                return;
            }
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Zombie)) return;
            Zombie zombie = Role.GetRole<Zombie>(PlayerControl.LocalPlayer);  
            if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                zombie.KilledBySeer = true;
                zombie.deadTime = DateTime.MaxValue;
                return;
            }
            if (!PlayerControl.LocalPlayer.Data.IsDead || zombie.KilledBySeer) {
                zombie.deadTime = DateTime.MaxValue;
                return;
            }
            if (zombie.deadTime != DateTime.MaxValue && (DateTime.UtcNow - zombie.deadTime).TotalSeconds >= CustomGameOptions.ZombieReviveTime) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.ZombieRevive, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                ReviveSelf.ReviveBody(PlayerControl.LocalPlayer);
                zombie.deadTime = DateTime.MaxValue;
            }
        }
    }
}