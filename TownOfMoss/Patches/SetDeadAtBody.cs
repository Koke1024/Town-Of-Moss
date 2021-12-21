using HarmonyLib;

namespace TownOfUs
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class SetDeadAtBody {
        public static void Postfix(PlayerControl __instance)
        {
            if (!CustomGameOptions.GhostCantMove) {
                return;
            }

            if (PlayerControl.LocalPlayer != __instance) {
                return;
            }
            
            if (Utils.myBody == null && Utils.ExistBody(__instance.PlayerId)) {
                Utils.myBody = Utils.GetBody(__instance.PlayerId);
            }

            if (Utils.myBody != null) {
                __instance.transform.position = Utils.myBody.TruePosition;                
            }
        }
    }
}
