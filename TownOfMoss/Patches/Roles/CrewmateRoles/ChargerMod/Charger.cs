using HarmonyLib;
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
            
            Charge = 1.0f;
            flashed = false;
        }
    }
    
    [HarmonyPatch(typeof(Vent), nameof(Vent.MoveToVent))]
    public static class DisableVentMove{
        public static bool Prefix(Vent __instance, Vent otherVent) {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Charger)) {
                return false;
            }
            return true;
        }
    }
}