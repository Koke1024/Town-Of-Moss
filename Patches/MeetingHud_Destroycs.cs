using HarmonyLib;
using TownOfUs.Patches;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy))]
    public class MeetingHud_OnDestroy
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);                
            }
        }
    }
}