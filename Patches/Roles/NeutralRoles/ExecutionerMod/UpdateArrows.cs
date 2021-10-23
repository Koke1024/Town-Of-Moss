using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.Executioner)) {
                return;
            }

            if (__instance != PlayerControl.LocalPlayer) {
                return;
            }
            var role = Role.GetRole<Executioner>(__instance);
            if (role.Arrow != null)
            {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead ||
                    role.target.Data.IsDead)
                {
                    role.Arrow.gameObject.Destroy();
                    role.target = null;
                    return;
                }

                role.Arrow.target = role.target.transform.position;
            }
        }
    }
}