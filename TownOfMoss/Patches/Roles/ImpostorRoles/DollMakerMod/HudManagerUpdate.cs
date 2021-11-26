using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.DollMakerMod {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate {
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        public static void Postfix(HudManager __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.DollMaker)) return;
            var role = Role.GetRole<DollMaker>(PlayerControl.LocalPlayer);
            if (role._waxButton == null) {
                role._waxButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role._waxButton.graphic.enabled = true;
                role._waxButton.graphic.sprite = DollMaker._waxSprite;
            }

            role._waxButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role._waxButton.transform.localPosition = __instance.KillButton.transform.localPosition;

            __instance.KillButton.graphic.color = new Color(0, 0, 0, 0);
            __instance.KillButton.cooldownTimerText.color = new Color(0, 0, 0, 0);
            __instance.KillButton.gameObject.SetActive(false);

            var notImpostor = PlayerControl.AllPlayerControls.ToArray().Where(
                x => !x.Is(Faction.Impostors)
            ).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role._waxButton, float.NaN, notImpostor);
            
            if (role.ClosestPlayer) {
                role._waxButton.graphic.color = Palette.EnabledColor;
                role._waxButton.graphic.material.SetFloat(Desat, 0f);
            }
            else {
                role._waxButton.graphic.color = Palette.DisabledClear;
                role._waxButton.graphic.material.SetFloat(Desat, 1.0f);
            }
        }
    }
}