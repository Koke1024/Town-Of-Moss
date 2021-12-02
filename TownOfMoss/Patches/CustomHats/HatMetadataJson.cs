namespace TownOfUs.Patches.CustomHats
{
    public class HatMetadataJson
    {
        public HatMetadataElement[] Credits { get; set; }

        public HatMetadataJson(HatMetadataElement[] credits) {
            Credits = credits;
        }
        public HatMetadataJson() {
            Credits = new HatMetadataElement[] { };
        }
    }
}