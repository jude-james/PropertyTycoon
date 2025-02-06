using System.Collections.Generic;
using Tiles;

/// <summary>
/// A struct that stores all the data from external files needed
/// </summary>
public struct GameData
{
    public Tile[] Tiles { get; set; }
    public List<Property> Properties { get; set; }
    public Dictionary<string, string> OpportunityKnocksCards { get; set; }
    public Dictionary<string, string> PotLuckCards { get; set; }
}