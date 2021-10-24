using System.Linq;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.NeutralRoles.ZombieMod {
    public class ReviveSelf {
        public static void ReviveBody(PlayerControl player) {
            if (!player.Is(RoleEnum.Zombie)) {
                return;
            }
            Zombie role = Role.GetRole<Zombie>(player);
            if (role.KilledBySeer || LobbyBehaviour.Instance || MeetingHud.Instance) {
                role.KilledBySeer = true;
                role.deadTime = null;
                return;
            }
            
            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
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
            player.NetTransform.SnapTo(body.transform.position);

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
}