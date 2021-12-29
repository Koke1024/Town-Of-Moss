using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class DollMaker : Assassin
    {
        public KillButton _waxButton;
        public System.Collections.Generic.Dictionary<byte, float> DollList = new System.Collections.Generic.Dictionary<byte, float>();
        public PlayerControl ClosestPlayer;
        public static Sprite _waxSprite => TownOfUs.WaxSprite;

        public DollMaker(PlayerControl player) : base(player)
        {
            Name = "DollMaker";
            ImpostorText = () => "Make crews art";
            TaskText = () => "Wax Crews to make them dolls.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.DollMaker;
            Faction = Faction.Impostors;
        }
        
        public KillButton WaxButton
        {
            get => _waxButton;
            set
            {
                _waxButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public override void PostFixedUpdate() {
            base.PostFixedUpdate();
            
            if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                return;
            }
            
            var breakList = new Queue<byte>();
            foreach (var doll in DollList) {
                if (GameData.Instance.GetPlayerById(doll.Key).IsDead) {
                    continue;
                }
                PlayerControl closestPlayer = null;
                var targets = PlayerControl.AllPlayerControls.ToArray()
                    .ToList().FindAll(x =>
                        x.PlayerId != Player.PlayerId && x.PlayerId != doll.Key);
                if (Utils.SetClosestPlayerToPlayer(GameData.Instance.GetPlayerById(doll.Key)._object, ref closestPlayer,
                    0.8f, targets
                )) {
                    breakList.Enqueue(doll.Key);
                    continue;
                }
                
                if (GameData.Instance.GetPlayerById(doll.Key).IsDead) {
                    breakList.Enqueue(doll.Key);
                    continue;
                }
                if (doll.Key == PlayerControl.LocalPlayer.PlayerId) {
                    DollList[doll.Key] += Time.fixedDeltaTime;
                    PlayerControl.LocalPlayer.moveable = false;
                    PlayerControl.LocalPlayer.NetTransform.Halt();
                    if (DollList[doll.Key] >= CustomGameOptions.DollBreakTime) {
                        breakList.Enqueue(doll.Key);
                    }
                }
            }
            
            foreach (var breakQueue in breakList) {
                Utils.RpcMurderPlayer(GameData.Instance.GetPlayerById(breakQueue)._object, GameData.Instance.GetPlayerById(breakQueue)._object);
                DollList.Remove(breakQueue);
            }
        }

        private static readonly int Desat = Shader.PropertyToID("_Desat");
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (_waxButton == null) {
                _waxButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                _waxButton.graphic.enabled = true;
                _waxButton.graphic.sprite = DollMaker._waxSprite;
            }

            _waxButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            _waxButton.transform.localPosition = __instance.KillButton.transform.localPosition;

            __instance.KillButton.graphic.color = new Color(0, 0, 0, 0);
            __instance.KillButton.cooldownTimerText.color = new Color(0, 0, 0, 0);
            __instance.KillButton.gameObject.SetActive(false);

            var notImpostor = PlayerControl.AllPlayerControls.ToArray().Where(
                x => !x.Is(Faction.Impostors)
            ).ToList();
            Utils.SetTarget(ref ClosestPlayer, _waxButton, float.NaN, notImpostor);
            
            if (ClosestPlayer) {
                _waxButton.graphic.color = Palette.EnabledColor;
                _waxButton.graphic.material.SetFloat(Desat, 0f);
            }
            else {
                _waxButton.graphic.color = Palette.DisabledClear;
                _waxButton.graphic.material.SetFloat(Desat, 1.0f);
            }
        }
    }
}