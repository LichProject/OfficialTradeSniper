using LiveSearchEngine.Models.Poe.Enums;

namespace LiveSearchEngine.Models.Poe
{
    public class Item
    {
        /// <summary>
        /// Fetch Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Item name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Character league.
        /// </summary>
        public string League { get; set; }

        /// <summary>
        /// Item base type name.
        /// </summary>
        [JsonProperty("typeLine")]
        public string BaseType { get; set; }

        /// <summary>
        /// Item icon url.
        /// </summary>
        public string Icon { get; set; }

        [JsonProperty("descrText")]
        public string Description { get; set; }

        [JsonProperty("secDescrText")]
        public string SecondDescription { get; set; }

        /// <summary>
        /// Item rarity type.
        /// </summary>
        [JsonProperty("frameType")]
        public ItemRarity Rarity { get; set; }

        /// <summary>
        /// Item width.
        /// </summary>
        [JsonProperty("w")]
        public int Width { get; set; }

        /// <summary>
        /// Item height.
        /// </summary>
        [JsonProperty("h")]
        public int Height { get; set; }

        /// <summary>
        /// Item ilvl.
        /// </summary>
        [JsonProperty("ilvl")]
        public int ItemLevel { get; set; }

        public Influences Influences { get; set; }

        /// <summary>
        /// Item implicit mods.
        /// </summary>
        public string[] ImplicitMods { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Item explicit mods.
        /// </summary>
        public string[] ExplicitMods { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Item enchant mods.
        /// </summary>
        public string[] EnchantMods { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Item crafted mods.
        /// </summary>
        public string[] CraftedMods { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Item fractured mods.
        /// </summary>
        public string[] FracturedMods { get; set; } = Array.Empty<string>();
        
        /// <summary>
        /// Item base properties.
        /// </summary>
        public Property[] Properties { get; set; } = Array.Empty<Property>();
        
        /// <summary>
        /// Item base properties.
        /// </summary>
        public Property[] AdditionalProperties { get; set; } = Array.Empty<Property>();

        /// <summary>
        /// Item requirements.
        /// </summary>
        public Property[] Requirements { get; set; } = Array.Empty<Property>();

        /// <summary>
        /// Item sockets.
        /// </summary>
        public Socket[] Sockets { get; set; } = Array.Empty<Socket>();

        public Extended Extended { get; set; }

        /// <summary>
        /// Hybrid with other item data (example vaal gem).
        /// </summary>
        public Item Hybrid { get; set; }
        
        /// <summary>
        /// Item identified or not.
        /// </summary>
        public bool Identified { get; set; }

        /// <summary>
        /// Support gem or not.
        /// </summary>
        public bool Support { get; set; }

        public bool IsVaalGem { set; get; }

        /// <summary>
        /// Item is corrupted or not.
        /// </summary>
        public bool Corrupted { get; set; }
        
        /// <summary>
        /// Item is mirrored or not.
        /// </summary>
        [JsonProperty("duplicated")]
        public bool Mirrored { get; set; }
        
        /// <summary>
        /// Item is synthesised or not.
        /// </summary>
        public bool Synthesised { get; set; }
        
        /// <summary>
        /// Item is fractured or not.
        /// </summary>
        public bool Fractured { get; set; }

        /// <summary>
        /// Item is stackable and have stack size.
        /// </summary>
        public decimal? StackSize { get; set; } = 1;

        public bool Verified { get; set; }
    }
}