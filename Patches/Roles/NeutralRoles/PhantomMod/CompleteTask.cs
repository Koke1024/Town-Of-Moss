using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            var role = Role.GetRole<Phantom>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletePhantomTasks = true;
                if (AmongUsClient.Instance.AmHost)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.NeutralWin, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write((byte)RoleEnum.Phantom);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
            }
        }
    }
}