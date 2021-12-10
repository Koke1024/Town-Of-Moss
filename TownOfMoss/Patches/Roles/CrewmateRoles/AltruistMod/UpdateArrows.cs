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

    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateMorph {
        public static void Postfix(PlayerControl __instance) {
            if (!CustomGameOptions.AltruistLendBody) {
                return;
            }

            if (__instance.Data.IsDead && __instance.Is(RoleEnum.Altruist)) {
                var role = Role.GetRole<Altruist>(__instance);
                if (role.revivedPlayer != null) {
                    Utils.Morph(role.revivedPlayer, __instance);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class MeetingHud_Start {
        public static void Postfix(MeetingHud __instance) {
            foreach (Altruist role in Role.GetRoles(RoleEnum.Altruist)) {
                if (role.revivedPlayer != null) {
                    Utils.Unmorph(role.revivedPlayer);
                    Utils.MurderPlayer(role.revivedPlayer, role.revivedPlayer);

                    Utils.GetBody(role.revivedPlayer.PlayerId).gameObject.Destroy();
                    role.revivedPlayer = null;
                    role.Player.Revive();
                }
            }
        }
    }
}