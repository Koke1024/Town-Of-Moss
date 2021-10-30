using HarmonyLib;
using Hazel;
using Il2CppSystem.Web.Util;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MayorMod {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update {
        public static Sprite Button => TownOfUs.ButtonSprite;

        public static void Postfix(HudManager __instance) {
            UpdateButtonButton(__instance);
        }

        private static void UpdateButtonButton(HudManager __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;

            var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

            if (role.ButtonButton == null) {
                role.ButtonButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.ButtonButton.renderer.enabled = true;
                role.ButtonButton.renderer.sprite = Button;
            }

            role.ButtonButton.renderer.sprite = Button;


            role.ButtonButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            role.ButtonButton.SetCoolDown(0f, 1f);
            var renderer = role.ButtonButton.renderer;

            var position1 = __instance.UseButton.transform.position;
            role.ButtonButton.transform.position = new Vector3(
                Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                position1.z);

            if (!role.ButtonUsed) {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    static class MayorUpdate {
        public static void Postfix(PlayerControl __instance) {
            if (PlayerControl.LocalPlayer == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer.Data == null) return;

            Mayor mayor = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
            if (mayor.Player.Data.IsDead && !mayor.ButtonUsed && CustomGameOptions.MayorMeetingOnDead) {
                mayor.ButtonUsed = true;
                if (MeetingHud.Instance != null) {
                    return;
                }
                mayor.reportDelay -= Time.fixedDeltaTime;
                if (mayor.reportDelay <= 0f) {
                    PerformKill.MayorEmergencyMeeting(mayor);
                }
            }
        }
    }
}