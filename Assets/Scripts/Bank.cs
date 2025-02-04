using System.Collections.Generic;

public class Bank
{
    // public int Money { get; set; } // Not needed as bank has endless money in digital versions 
    public int Houses { get; set; }
    public int Hotels { get; set; }
    public List<Property> TitleDeeds { get; set; } 
    
    public Bank(int houses, int hotels, List<Property> titleDeeds)
    {
        Houses = houses;
        Hotels = hotels;
        TitleDeeds = titleDeeds;
    }
    
    private void GiveMoney(Player player, int m)
    {
        player.UpdateMoney(m);
    }

    private void TakeMoney(Player player, int m)
    {
        if (player.Money >= m)
        {
            player.UpdateMoney(-m);
        }
    }
}