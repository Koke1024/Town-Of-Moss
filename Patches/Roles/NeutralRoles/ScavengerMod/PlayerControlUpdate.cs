using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.ScavengerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Scavenger)) return;

            var role = Role.GetRole<Scavenger>(PlayerControl.LocalPlayer);
            if (role.EatButton == null)
            {
                role.EatButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.EatButton.renderer.enabled = true;
            }

            role.EatButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var offset = __instance.KillButton.transform.localPosition.y - __instance.ReportButton.transform.localPosition.y;
            var position = __instance.KillButton.transform.localPosition;
            role.EatButton.transform.localPosition = new Vector3(position.x + offset,
                __instance.ReportButton.transform.localPosition.y, position.z);

            role.EatButton.renderer.sprite = TownOfUs.Inhale;


            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = role.EatButton;
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


            EatButtonTarget.SetTarget(killButton, closestBody, role);
            role.EatButton.SetCoolDown(0, 1.0f);
        }
    }
}