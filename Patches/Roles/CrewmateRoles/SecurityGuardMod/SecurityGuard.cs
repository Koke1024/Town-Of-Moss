using System.Linq;
using HarmonyLib;
using Hazel;
using Il2CppSystem;
using TheOtherRoles.Objects;
using TownOfUs.Extensions;
using TownOfUs.Patches.CrewmateRoles.SecurityGuardMod;
using UnityEngine;
using static Il2CppSystem.BitConverter;
using Buffer = System.Buffer;

namespace TownOfUs.Roles
{
    public class SecurityGuard : Role {

        public static float cooldown = 30f;
        public static int remainingScrews = 7;
        public static int totalScrews = 7;
        public static int ventPrice = 1;
        public static int camPrice = 2;
        public static int placedCameras = 0;
        public static Vent ventTarget = null;
        private static Sprite closeVentButtonSprite;
        
        public static CustomButton SecurityGuardButton = null;
        public static TMPro.TMP_Text SecurityGuardButtonScrewsText;
        
        public SecurityGuard(PlayerControl player) : base(player)
        {
            Name = "SecurityGuard";
            ImpostorText = () => "Protect Security for Crewmates";
            TaskText = () => "Seal vents and add cameras";
            Color = new Color(0.67f, 0.67f, 1f);
            RoleType = RoleEnum.SecurityGuard;
            
            ventTarget = null;
            placedCameras = 0;
            cooldown = CustomGameOptions.SecurityGuardCooldown;
            totalScrews = remainingScrews = Mathf.RoundToInt(CustomGameOptions.SecurityGuardTotalScrews);
            camPrice = Mathf.RoundToInt(CustomGameOptions.SecurityGuardCamPrice);
            ventPrice = Mathf.RoundToInt(CustomGameOptions.SecurityGuardVentPrice);

            ButtonInit();
        }
        
        public static Sprite getCloseVentButtonSprite() {
            if (closeVentButtonSprite) return closeVentButtonSprite;
            closeVentButtonSprite = TownOfUs.CloseVentButtonSprite;
            return closeVentButtonSprite;
        }

        private static Sprite placeCameraButtonSprite;
        public static Sprite getPlaceCameraButtonSprite() {
            if (placeCameraButtonSprite) return placeCameraButtonSprite;
            placeCameraButtonSprite = TownOfUs.PlaceCameraSprite;
            return placeCameraButtonSprite;
        }

        private static Sprite animatedVentSealedSprite;
        public static Sprite getAnimatedVentSealedSprite() {
            if (animatedVentSealedSprite) return animatedVentSealedSprite;
            animatedVentSealedSprite = TownOfUs.AnimatedVentSprite;
            return animatedVentSealedSprite;
        }

        private static Sprite staticVentSealedSprite;
        public static Sprite getStaticVentSealedSprite() {
            if (staticVentSealedSprite) return staticVentSealedSprite;
            staticVentSealedSprite = TownOfUs.StaticVentSprite;
            return staticVentSealedSprite;
        }
        
        public void ButtonInit() {
            var role = Role.GetRoles(RoleEnum.SecurityGuard).FirstOrDefault();
            if (role == null) {
                return;
            }
            SecurityGuardButton = new CustomButton(
                () => {
                    if (SecurityGuard.ventTarget != null) {
                        // Seal vent
                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SealVent, Hazel.SendOption.Reliable);
                        writer.WritePacked(SecurityGuard.ventTarget.Id);
                        writer.EndMessage();
                        SGAction.sealVent(SecurityGuard.ventTarget.Id);
                        SecurityGuard.ventTarget = null;
                    }
                    else if (PlayerControl.GameOptions.MapId != 1) {
                        // Place camera if there's no vent and it's not MiraHQ
                        var pos = PlayerControl.LocalPlayer.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.PlaceCamera, Hazel.SendOption.Reliable);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        SGAction.placeCamera(buff);
                    }

                    SecurityGuardButton.Timer = SecurityGuardButton.MaxTimer;
                },
                () => {
                    return role != null &&
                           role.Player == PlayerControl.LocalPlayer &&
                           !PlayerControl.LocalPlayer.Data.IsDead && SecurityGuard.remainingScrews >=
                           Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice);
                },
                () => {
                    SecurityGuardButton.killButtonManager.renderer.sprite =
                        (SecurityGuard.ventTarget == null && PlayerControl.GameOptions.MapId != 1)
                            ? SecurityGuard.getPlaceCameraButtonSprite()
                            : SecurityGuard.getCloseVentButtonSprite();
                    if (SecurityGuardButtonScrewsText != null)
                        SecurityGuardButtonScrewsText.text =
                            $"{SecurityGuard.remainingScrews}/{SecurityGuard.totalScrews}";

                    if (SecurityGuard.ventTarget != null)
                        return SecurityGuard.remainingScrews >= SecurityGuard.ventPrice &&
                               PlayerControl.LocalPlayer.CanMove;
                    return PlayerControl.GameOptions.MapId != 1 &&
                           SecurityGuard.remainingScrews >= SecurityGuard.camPrice &&
                           PlayerControl.LocalPlayer.CanMove;
                },
                () => { SecurityGuardButton.Timer = SecurityGuardButton.MaxTimer; },
                SecurityGuard.getPlaceCameraButtonSprite(),
                new Vector3(-1.3f, 0f, 0f),
                HudManager.Instance,
                KeyCode.Q
            );

            SecurityGuardButton.MaxTimer = SecurityGuard.cooldown;
            SecurityGuardButtonScrewsText = GameObject.Instantiate(SecurityGuardButton.killButtonManager.TimerText,
                SecurityGuardButton.killButtonManager.TimerText.transform.parent);
            SecurityGuardButtonScrewsText.text = "";
            SecurityGuardButtonScrewsText.enableWordWrapping = false;
            SecurityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
            SecurityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
        }
    }
}