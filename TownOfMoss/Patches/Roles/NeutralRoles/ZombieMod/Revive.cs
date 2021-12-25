using System.Linq;
using HarmonyLib;
using Il2CppSystem;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.ZombieMod {
    public class ReviveSelf {
        public static void ReviveBody(PlayerControl player) {
            if (!player.Is(RoleEnum.Zombie)) {
                return;
            }
            Zombie role = Role.GetRole<Zombie>(player);
            if (role.KilledBySeer || LobbyBehaviour.Instance || MeetingHud.Instance) {
                role.KilledBySeer = true;
                role.deadTime = DateTime.MaxValue;
                return;
            }
            
            player.Revive();
            foreach (var p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Kirby))) {
                Kirby kirby = Role.GetRole<Kirby>(p);
                if(kirby._aten && kirby._aten.ParentId == player.PlayerId) {
                    kirby.Spit();
                }
            }
            var body = Object.FindObjectsOfType<DeadBody>()
                .FirstOrDefault(b => b.ParentId == player.PlayerId);
            if (body == null) {
                return;
            }

            Object.Destroy(body.gameObject);

            if (player == PlayerControl.LocalPlayer) {
                player.myTasks.RemoveAt(0);
            }

            var revived = new System.Collections.Generic.List<PlayerControl>();
            revived.Add(player);

            if (revived.Any(x => x.AmOwner)) {
                try {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch {
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
    public static class Movable {
        public static void Postfix(PlayerControl __instance) {
            if (__instance.Is(RoleEnum.Zombie)) {
                Role.GetRole<Zombie>(__instance).Player.moveable = true;
            }
        }
    }
}