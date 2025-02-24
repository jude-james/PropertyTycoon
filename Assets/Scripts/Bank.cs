using System.Collections.Generic;
using Tiles;

/// <summary>
/// Persistent bank object throughout duration of game, containing houses, hotels and list of titleDeed cards
/// </summary>
public class Bank
{
    public int Houses { get; set; }
    public int Hotels { get; set; }
    public List<Property> TitleDeeds { get; set; } 
    
    /// <summary>
    /// Initializes a new instance of the Bank class.
    /// </summary>
    /// <param name="houses">The initial number of houses.</param>
    /// <param name="hotels">The initial number of hotels.</param>
    /// <param name="titleDeeds">The initial list of title deeds.</param>
    public Bank(int houses, int hotels, List<Property> titleDeeds)
    {
        Houses = houses;
        Hotels = hotels;
        TitleDeeds = titleDeeds;
    }
}