using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using Il2CppSystem;
using Il2CppSystem.Text.RegularExpressions;
using Reactor;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Extensions {
    public static class AmongUsExtensions {
        private static bool useLog = true;

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie) {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (var keyValuePair in self)
                if (keyValuePair.Value > result.Value) {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value) {
                    tie = true;
                }

            return result;
        }

        public static KeyValuePair<byte, int> MaxPair(this byte[] self, out bool tie) {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            for (byte i = 0; i < self.Length; i++)
                if (self[i] > result.Value) {
                    result = new KeyValuePair<byte, int>(i, self[i]);
                    tie = false;
                }
                else if (self[i] == result.Value) {
                    tie = true;
                }

            return result;
        }

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player) {
            return new VisualAppearance();
        }

        public static bool TryGetAppearance(this PlayerControl player, IVisualAlteration modifier,
            out VisualAppearance appearance) {
            if (modifier != null)
                return modifier.TryGetModifiedAppearance(out appearance);

            appearance = player.GetDefaultAppearance();
            return false;
        }

        public static VisualAppearance GetAppearance(this PlayerControl player) {
            if (player.TryGetAppearance(Role.GetRole(player) as IVisualAlteration, out var appearance))
                return appearance;
            else if (player.TryGetAppearance(Modifier.GetModifier(player) as IVisualAlteration, out appearance))
                return appearance;
            else
                return player.GetDefaultAppearance();
        }

        public static void Log(object message = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0) {
            if (!useLog) {
                return;
            }

            PluginSingleton<TownOfUs>.Instance.Log.Log(
                LogLevel.Message, $"[{System.DateTime.Now}][{Regex.Replace(filePath, ".*\\\\", "\\")}:{lineNumber}]{memberName} {message}");
        }

        public static bool IsImpostor(this GameData.PlayerInfo playerinfo) {
            return playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;
        }

        public static void SetImpostor(this GameData.PlayerInfo playerinfo, bool impostor) {
            if (playerinfo.Role != null)
                playerinfo.Role.TeamType = impostor ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
        }

        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl) {
            return playerControl.Data.DefaultOutfit;
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType,
            GameData.PlayerOutfit outfit) {
            playerControl.Data.SetOutfit((PlayerOutfitType)CustomOutfitType, outfit);
            playerControl.SetOutfit(CustomOutfitType);
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType) {
            var outfitType = (PlayerOutfitType)CustomOutfitType;
            if (!playerControl.Data.Outfits.ContainsKey(outfitType)) {
                return;
            }

            var newOutfit = playerControl.Data.Outfits[outfitType];
            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId);
            playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            if (playerControl?.MyPhysics?.Skin?.skin?.ProdId != newOutfit.SkinId)
                playerControl.RawSetSkin(newOutfit.SkinId);
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }


        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) {
            return (CustomPlayerOutfitType)playerControl.CurrentOutfitType;
        }

        public static bool IsNullOrDestroyed(this System.Object obj) {
            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;

            return false;
        }
    }
}