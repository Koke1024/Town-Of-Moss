using UnityEngine;

namespace TownOfUs
{
    public class VisualAppearance
    {
        public float SpeedFactor { get; set; } = 1.0f;
        public Vector3 SizeFactor { get; set; } = new Vector3(0.7f, 0.7f, 1.0f);

        public int ColorId { get; set; }
        public string HatId { get; set; }
        public string SkinId { get; set; }
        public string PetId { get; set; }
    }
}