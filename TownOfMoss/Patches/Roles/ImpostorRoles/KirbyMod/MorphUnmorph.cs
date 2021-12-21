using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnmorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Kirby))
            {
                var kirby = (Kirby) role;
                if (kirby.Morphed) {
                    if (!GameData.Instance.GetPlayerById(kirby._aten.ParentId).IsDead) {
                        kirby.Unmorph();
                        return;
                    }
                    kirby.Morph();
                }
                else if (kirby.MorphedPlayer) kirby.Unmorph();
            }
        }
    }
}