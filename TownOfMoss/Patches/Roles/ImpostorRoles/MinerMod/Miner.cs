using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Miner : Assassin
    {
        public readonly List<Vent> Vents = new List<Vent>();
        public static Vent ventModel = null;

        public KillButton _mineButton;
        public DateTime LastMined;


        public Miner(PlayerControl player) : base(player)
        {
            Name = "Miner";
            ImpostorText = () => "From the top, make it drop, that's a vent";
            TaskText = () => "From the top, make it drop, that's a vent";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Miner;
            Faction = Faction.Impostors;
            VentSize = Vector2.zero;
        }

        public override void InitializeLocal() {
            base.InitializeLocal();
            
            LastMined = DateTime.UtcNow;
            LastMined = LastMined.AddSeconds(-10f);
        }

        public bool CanPlace { get; set; }
        public Vector2 VentSize { get; set; }

        public KillButton MineButton
        {
            get => _mineButton;
            set
            {
                _mineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float MineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMined;
            var num = CustomGameOptions.MineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            LastMined = DateTime.UtcNow;
        }

        public static Sprite MineSprite => TownOfUs.MineSprite;
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            if (CustomGameOptions.MaxVentNum <= Vents.Count) {
                MineButton.graphic.color = Palette.DisabledClear;
                MineButton.graphic.material.SetFloat("_Desat", 1f);
                CanPlace = false;
                return;
            }
            if (MineButton == null)
            {
                MineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                MineButton.graphic.enabled = true;
                MineButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                MineButton.gameObject.SetActive(false);
            }

            MineButton.GetComponent<AspectPosition>().Update();
            MineButton.graphic.sprite = MineSprite;
            MineButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            MineButton.SetCoolDown(MineTimer(), CustomGameOptions.MineCd);

            if (VentSize == Vector2.zero) {
                var vent = Object.FindObjectOfType<Vent>();
                if (vent == null) {
                    return;
                }
                VentSize =
                    Vector2.Scale(vent.GetComponent<BoxCollider2D>().size, vent.transform.localScale) * 0.75f;
            }

            var hits = Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, VentSize, 0);
            hits = hits.ToArray().Where(c =>
                    (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                .ToArray();
            if (hits.Count == 0)
            {
                MineButton.graphic.color = Palette.EnabledColor;
                MineButton.graphic.material.SetFloat("_Desat", 0f);
                CanPlace = true;
            }
            else
            {
                MineButton.graphic.color = Palette.DisabledClear;
                MineButton.graphic.material.SetFloat("_Desat", 1f);
                CanPlace = false;
            }
        }
    }
}