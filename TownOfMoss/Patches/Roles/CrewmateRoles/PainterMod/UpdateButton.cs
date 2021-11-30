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
        public static Sprite[] PaintSprite => TownOfUs.PaintSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (ShipStatus.Instance == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) return;
            var role = Role.GetRole<Painter>(PlayerControl.LocalPlayer);

            if (!role._paintButtons.Any()) {
                if (__instance.KillButton == null) {
                    return;
                }
                for (int i = 0; i < CustomGameOptions.PaintColorMax; ++i) {
                    ActionButton btn = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    btn.graphic.enabled = true;
                    btn.graphic.sprite = PaintSprite[i];
                    
                    role._paintButtons.Add(btn);
                }
            }

            var t = 0; 
            // role.closeVent = ClosestVent();

            bool onInk = false;
            foreach (var (pos, color) in Painter.PaintedPoint) {
                var dist = Vector2.Distance(pos, PlayerControl.LocalPlayer.GetTruePosition());
                if (dist < 2.5f) {
                    onInk = true;
                    break;
                }
            }

            if (!onInk) {
                foreach (var (pos, color) in Painter.PaintedPointBefore) {
                    var dist = Vector2.Distance(pos, PlayerControl.LocalPlayer.GetTruePosition());
                    if (dist < 2.5f) {
                        onInk = true;
                        break;
                    }
                }
            }
            
            foreach(var btn in role._paintButtons) {
                btn.SetCoolDown(role.PaintTimer(), CustomGameOptions.PaintCd);
                if (!onInk) {
                    btn.graphic.color = Palette.EnabledColor;
                    btn.graphic.material.SetFloat("_Desat", 0f);
                    btn.enabled = true;
                }
                else {
                    btn.graphic.color = Palette.DisabledClear;
                    btn.graphic.material.SetFloat("_Desat", 1f);
                    btn.enabled = false;
                }
                btn.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
                // if (role.closeVent == null) {
                //     btn.renderer.sprite = TownOfUs.PaintSprite[t];
                // }else{
                //     btn.renderer.sprite = TownOfUs.PourSprite;
                // }
                var offset = __instance.UseButton.transform.localPosition.y - __instance.ReportButton.transform.localPosition.y;
                // var position = __instance.KillButton.transform.localPosition;
                // btn.transform.localPosition = new Vector3(position.x + offset,
                //     position.y - offset * t, position.z);
                btn.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition + new Vector3(offset, - offset * t, 0);
                ++t;
                btn.gameObject.SetActive(false);
                btn.GetComponent<AspectPosition>().Update();
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