using UnityEngine;
using System.Collections.Generic;

namespace Rudy
{
    public class Player
    {
        enum Token
        {
            Boot,
            Smartphone,
            Ship,
            Hatstand,
            Cat,
            Iron
        }
        private string name;
        private int money;
        private List<Property> ownedProperty;
        private int currentSpace;
        private int jailCards;
        private bool inJail;
        private bool bot;

        public Player(string name, bool bot)
        {
            // Initialize starting stats
            money = 1500;
            currentSpace = 0;
            this.name = name;
            this.bot = bot;
        }

        public int Move(int diceValue)
        {
            currentSpace = currentSpace + diceValue;
            return currentSpace;
            // Returns position to allow board to handle finding the correct space and then update the UI
            // I think that the board will only need to know the players position after movement, but if not a GetSpace function will be added
        }

        public void UpdateMoney(int amount)
        {
            // If amount is negative, checks that there is money to take
            if (amount < 0 && money < (amount*-1))
            {
                // If they can't pay, player has to mortgage or go bankrupt
            }
            else
            {
                money += amount;
            }
        }

        public string GetName() {  return name; }
        public int GetMoney() { return money; }
        public List<Property> HeldProperties() { return ownedProperty; }
        public bool IsInJail() { return inJail; }
    }
}