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
    
    public Bank(int houses, int hotels, List<Property> titleDeeds)
    {
        Houses = houses;
        Hotels = hotels;
        TitleDeeds = titleDeeds;
    }
}