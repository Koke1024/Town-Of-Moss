using HarmonyLib;
using TownOfUs.Patches;
using TownOfUs.Roles;

namespace TownOfUs
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class SetDeadAtBody
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.LocalPlayer == __instance) {
                DeadBody body = CanMove.CanMovePatch.GetMyBody();
                if (body) {
                    __instance.transform.position = body.TruePosition;
                }
            }
        }
    }
}
