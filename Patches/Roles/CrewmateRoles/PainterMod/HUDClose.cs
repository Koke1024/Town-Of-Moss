using System.Collections.Generic;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.PainterMod
{

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) {
                return;
            }
            
            foreach (var (id, c) in Painter.PaintedPlayers) {
                var player = GameData.Instance.GetPlayerById(id);
                player._object.myRend.material.SetColor("_VisorColor", Palette.VisorColor);
            }
            Painter.PaintedPlayers.Clear();
        }
    }
}