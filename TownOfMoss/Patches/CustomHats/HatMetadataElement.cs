namespace TownOfUs.Patches.CustomHats
{
    public class HatMetadataElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }

        public HatMetadataElement(string id, string name, string artist) {
            Id = id;
            Name = name;
            Artist = artist;
        }

        public HatMetadataElement() {
            Id = "";
            Name = "";
            Artist = "";
        }
    }
}