namespace LiveSearchEngine.Models.Poe
{
    public class Stash
    {
        /// <summary>
        /// Stash tab name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Stash tab X position (starts at 0).
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Stash tab Y position (starts at 0).
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Stash tab X position (starts at 1).
        /// </summary>
        public int RealX => X + 1;

        /// <summary>
        /// Stash tab Y position (starts at 1).
        /// </summary>
        public int RealY => Y + 1;
    }
}