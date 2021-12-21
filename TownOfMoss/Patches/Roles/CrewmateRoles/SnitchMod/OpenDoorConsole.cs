using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    #region DoorConsole
    [HarmonyPatch(typeof(DoorConsole), nameof(DoorConsole.Use))]
    public static class DoorConsoleUsePatch
    {
        public static void Postfix(DoorConsole __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) {
                return;
            }

            var snitch = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);

            if (CustomGameOptions.SnitchOpenDoorImmediately == OpenDoorImmediate.None || (CustomGameOptions.SnitchOpenDoorImmediately == OpenDoorImmediate.OneTask && !snitch.OneTaskLeft)) {
                return;
            }
            __instance.MyDoor.SetDoorway(true);
            Minigame.Instance.ForceClose();
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.OpenDoor, SendOption.Reliable, -1);
            writer.Write(__instance.MyDoor.Id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
    #endregion

}