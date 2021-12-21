using HarmonyLib;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy))]
    public class GhostSightRepair
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead && DestroyableSingleton<HudManager>.Instance && DestroyableSingleton<HudManager>.Instance.ShadowQuad) {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
            }
        }
    }
    
}