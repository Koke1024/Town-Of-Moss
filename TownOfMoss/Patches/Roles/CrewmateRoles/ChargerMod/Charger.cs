using HarmonyLib;
using Reactor;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Charger : Role {
        public float Charge { get; set; }
        public bool flashed = false;
        
        public Charger(PlayerControl player) : base(player)
        {
            Name = "Charger";
            ImpostorText = () => "Charge energy to see around";
            TaskText = () => "Charge energy to see around";
            Color = new Color(0.99f, 1f, 0.2f);
            RoleType = RoleEnum.Charger;
        }

        public override void InitializeLocal() {
            base.InitializeLocal();
            Charge = 1.0f;
            flashed = false;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            Charge = 1.0f;
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            if (Player.inVent) {
                if (Charge > 1.0f) {
                    if (!flashed) {
                        Coroutines.Start(Utils.FlashCoroutine(Color));
                        flashed = true;
                    }
                    return;
                }
                Charge += 1 / (60.0f * CustomGameOptions.MaxChargeTime);
            }
            else {
                flashed = false;
                if (Charge < 0) {
                    Charge = 0;
                    return;
                }
                Charge -= 1 / (60.0f * CustomGameOptions.ConsumeChargeTime);
            }
        }
    }
    
    [HarmonyPatch(typeof(Vent), nameof(Vent.MoveToVent))]
    public static class DisableVentMove{
        public static bool Prefix(Vent __instance, Vent otherVent) {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Charger);
        }
    }
    
    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public class NoButton {
        public static bool Prefix(PlayerControl __instance) {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Charger);
        }
    }
}