using System.Collections.Generic;

namespace Jude
{
    /// <summary>
    /// A struct that stores all data needed for game to start
    /// </summary>
    public struct GameData
    {
        public Space[] Spaces { get; set; }
        public List<Property> Properties { get; set; }
        public Dictionary<string, string> OpportunityKnocksCards { get; set; }
        public Dictionary<string, string> PotLuckCards { get; set; }
    }
}