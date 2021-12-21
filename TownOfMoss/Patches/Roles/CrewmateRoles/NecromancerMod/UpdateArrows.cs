using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.NecromancerMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.Necromancer)) {
                return;
            }

            if (Coroutine.Arrow != null) {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead ||
                    Coroutine.Target.Data.IsDead) {
                    Coroutine.Arrow.gameObject.Destroy();
                    Coroutine.Target = null;
                    return;
                }

                Coroutine.Arrow.target = Coroutine.Target.transform.position;
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class NecroClean {
        public static void Prefix(ExileController __instance) {
            foreach (Necromancer role in Role.GetRoles(RoleEnum.Necromancer)) {
                foreach (var revived in role.RevivedPlayer) {
                    Utils.MurderPlayer(revived, revived);
                    Utils.GetBody(revived.PlayerId).gameObject.Destroy();
                }
                role.RevivedPlayer.Clear();
            }
        }
    }
}