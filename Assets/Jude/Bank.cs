using System;
using System.Collections.Generic;

namespace Jude
{
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

        public void Transaction(Player player, int amount)
        {
            throw new NotImplementedException();
            // player.Money += amount;
        }
    }
}