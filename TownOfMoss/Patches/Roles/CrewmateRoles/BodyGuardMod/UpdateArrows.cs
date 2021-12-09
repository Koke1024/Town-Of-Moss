using HarmonyLib;
using Il2CppSystem;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.BodyGuardMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.BodyGuard)) {
                return;
            }
            if (__instance != PlayerControl.LocalPlayer) {
                return;
            }
            var role = Role.GetRole<BodyGuard>(__instance);
            if (role.Arrow != null && role.ShieldedPlayer != null)
            {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead ||
                    role.ShieldedPlayer.Data.IsDead)
                {
                    role.ShieldedPlayer = null;
                    role.Arrow.Destroy();
                    return;
                }

                role.Arrow.target = role.ShieldedPlayer.transform.position;
                if (Vector2.Distance(__instance.GetTruePosition(), role.ShieldedPlayer.GetTruePosition()) > CustomGameOptions.GuardRange / 2.0f) {
                    role.Arrow.gameObject.SetActive(false);
                }
                else {
                    role.Arrow.gameObject.SetActive(true);
                }
                if((DateTime.UtcNow - role.ShieldedTime).TotalSeconds > CustomGameOptions.GuardDuration) {
                    role.ShieldedTime = DateTime.UtcNow;
                    role.ShieldedPlayer = null;
                    role.Arrow.gameObject.SetActive(false);
                    role.Arrow.Destroy();
                }
            }
        }
    }
}