

using System.Collections.Generic;
using Hazel;
using Il2CppSystem;
using Reactor.Extensions;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public enum PaintColor {
        PaintRed,
        PaintGreen,
        PaintBlue,
        PaintColorMax,
        PaintNone,
    }
    public class Painter : Role {
        public List<KillButtonManager> _paintButtons = new List<KillButtonManager>();
        public Vent closeVent;
        public static readonly Color[] PaintColors = {
            new Color(1, 0, 0),
            new Color(0, 0, 1),
            new Color(0.79f, 0.78f, 0f),
        };
        public DateTime lastPainted;
        public static Dictionary<byte, PaintColor> PaintedPlayers = new Dictionary<byte, PaintColor>();
        public static Dictionary<int, PaintColor> PaintedVent = new Dictionary<int, PaintColor>();
        public static List<(Vector2, PaintColor)> PaintedPoint = new List<(Vector2, PaintColor)>();
        public static Dictionary<int, PaintColor> PaintedVentBefore = new Dictionary<int, PaintColor>();
        public static List<(Vector2, PaintColor)> PaintedPointBefore = new List<(Vector2, PaintColor)>();
        public static List<GameObject> InkListBefore = new List<GameObject>();
        public static List<GameObject> InkList = new List<GameObject>();
        
        public static Sprite InkSprite => TownOfUs.InkSprite;
        public static Sprite PourSprite => TownOfUs.PourSprite;

        public Painter(PlayerControl player) : base(player) {
            Name = "Painter";
            ImpostorText = () => "Paint Floor";
            TaskText = () => "Paint Floor And Mark Players";
            Color = new Color(0.81f, 0.81f, 0.81f);
            RoleType = RoleEnum.Painter;
            lastPainted = DateTime.UtcNow;


            PaintedPlayers = new Dictionary<byte, PaintColor>();
            PaintedVent = new Dictionary<int, PaintColor>();
            PaintedPoint = new List<(Vector2, PaintColor)>();
            PaintedVentBefore = new Dictionary<int, PaintColor>();
            PaintedPointBefore = new List<(Vector2, PaintColor)>();
            InkList = new List<GameObject>();
        }

        public float PaintTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastPainted;
            var num = CustomGameOptions.PaintCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public static void RpcSetPaintPoint(Vector2 point, PaintColor color) {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetPaintPoint, SendOption.Reliable, -1);
            writer.Write(point);
            writer.Write((byte)color);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            SetPaintPoint(point, color);
        }
        
        public static void RpcSetPaintVent(int id, PaintColor color) {
            if (PaintedVent.ContainsKey(id) && PaintedVent[id] == color) {
                return;
            }
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetPaintVent, SendOption.Reliable, -1);
            writer.Write(id);
            writer.Write((byte)color);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            SetPaintVent(id, color);
        }
        
        public static void RpcSetPaintPlayer(byte id, PaintColor color) {
            if (PaintedPlayers.ContainsKey(id) && PaintedPlayers[id] == color) {
                return;
            }
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetPaintPlayer, SendOption.Reliable, -1);
            writer.Write(id);
            writer.Write((byte)color);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            SetPaintPlayer(id, color);
        }
        
        public static void SetPaintPoint(Vector2 point, PaintColor color) {
            PaintedPointBefore.Add((point, color));

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) {
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
                var ink = new GameObject("Ink");
                Vector3 position = new Vector3(point.x, point.y, PlayerControl.LocalPlayer.transform.localPosition.z + 0.001f); // just behind player
                ink.transform.position = position;
                ink.transform.localPosition = position;

                var inkRenderer = ink.AddComponent<SpriteRenderer>();
                inkRenderer.sprite = InkSprite;
                inkRenderer.color = Color.Lerp(PaintColors[(int)color], new Color(1,1,1,0), 0.5f);
                
                PlayerControl.LocalPlayer.StartCoroutine(Effects.ScaleIn(ink.transform, 0, 1.0f, 0.15f));
                
                ink.SetActive(true);
                InkListBefore.Add(ink);
            }
        }
        
        public static void SetPaintVent(int id, PaintColor color) {
            PaintedVentBefore[id] = color;

            // if (PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) {
                Vent vent = ShipStatus.Instance.AllVents[id];
                vent.myRend.color = PaintColors[(int)color];
                vent.myRend.material.SetColor("_OutlineColor", PaintColors[(int)color]);
            // }
        }
        
        public static void SetPaintPlayer(byte id, PaintColor color) {
            PaintedPlayers[id] = color;
            AmongUsExtensions.Log($"player {id}'s new color:{PaintedPlayers[id]}");
        }

        public static void IsColoredVent(int id, ref PaintColor color) {
            if (!PaintedVent.ContainsKey(id)) {
                color = PaintColor.PaintNone;
                return;
            }
            color = PaintedVent[id];
        }
        
        
    }
}