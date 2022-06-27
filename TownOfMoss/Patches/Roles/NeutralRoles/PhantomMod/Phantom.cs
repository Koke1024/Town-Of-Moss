using UnityEngine;

namespace TownOfUs.Roles
{
    public class Phantom : Role
    {
        public bool Caught;
        public bool CompletePhantomTasks;
        public bool Faded;

        public Phantom(PlayerControl player) : base(player)
        {
            Name = "Phantom";
            ImpostorText = () => "";
            TaskText = () => "Complete all your tasks without being caught!";
            Color = new Color(0.4f, 0.16f, 0.38f, 1f);
            RoleType = RoleEnum.Phantom;
            Faction = Faction.Neutral;
        }

        public void Wins() {
            CompletePhantomTasks = true;
        }
        public void Loses()
        {
            Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }

        public void Fade()
        {
            if (!ShipStatus.Instance) {
                return;
            }
            Faded = true;
            var color = new Color(1f, 1f, 1f, 0f);


            var maxDistance = ShipStatus.Instance.MaxLightRadius * PlayerControl.GameOptions.CrewLightMod;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.TrueGhostSpeed * 0.13f;
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            Player.MyRend.color = color;

            Player.HatRenderer.SetHat("", 0);
            Player.nameText.text = "";
            // if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
            //     .AllSkins.ToArray()[0].ProdId)
            //     Player.MyPhysics.SetSkin("");
            if (Player.CurrentPet != null) Object.Destroy(Player.CurrentPet.gameObject);
            // Player.CurrentPet =
            //     Object.Instantiate(
            //         DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0].PetPrefab);  //todo pet
            Player.CurrentPet.transform.position = Player.transform.position;
            Player.CurrentPet.Source = Player;
            Player.CurrentPet.Visible = Player.Visible;
        }
        
        public override bool DidWin(GameOverReason gameOverReason) {
            return CompletePhantomTasks;
        }

        public override void Outro(EndGameManager __instance) {
            base.Outro(__instance);
            NeutralOutro(__instance);
        }
    }
}