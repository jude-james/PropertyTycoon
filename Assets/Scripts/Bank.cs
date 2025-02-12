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
    
    // tbh now we know the bank has endless money, these methods might be pointless, and if so, the bank class may be
    // small enough again to merge into the board class
    private void GiveMoney(Player player, int amount)
    {
        player.UpdateMoney(amount);
    }

    private void TakeMoney(Player player, int amount)
    {
        player.UpdateMoney(-amount);
    }
}