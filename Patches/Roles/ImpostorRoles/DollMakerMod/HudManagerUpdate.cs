using HarmonyLib;
using Il2CppSystem;
using Reactor;
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
                role._waxButton.renderer.enabled = true;
                role._waxButton.renderer.sprite = DollMaker._waxSprite;
            }

            role._waxButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;

            __instance.KillButton.renderer.color = new Color(0, 0, 0, 0);
            __instance.KillButton.TimerText.color = new Color(0, 0, 0, 0);

            // role._waxButton.transform.localPosition = new Vector3(position.x,
            // __instance.KillButton.transform.localPosition.y, position.z);

            Utils.SetTarget(ref role.ClosestPlayer, role._waxButton);
            
            if (role.ClosestPlayer) {
                role._waxButton.renderer.color = Palette.EnabledColor;
                role._waxButton.renderer.material.SetFloat(Desat, 0f);
            }
            else {
                role._waxButton.renderer.color = Palette.DisabledClear;
                role._waxButton.renderer.material.SetFloat(Desat, 1.0f);
            }
        }
    }
}