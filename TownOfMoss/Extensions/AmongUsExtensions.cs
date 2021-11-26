using System.Collections.Generic;
using BepInEx.Logging;
using Il2CppSystem;
using Reactor;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Extensions
{
    public static class AmongUsExtensions {
        private static bool useLog = true;
        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (var keyValuePair in self)
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }

            return result;
        }

        public static KeyValuePair<byte, int> MaxPair(this byte[] self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            for (byte i = 0; i < self.Length; i++)
                if (self[i] > result.Value)
                {
                    result = new KeyValuePair<byte, int>(i, self[i]);
                    tie = false;
                }
                else if (self[i] == result.Value)
                {
                    tie = true;
                }

            return result;
        }

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player)
        {
            if(player.Data == null){
                return new VisualAppearance();
            }
            return new VisualAppearance()
            {
                ColorId = player.Data.DefaultOutfit.ColorId,
                HatId = player.Data.DefaultOutfit.HatId,
                SkinId = player.Data.DefaultOutfit.SkinId,
                PetId = player.Data.DefaultOutfit.PetId
            };
        }

        public static bool TryGetAppearance(this PlayerControl player, IVisualAlteration modifier,
            out VisualAppearance appearance) {
            if (modifier != null){
                return modifier.TryGetModifiedAppearance(out appearance);
            }

            if (player != null) {
                appearance = player.GetDefaultAppearance();                
            }
            else {
                appearance = null;
            }
            return false;
        }

        public static VisualAppearance GetAppearance(this PlayerControl player)
        {
            if (player.TryGetAppearance(Role.GetRole(player) as IVisualAlteration, out var appearance))
                return appearance;
            else if (player.TryGetAppearance(Modifier.GetModifier(player) as IVisualAlteration, out appearance))
                return appearance;
            else
                return player.GetDefaultAppearance();
        }

        public static void Log(object message, LogLevel level = LogLevel.Message) {
            if (!useLog) {
                return;
            }
            PluginSingleton<TownOfUs>.Instance.Log.Log(
                level, $"[{System.DateTime.Now}] {message}");
        }
    }
}