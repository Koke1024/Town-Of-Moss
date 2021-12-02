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
                role.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MorphButton.graphic.enabled = true;
                role.MorphButton.graphic.sprite = Morphling.SampleSprite;
                role.MorphButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.MorphButton.gameObject.SetActive(false);
                
                if (role.Player.Is(RoleEnum.Jester)) {
                    role.MorphButton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(TownOfUs.ButtonPosition.x + TownOfUs.ButtonOffset.x, TownOfUs.ButtonPosition.y, TownOfUs.ButtonPosition.z);
                }
            }
            role.MorphButton.GetComponent<AspectPosition>().Update();

            if (role.MorphButton.graphic.sprite != Morphling.SampleSprite &&
                role.MorphButton.graphic.sprite != Morphling.MorphSprite)
                role.MorphButton.graphic.sprite = Morphling.SampleSprite;

            role.MorphButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            


            if (role.MorphButton.graphic.sprite == Morphling.SampleSprite) {
                role.MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
                if (role.ClosestPlayer) {
                    role.MorphButton.graphic.color = Palette.EnabledColor;
                    role.MorphButton.graphic.material.SetFloat(Desat, 0f);
                }
                else {
                    role.MorphButton.graphic.color = Palette.DisabledClear;
                    role.MorphButton.graphic.material.SetFloat(Desat, 1.0f);
                }
            }
            else {
                if (role.Morphed) {
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }
                role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.MorphButton.graphic.color = role.sampledColor;
                role.MorphButton.graphic.material.SetFloat(Desat, 0f);
            }
        }
    }
}