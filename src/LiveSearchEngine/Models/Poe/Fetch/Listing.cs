using System;

namespace LiveSearchEngine.Models.Poe.Fetch
{
    public class Listing
    {
        public DateTime Indexed { get; set; }
        public string Method { get; set; }
        public Account Account { get; set; }
        public Price Price { get; set; }
        public Stash Stash { get; set; }
        public string Whisper { get; set; }
    }
}