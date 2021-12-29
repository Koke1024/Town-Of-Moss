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
    }
}