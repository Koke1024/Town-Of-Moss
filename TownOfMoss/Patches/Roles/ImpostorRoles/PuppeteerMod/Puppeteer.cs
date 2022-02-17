using System.Linq;
using Hazel;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using TownOfUs.CrewmateRoles.BodyGuardMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Puppeteer : Assassin

    {
        public KillButton _possessButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl PossessPlayer;
        public DateTime PossStart;
        public float PossessTime;
        public float duration;
        public DateTime lastPossess;
        public bool possessStarting = false;
        public static List<byte> CantReportPlayer = new List<byte>();
        
        public static Sprite PossessSprite => TownOfUs.PossessSprite;
        public static Sprite UnPossessSprite => TownOfUs.ReleaseSprite;

        public Puppeteer(PlayerControl player) : base(player)
        {
            Name = "Puppeteer";
            ImpostorText = () => "Control Crew to Kill";
            TaskText = () => "Control Crew to Kill";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Puppeteer;
            Faction = Faction.Impostors;

            PossessPlayer = null;
            ClosestPlayer = null;
            _possessButton = null;
            
            lastPossess = DateTime.UtcNow.AddSeconds(-10.0f);
        }

        public KillButton PossessButton
        {
            get => _possessButton;
            set
            {
                _possessButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void UnPossess() {
            PossessPlayer = null;
            Player.moveable = true;
            if (PlayerControl.LocalPlayer == Player) {
                PossessButton.graphic.sprite = Puppeteer.PossessSprite;
            }
            // duration = Mathf.Max(PossessTime, 3.0f);
            duration = CustomGameOptions.ReleaseWaitTime;
        }
        public void KillUnPossess() {
            if (PlayerControl.LocalPlayer == Player) {
                Player.SetKillTimer(PlayerControl.GameOptions.KillCooldown);                
            }
            UnPossess();
        }

        public override void PostFixedUpdate() {
            base.PostFixedUpdate();
            
            if (PossessPlayer != null && (LobbyBehaviour.Instance || MeetingHud.Instance)) {
                PossessPlayer = null;
                PossessTime = 0;
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.UnPossess,
                    SendOption.Reliable, -1);
                writer2.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                UnPossess();
                return;
            }

            if (PossessPlayer != null) {
                if (PlayerControl.LocalPlayer == Player) {
                    PossessTime += Time.fixedDeltaTime;
                    if (PossessTime > CustomGameOptions.PossessMaxTime || PossessPlayer.Data.IsDead) {
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte) CustomRPC.UnPossess,
                            SendOption.Reliable, -1);
                        writer2.Write(Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        UnPossess();
                    }
                }
            }
            if (duration > 0) {
                if (PossessPlayer == null) {
                    Player.moveable = false;
                    Player.NetTransform.Halt();
                    duration -= Time.fixedDeltaTime;
                }

                if (duration <= 0) {
                    Player.moveable = true;                    
                }
            }
            
            if (PossessPlayer == PlayerControl.LocalPlayer) {
                PlayerControl closestPlayer = null;
                var targets = PlayerControl.AllPlayerControls.ToArray().Where(
                    x => !x.Is(Faction.Impostors) && x.PlayerId != PossessPlayer.PlayerId
                ).ToList();
                if (Utils.SetClosestPlayer(ref closestPlayer,
                    GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance] * 0.75f, targets
                )) {
                    if (closestPlayer.isShielded())
                    {
                        var bodyGuard = closestPlayer.getBodyGuard().Player.PlayerId;
                        var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer1.Write(bodyGuard);
                        writer1.Write(closestPlayer.PlayerId);
                        writer1.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer1);

                        if (CustomGameOptions.ShieldBreaks) {
                            KillUnPossess();
                        }

                        StopKill.BreakShield(bodyGuard, closestPlayer.PlayerId, PossessPlayer.Data.PlayerId, CustomGameOptions.ShieldBreaks);
                        return;
                    }
                    
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.PossessKill,
                        SendOption.Reliable, -1);
                    writer2.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, closestPlayer);
                    KillUnPossess();
                    CantReportPlayer.Add(closestPlayer.PlayerId);
                }
            }
            
        }

        private static readonly int Desat = Shader.PropertyToID("_Desat");
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (PossessButton == null) {
                PossessButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                PossessButton.graphic.enabled = true;
                PossessButton.graphic.sprite = Puppeteer.PossessSprite;
                PossessButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                PossessButton.gameObject.SetActive(false);
            }
            PossessButton.GetComponent<AspectPosition>().Update();

            if (PossessButton.graphic.sprite != Puppeteer.PossessSprite &&
                PossessButton.graphic.sprite != Puppeteer.UnPossessSprite)
                PossessButton.graphic.sprite = Puppeteer.PossessSprite;

            if (PossessButton.graphic.sprite == Puppeteer.PossessSprite) {
                if ((lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000.0f + PlayerControl.GameOptions.KillCooldown > 0) {
                    PossessButton.SetCoolDown((float)(lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000 + PlayerControl.GameOptions.KillCooldown, PlayerControl.GameOptions.KillCooldown);       
                    Player.SetKillTimer((float)(lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000 + PlayerControl.GameOptions.KillCooldown);
                    return;
                }

                if (duration > 0) {
                    //待機
                    PossessButton.SetCoolDown(duration, CustomGameOptions.ReleaseWaitTime);
                    // PossessButton.SetCoolDown(duration, Mathf.Max(3.0f, PossessTime));
                }
                else {
                    if ((float)(CustomGameOptions.PossessTime - (DateTime.UtcNow - PossStart).TotalMilliseconds / 1000.0f) > 0) {
                        PossessButton.SetCoolDown( (float)(CustomGameOptions.PossessTime - (DateTime.UtcNow - PossStart).TotalMilliseconds / 1000.0f), CustomGameOptions.PossessTime);
                    }
                    else {
                        PossessButton.SetCoolDown(Player.killTimer, PlayerControl.GameOptions.KillCooldown);
                    }
                }
                PossessButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance);
                PossessButton.graphic.enabled = !MeetingHud.Instance && !LobbyBehaviour.Instance;
                Utils.SetTarget(ref ClosestPlayer, PossessButton);
                if (ClosestPlayer) {
                    PossessButton.graphic.color = Palette.EnabledColor;
                    PossessButton.graphic.material.SetFloat(Desat, 0f);
                }
                else {
                    PossessButton.graphic.color = Palette.DisabledClear;
                    PossessButton.graphic.material.SetFloat(Desat, 1.0f);
                }
            }
            else {
                PossessButton.SetCoolDown((float)((DateTime.UtcNow - PossStart).TotalMilliseconds / 1000.0f), CustomGameOptions.PossessMaxTime);
                PossessButton.graphic.material.SetFloat(Desat, 0f);
                PossessButton.graphic.color = Palette.EnabledColor;
                PossessButton.enabled = true;
            }
        }
    }
}