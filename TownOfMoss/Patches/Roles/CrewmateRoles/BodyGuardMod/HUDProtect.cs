using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.BodyGuardMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDProtect
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateProtectButton(__instance);
        }

        public static void UpdateProtectButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.BodyGuard)) return;
            
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var protectButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<BodyGuard>(PlayerControl.LocalPlayer);


            if (isDead)
            {
                protectButton.gameObject.SetActive(false);
            }
            else
            {
                protectButton.gameObject.SetActive(!MeetingHud.Instance);
                protectButton.SetCoolDown(role.ShieldTimer(), CustomGameOptions.GuardCoolDown);
                Utils.SetTarget(ref role.ClosestPlayer, protectButton);
            }
        }
    }
}
