using HarmonyLib;
using Hazel;
using TownOfUs.NeutralRoles.ZombieMod;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ScavengerMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask) return true;

            foreach (var role in Role.AllRoles)
                if (role.RoleType == RoleEnum.Zombie)
                    ((Zombie) role).Loses();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.NeutralLose,
                SendOption.Reliable, -1);
            writer.Write((byte)RoleEnum.Scavenger);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            return true;
        }
    }
}