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
                    if (ventTarget != null) { // Seal vent
                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SealVent, Hazel.SendOption.Reliable);
                        writer.WritePacked(ventTarget.Id);
                        writer.EndMessage();
                        SGAction.sealVent(ventTarget.Id);
                        ventTarget = null;
                    }
                    else if (PlayerControl.GameOptions.MapId != (byte)ShipStatus.MapType.Hq &&
                             PlayerControl.GameOptions.MapId != (byte)ShipStatus.MapType.Ship) {
                        // Place camera if there's no vent and it's not MiraHQ
                        var pos = PlayerControl.LocalPlayer.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(GetBytes(pos.x), 0, buff, 0*sizeof(float), sizeof(float));
                        Buffer.BlockCopy(GetBytes(pos.y), 0, buff, 1*sizeof(float), sizeof(float));

                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceCamera, Hazel.SendOption.Reliable);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        SGAction.placeCamera(buff);
                    }
                    else {
                        return;
                    }
                    SecurityGuardButton.Timer = SecurityGuardButton.MaxTimer;
                },
                () => {
                    return role != null &&
                           role.Player == PlayerControl.LocalPlayer &&
                           !PlayerControl.LocalPlayer.Data.IsDead && remainingScrews >=
                           Mathf.Min(ventPrice, camPrice);
                },
                () => {
                    SecurityGuardButton.killButton.graphic.color = Palette.EnabledColor;
                    SecurityGuardButton.killButton.graphic.material.SetFloat("_Desat", 0f);
                    if (ventTarget == null) {
                        if (PlayerControl.GameOptions.MapId == (byte)ShipStatus.MapType.Pb ||
                            PlayerControl.GameOptions.MapId == (byte)ShipStatus.MapType.Ship) {
                            SecurityGuardButton.killButton.graphic.sprite = getPlaceCameraButtonSprite();                            
                        }
                        else {
                            SecurityGuardButton.killButton.graphic.sprite = getCloseVentButtonSprite();
                            SecurityGuardButton.killButton.graphic.color = Palette.DisabledClear;
                            SecurityGuardButton.killButton.graphic.material.SetFloat("_Desat", 1f);                            
                        }
                    }
                    else {
                        SecurityGuardButton.killButton.graphic.sprite = getCloseVentButtonSprite();
                    }
                    if (SecurityGuardButtonScrewsText != null)
                        SecurityGuardButtonScrewsText.text =
                            $"{remainingScrews}/{totalScrews}";

                    if (ventTarget != null)
                        return remainingScrews >= ventPrice &&
                               PlayerControl.LocalPlayer.CanMove;
                    return PlayerControl.GameOptions.MapId != 1 &&
                           remainingScrews >= camPrice &&
                           PlayerControl.LocalPlayer.CanMove;
                },
                () => { SecurityGuardButton.Timer = SecurityGuardButton.MaxTimer; },
                getPlaceCameraButtonSprite(),
                Vector3.zero, 
                HudManager.Instance,
                KeyCode.Q
            );

            SecurityGuardButton.MaxTimer = cooldown;
            SecurityGuardButtonScrewsText = GameObject.Instantiate(SecurityGuardButton.killButton.cooldownTimerText,
                SecurityGuardButton.killButton.cooldownTimerText.transform.parent);
            SecurityGuardButtonScrewsText.text = "";
            SecurityGuardButtonScrewsText.enableWordWrapping = false;
            SecurityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
            SecurityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
            
            SecurityGuardButton.killButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
            SecurityGuardButton.killButton.gameObject.SetActive(false);
        }
    }
}