using HarmonyLib;
using Il2CppSystem;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.PuppeteerMod {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate {
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        public static void Postfix(HudManager __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Puppeteer)) return;
            var role = Role.GetRole<Puppeteer>(PlayerControl.LocalPlayer);
            if (role.PossessButton == null) {
                role.PossessButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PossessButton.graphic.enabled = true;
                role.PossessButton.graphic.sprite = Puppeteer.PossessSprite;
                role.PossessButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.PossessButton.gameObject.SetActive(false);
            }
            role.PossessButton.GetComponent<AspectPosition>().Update();

            if (role.PossessButton.graphic.sprite != Puppeteer.PossessSprite &&
                role.PossessButton.graphic.sprite != Puppeteer.UnPossessSprite)
                role.PossessButton.graphic.sprite = Puppeteer.PossessSprite;

            if (role.PossessButton.graphic.sprite == Puppeteer.PossessSprite) {
                if ((role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000.0f + PlayerControl.GameOptions.KillCooldown > 0) {
                    role.PossessButton.SetCoolDown((float)(role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000 + PlayerControl.GameOptions.KillCooldown, PlayerControl.GameOptions.KillCooldown);       
                    role.Player.SetKillTimer((float)(role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000 + PlayerControl.GameOptions.KillCooldown);
                    return;
                }

                if (role.duration > 0) {
                    //待機
                    role.PossessButton.SetCoolDown(role.duration, CustomGameOptions.ReleaseWaitTime);
                    // role.PossessButton.SetCoolDown(role.duration, Mathf.Max(3.0f, role.PossessTime));
                }
                else {
                    if ((float)(CustomGameOptions.PossessTime - (DateTime.UtcNow - role.PossStart).TotalMilliseconds / 1000.0f) > 0) {
                        role.PossessButton.SetCoolDown( (float)(CustomGameOptions.PossessTime - (DateTime.UtcNow - role.PossStart).TotalMilliseconds / 1000.0f), CustomGameOptions.PossessTime);
                    }
                    else {
                        role.PossessButton.SetCoolDown(role.Player.killTimer, PlayerControl.GameOptions.KillCooldown);
                    }
                }
                role.PossessButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance);
                role.PossessButton.graphic.enabled = !MeetingHud.Instance && !LobbyBehaviour.Instance;
                Utils.SetTarget(ref role.ClosestPlayer, role.PossessButton);
                if (role.ClosestPlayer) {
                    role.PossessButton.graphic.color = Palette.EnabledColor;
                    role.PossessButton.graphic.material.SetFloat(Desat, 0f);
                }
                else {
                    role.PossessButton.graphic.color = Palette.DisabledClear;
                    role.PossessButton.graphic.material.SetFloat(Desat, 1.0f);
                }
            }
            else {
                role.PossessButton.SetCoolDown((float)((DateTime.UtcNow - role.PossStart).TotalMilliseconds / 1000.0f), CustomGameOptions.PossessMaxTime);
                role.PossessButton.graphic.material.SetFloat(Desat, 0f);
                role.PossessButton.graphic.color = Palette.EnabledColor;
                role.PossessButton.enabled = true;
            }
        }
    }
}