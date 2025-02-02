using System;
using System.Collections.Generic;

namespace Jude
{
    public class Bank
    {
        public int Money { get; set; }
        public int Houses { get; set; }
        public int Hotels { get; set; }
        public List<Property> TitleDeeds { get; set; } 
        
        public Bank(int money, int houses, int hotels, List<Property> titleDeeds)
        {
            Money = money;
            Houses = houses;
            Hotels = hotels;
            TitleDeeds = titleDeeds;
        }

        public void Transaction(Player player, int amount)
        {
            throw new NotImplementedException();
            // Money -= amount;
            // player.Money += amount;
        }
    }
}