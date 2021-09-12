namespace LiveSearchEngine.Models.Poe
{
    public class Account
    {
        /// <summary>
        /// Player language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Player character name.
        /// </summary>
        public string LastCharacterName { get; set; }

        /// <summary>
        /// Account name.
        /// </summary>
        public string Name { get; set; }
        
        public Online Online { get; set; }
    }
}