using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MorphlingMod {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate {
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        public static void Postfix(HudManager __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.CanMorph()) return;
            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
            if (role.MorphButton == null) {
                role.MorphButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.MorphButton.renderer.enabled = true;
                role.MorphButton.renderer.sprite = Morphling.SampleSprite;
            }

            if (role.MorphButton.renderer.sprite != Morphling.SampleSprite &&
                role.MorphButton.renderer.sprite != Morphling.MorphSprite)
                role.MorphButton.renderer.sprite = Morphling.SampleSprite;

            role.MorphButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;

            if (role.Player.Is(RoleEnum.Jester)) {
                var size = __instance.UseButton.transform.localPosition.y - __instance.ReportButton.transform.localPosition.y;
                role.MorphButton.transform.localPosition = new Vector3(position.x + size,
                    __instance.ReportButton.transform.localPosition.y, position.z);
            }
            else {
                role.MorphButton.transform.localPosition = new Vector3(position.x,
                    __instance.ReportButton.transform.localPosition.y, position.z);
            }

            if (role.MorphButton.renderer.sprite == Morphling.SampleSprite) {
                role.MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
                if (role.ClosestPlayer) {
                    role.MorphButton.renderer.color = Palette.EnabledColor;
                    role.MorphButton.renderer.material.SetFloat(Desat, 0f);
                }
                else {
                    role.MorphButton.renderer.color = Palette.DisabledClear;
                    role.MorphButton.renderer.material.SetFloat(Desat, 1.0f);
                }
            }
            else {
                if (role.Morphed) {
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }
                role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.MorphButton.renderer.color = role.sampledColor;
                role.MorphButton.renderer.material.SetFloat(Desat, 0f);
            }
        }
    }
}