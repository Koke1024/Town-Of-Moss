using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MinerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite MineSprite => TownOfUs.MineSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Miner.ventModel == null) {
                Miner.ventModel = Object.FindObjectOfType<Vent>(); 
            }
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Miner)) return;
            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);
            if (CustomGameOptions.MaxVentNum <= role.Vents.Count) {
                role.MineButton.graphic.color = Palette.DisabledClear;
                role.MineButton.graphic.material.SetFloat("_Desat", 1f);
                role.CanPlace = false;
                return;
            }
            if (role.MineButton == null)
            {
                role.MineButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.MineButton.graphic.enabled = true;
            }

            role.MineButton.graphic.sprite = MineSprite;
            role.MineButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.MineButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);
            role.MineButton.SetCoolDown(role.MineTimer(), CustomGameOptions.MineCd);

            if (role.VentSize == Vector2.zero) {
                var vent = Object.FindObjectOfType<Vent>();
                if (vent == null) {
                    return;
                }
                role.VentSize =
                    Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) * 0.75f;
            }

            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, role.VentSize, 0);
            hits = hits.ToArray().Where(c =>
                    (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                .ToArray();
            if (hits.Count == 0)
            {
                role.MineButton.graphic.color = Palette.EnabledColor;
                role.MineButton.graphic.material.SetFloat("_Desat", 0f);
                role.CanPlace = true;
            }
            else
            {
                role.MineButton.graphic.color = Palette.DisabledClear;
                role.MineButton.graphic.material.SetFloat("_Desat", 1f);
                role.CanPlace = false;
            }
        }
    }
}