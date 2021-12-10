using System.Collections;
using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.AltruistMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.Altruist)) {
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
    public class MeetingHud_Start {
        public static void Prefix(MeetingHud __instance) {
            foreach (Altruist role in Role.GetRoles(RoleEnum.Altruist)) {
                if (role.revivedPlayer != null && !role.revivedPlayer.Data.IsDead) {
                    Utils.MurderPlayer(role.revivedPlayer, role.revivedPlayer);

                    Utils.GetBody(role.revivedPlayer.PlayerId).gameObject.Destroy();
                    role.revivedPlayer = null;
                    role.Player.Revive();
                }
            }
        }
    }
}