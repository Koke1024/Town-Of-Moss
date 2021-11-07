using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.PainterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite PaintSprite => TownOfUs.PaintSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (ShipStatus.Instance == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) return;
            var role = Role.GetRole<Painter>(PlayerControl.LocalPlayer);

            if (!role._paintButtons.Any()) {
                AmongUsExtensions.Log($"no button");
                if (__instance.KillButton == null) {
                    AmongUsExtensions.Log($"kill button null");
                    return;
                }
                for (int i = 0; i < CustomGameOptions.PaintColorMax; ++i) {
                    KillButtonManager btn = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                    AmongUsExtensions.Log($"Instantiate");
                    btn.renderer.enabled = true;
                    btn.renderer.sprite = PaintSprite;
                    btn.renderer.material.color = Painter.PaintColors[i];
                    
                    btn.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
                    role._paintButtons.Add(btn);
                    
                    
                }
                AmongUsExtensions.Log($"count {role._paintButtons.Count}");
            }

            var t = 0; 
            role.closeVent = ClosestVent();
            foreach(var btn in role._paintButtons) {
                btn.SetCoolDown(role.PaintTimer(), CustomGameOptions.PaintCd);
                if (role.PaintTimer() == 0) {
                    btn.enabled = true;
                }
                if (role.closeVent == null) {
                    btn.renderer.sprite = TownOfUs.PaintSprite;
                }else{
                    btn.renderer.sprite = TownOfUs.PourSprite;
                }
                var offset = __instance.UseButton.transform.localPosition.y - __instance.ReportButton.transform.localPosition.y;
                var position = __instance.KillButton.transform.localPosition;
                btn.transform.localPosition = new Vector3(position.x + offset,
                    position.y - offset * t, position.z);
                ++t;
            }
        }
        public static Vent ClosestVent() {
            if (ShipStatus.Instance.AllVents.Count == 0) {
                return null;
            }
            Vent target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++) {
                Vent vent = ShipStatus.Instance.AllVents[i];
                if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                float distance = Vector2.Distance(vent.transform.position, truePosition);
                if (distance <= vent.UsableDistance && distance < closestDistance) {
                    closestDistance = distance;
                    target = vent;
                }
            }

            return target;
        }
    }
}