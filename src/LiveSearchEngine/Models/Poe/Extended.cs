namespace LiveSearchEngine.Models.Poe
{
    public class Extended
    {
        [JsonProperty("ar")]
        public int Armour { get; set; }

        [JsonProperty("ar_aug")]
        public bool IsArmourAugmented { get; set; }

        [JsonProperty("ev")]
        public int Evasion { get; set; }

        [JsonProperty("ev_aug")]
        public bool IsEvasionAugmented { get; set; }

        [JsonProperty("es")]
        public int EnergyShield { get; set; }

        [JsonProperty("es_aug")]
        public bool IsEnergyShieldAugmented { get; set; }

        public object Hashes { get; set; }
        public object Mods { get; set; }

        [JsonProperty("text")]
        public string Base64InGameClipboardText { get; set; }
    }
}