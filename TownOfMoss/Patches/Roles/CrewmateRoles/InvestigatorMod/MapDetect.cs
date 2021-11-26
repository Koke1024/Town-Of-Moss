using Il2CppSystem.Collections.Generic;
using HarmonyLib;
using Il2CppSystem;
using Rewired;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    public static class ShowNeighborhood {

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
        public static class InvestigatorMapClose {
            private static void Postfix(MapTaskOverlay __instance) {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) {
                    return;
                }
                Investigator role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
                foreach (var row in role.crewPoints) {
                    row.Value.enabled = false;
                }
                PlayerControl.LocalPlayer.moveable = true;
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
        public static class positionUpdate {
            private static void Postfix(MapBehaviour __instance) {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) {
                    return;
                }
                var role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);

                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt();
                if ((DateTime.UtcNow - role.lastUpdated).TotalSeconds < CustomGameOptions.InvestigatorMapUpdate) {
                    return;
                }

                var positions = new System.Collections.Generic.Dictionary<byte, Vector2>();
                foreach (DeadBody body in GameObject.FindObjectsOfType<DeadBody>()){
                    if (body.Reported) {
                        continue;
                    }
                    positions.Add(body.ParentId, body.TruePosition);
                }

                role.lastUpdated = DateTime.UtcNow;

                if (role.crewPoints.Count == 0) {
                    foreach (var playerControl in PlayerControl.AllPlayerControls) {
                        if (playerControl.PlayerId == PlayerControl.LocalPlayer.PlayerId) {
                            continue;
                        }
                        var crewIcon = GameObject.Instantiate(MapBehaviour.Instance.HerePoint, MapBehaviour.Instance.HerePoint.transform.parent, true);
                        role.crewPoints.Add(playerControl.PlayerId, crewIcon);
                    }
                }

                if (role.crewPoints.Count == 0) {
                    return;
                }

                foreach (var row in role.crewPoints) {
                    var player = Utils.PlayerById(row.Key);
                    var position = player.GetTruePosition();
                    if (player.Data.IsDead) {
                        if (positions.ContainsKey(player.Data.PlayerId)) {
                            position = positions[player.Data.PlayerId];
                        }
                        else {
                            continue;
                        }
                    }
                    var distance =
                        Vector2.Distance(position, PlayerControl.LocalPlayer.GetTruePosition());
                    if(distance < (player.Data.IsDead? CustomGameOptions.InvestigatorSeeRange: CustomGameOptions.InvestigatorSeeRange / 2.0f))
                    {
                        if (distance < CustomGameOptions.InvestigatorSeeRange * CustomGameOptions.InvestigatorSeeColorRange / 200.0f && !player.inVent) {
                            player.SetPlayerMaterialColors(row.Value);
                        }
                        else {
                            PlayerControl.SetPlayerMaterialColors(Color.black, row.Value);
                        }
                        var vector = new Vector3(position.x, position.y, player.transform.position.z);
                        vector /= ShipStatus.Instance.MapScale;
                        vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                        vector.z = -1f;
                        row.Value.transform.localPosition = vector;
                        row.Value.enabled = true;
                    }
                    else {
                        row.Value.enabled = false;
                    }
                }
            }
        }
    }
}