using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Kirby)) return;

            var role = Role.GetRole<Kirby>(PlayerControl.LocalPlayer);
            if (role.InhaleButton == null)
            {
                role.InhaleButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InhaleButton.graphic.enabled = true;
                role.InhaleButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.InhaleButton.gameObject.SetActive(false);
                role.InhaleButton.graphic.sprite = TownOfUs.Inhale;
            }
            role.InhaleButton.GetComponent<AspectPosition>().Update();
            role.InhaleButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);


            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[0];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = role.InhaleButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();
                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }


            InhaleButtonTarget.SetTarget(killButton, closestBody, role);
            role.InhaleButton.SetCoolDown(0, 1.0f);
        }
    }
}