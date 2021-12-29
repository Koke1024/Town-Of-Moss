using Il2CppSystem;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Morphling : Undertaker, IVisualAlteration

    {
        public KillButton _morphButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;

        public PlayerControl SampledPlayer;

        public Color sampledColor;
        public float TimeRemaining;
        
        public static Sprite SampleSprite => TownOfUs.SampleSprite;
        public static Sprite MorphSprite => TownOfUs.MorphSprite;

        public Morphling(PlayerControl player) : base(player)
        {
            LastMorphed = DateTime.UtcNow;
            if (GetType() != typeof(Morphling)) {
                return;
            }
            Name = "Morphling";
            ImpostorText = () => "Transform into crewmates";
            TaskText = () => "Morph into crewmates to be disguised";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Morphling;
            Faction = Faction.Impostors;
        }

        public KillButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public bool Morphed => TimeRemaining > 0f;

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Utils.Unmorph(Player);
            LastMorphed = DateTime.UtcNow;
            TimeRemaining = 0;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MorphedPlayer);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            MorphButton.graphic.sprite = TownOfUs.SampleSprite;
            SampledPlayer = null;
            LastMorphed = DateTime.UtcNow;
        }

        private static readonly int Desat = Shader.PropertyToID("_Desat");
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (MorphButton == null) {
                MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                MorphButton.graphic.enabled = true;
                MorphButton.graphic.sprite = Morphling.SampleSprite;
                MorphButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                MorphButton.gameObject.SetActive(false);
                
                if (Player.Is(RoleEnum.Jester)) {
                    MorphButton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(TownOfUs.ButtonPosition.x + TownOfUs.ButtonOffset.x, TownOfUs.ButtonPosition.y, TownOfUs.ButtonPosition.z);
                }
            }
            MorphButton.GetComponent<AspectPosition>().Update();

            if (MorphButton.graphic.sprite != Morphling.SampleSprite &&
                MorphButton.graphic.sprite != Morphling.MorphSprite)
                MorphButton.graphic.sprite = Morphling.SampleSprite;

            MorphButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            


            if (MorphButton.graphic.sprite == Morphling.SampleSprite) {
                MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref ClosestPlayer, MorphButton);
                if (ClosestPlayer) {
                    MorphButton.graphic.color = Palette.EnabledColor;
                    MorphButton.graphic.material.SetFloat(Desat, 0f);
                }
                else {
                    MorphButton.graphic.color = Palette.DisabledClear;
                    MorphButton.graphic.material.SetFloat(Desat, 1.0f);
                }
            }
            else {
                if (Morphed) {
                    MorphButton.SetCoolDown(TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }
                MorphButton.SetCoolDown(MorphTimer(), CustomGameOptions.MorphlingCd);
                MorphButton.graphic.color = sampledColor;
                MorphButton.graphic.material.SetFloat(Desat, 0f);
            }
        }
    }
}