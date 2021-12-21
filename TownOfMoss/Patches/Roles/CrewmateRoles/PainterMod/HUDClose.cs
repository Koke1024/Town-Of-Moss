using HarmonyLib;
using Il2CppSystem;
using MonoMod.Utils;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.PainterMod
{

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            // if (!PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) {
            //     return;
            // }
            
            foreach (var (id, c) in Painter.PaintedPlayers) {
                var player = GameData.Instance.GetPlayerById(id);
                player._object.myRend.material.SetColor("_VisorColor", Palette.VisorColor);
            }



            if (PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) {
                Role.GetRole<Painter>(PlayerControl.LocalPlayer).lastPainted = DateTime.UtcNow;
                Painter.InkListBefore.Clear();
            }

            foreach (var (point, color) in Painter.PaintedPointBefore) {
                var ink = new GameObject("Ink");
                Vector3 position = new Vector3(point.x, point.y,
                    Utils.getZfromY(point.y) + 0.001f); // just behind player
                ink.transform.position = position;
                ink.transform.localPosition = position;

                var inkRenderer = ink.AddComponent<SpriteRenderer>();
                inkRenderer.sprite = Painter.InkSprite;
                inkRenderer.color = Painter.PaintColors[(int)color];
                inkRenderer.color = Color.Lerp(Painter.PaintColors[(int)color], new Color(1,1,1,0), 0.2f);

                ink.SetActive(true);
                Painter.InkList.Add(ink);
            }

            Painter.PaintedPoint.AddRange(Painter.PaintedPointBefore);
            Painter.PaintedPointBefore.Clear();
            
            Painter.PaintedVent.AddRange(Painter.PaintedVentBefore);
            Painter.PaintedVentBefore.Clear();
            
            Painter.PaintedPlayers.Clear();
        }
    }
}