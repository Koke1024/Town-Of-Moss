using HarmonyLib;

namespace TownOfUs.Patches
{
    [HarmonyPatch]
    public static class RevivePatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
        public static void Prefix(PlayerControl __instance) {
            __instance.NetTransform.SnapTo(Utils.GetBody(__instance.PlayerId).TruePosition - __instance.Collider.offset);
        }
    }
}