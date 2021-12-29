using System.Linq;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Sniffer : Role
    {
        public float sniffInterval = 5f;

        public Sniffer(PlayerControl player) : base(player)
        {
            Name = "Sniffer";
            ImpostorText = () => "Finding a dead body with your sense of smell";
            TaskText = () =>  "Finding a dead body with your sense of smell";
            Color = new Color(0.65f, 0f, 0.83f);
            RoleType = RoleEnum.Sniffer;

            sniffInterval = 5.0f;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color =
                new Color(0.27451f, 0.27451f, 0.27451f);
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            if (!PlayerControl.LocalPlayer.CanMove) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            sniffInterval -= Time.deltaTime;
            if (sniffInterval > 0) {
                return;
            }

            if (!Utils.KilledPlayers.Any()) {
                sniffInterval = 1.0f;
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color = new Color(0.27451f, 0.27451f, 0.27451f);
                return;
            }
            float closestDistance = CustomGameOptions.SnifferMaxRange * CustomGameOptions.SnifferMaxRange;
            foreach (var killed in Utils.KilledPlayers) { 
                closestDistance = Mathf.Min(closestDistance, Vector2.SqrMagnitude(Player.GetTruePosition() - killed.Value.Body.TruePosition));
            }

            closestDistance = Mathf.Sqrt(closestDistance);

            var clamp = Mathf.Clamp(closestDistance / CustomGameOptions.SnifferMaxRange, 0, 1.0f);

            if (DestroyableSingleton<HudManager>.Instance.ShadowQuad) {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color =
                    Color.Lerp(new Color(1f, 0f, 0.2f), new Color(0.27451f, 0.27451f, 0.27451f), clamp);
            }

            sniffInterval = 1.0f;
        }

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if(!CustomGameOptions.SnifferCanReport) {
                __instance.ReportButton.enabled = false;
                __instance.ReportButton.SetActive(false);
            }
        }
    }
}