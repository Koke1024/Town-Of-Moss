using System;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    public class StartStop
    {
        public static Color oldColor;

        public static void StartRewind(TimeLord role)
        {
            //System.Console.WriteLine("START...");
            RecordRewind.rewinding = true;
            RecordRewind.whoIsRewinding = role;
            PlayerControl.LocalPlayer.moveable = false;
            oldColor = HudManager.Instance.FullScreen.color;
            var rewindColor = new Color(0f, 0.5f, 0.8f, 0.3f);
            HudManager.Instance.FullScreen.color = rewindColor;
            HudManager.Instance.FullScreen.enabled = true;
            role.StartRewind = DateTime.UtcNow;
            
            PlayerControl.LocalPlayer.Collider.enabled = false;
            PlayerControl.LocalPlayer.NetTransform.enabled = false;

            if (CustomGameOptions.RewindFlash) {
                role.Player.myRend.material.SetFloat("_Outline", 10f);
                role.Player.myRend.material.SetColor("_OutlineColor", role.Color);                
            }

            if (Minigame.Instance) {
                Minigame.Instance.Close();
                Minigame.Instance.Close();                
            }
        }

        public static void StopRewind(TimeLord role)
        {
            //System.Console.WriteLine("STOP...");
            role.FinishRewind = DateTime.UtcNow;
            RecordRewind.rewinding = false;
            PlayerControl.LocalPlayer.moveable = true;
            HudManager.Instance.FullScreen.enabled = false;
            HudManager.Instance.FullScreen.color = oldColor;

            if (CustomGameOptions.RewindFlash) {
                role.Player.myRend.material.SetFloat("_Outline", 0f);
                role.Player.myRend.material.SetColor("_OutlineColor", new Color());
            }

            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(true);
            PlayerControl.LocalPlayer.Collider.enabled = true;
            PlayerControl.LocalPlayer.NetTransform.enabled = true;
        }
    }
}