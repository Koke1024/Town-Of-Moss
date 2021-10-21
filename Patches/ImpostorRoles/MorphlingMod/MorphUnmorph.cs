using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnmorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetMorphRoles())
            {
                var morphling = (Morphling) role;
                if (morphling.Morphed)
                    morphling.Morph();
                else if (morphling.MorphedPlayer) morphling.Unmorph();
            }
        }
    }
}