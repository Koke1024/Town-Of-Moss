using HarmonyLib;
using Hazel;
using Il2CppSystem;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class DoClick
    {

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.CanMorph();
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.MorphButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (role.MorphButton.graphic.sprite == Morphling.SampleSprite)
                {
                    if (target == null) return false;
                    role.SampledPlayer = target;
                    
                    if (RainbowUtils.IsRainbow(target.Data.DefaultOutfit.ColorId))
                        role.sampledColor = RainbowUtils.Rainbow;
                    else
                        role.sampledColor = Palette.PlayerColors[target.Data.DefaultOutfit.ColorId];
                    
                    role.MorphButton.graphic.sprite = Morphling.MorphSprite;
                    role.MorphButton.graphic.material.SetColor("_Color", role.sampledColor);
                    role.MorphButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    if (role.MorphTimer() < 5f)
                        role.LastMorphed = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MorphlingCd);
                }
                else
                {
                    if (role.Morphed) {
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte) CustomRPC.Unmorph,
                            SendOption.Reliable, -1);
                        writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        
                        role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        role.Unmorph();
                        return false;
                    }
                    if (__instance.isCoolingDown || role.MorphTimer() != 0) {
                        return false;
                    }
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.Morph,
                        SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.SampledPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    Utils.Morph(role.Player, role.SampledPlayer, true);
                }

                return false;
            }

            return true;
        }
    }
}
