using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tiles
{
    public class Utility : Property
    {
        private int _rent1;
        private int _rent2;
        private string _utilType;
        
        public void SetUp(string name, int cost, int rent1, int rent2, string utilType)
        {
            _rent1 = rent1;
            _rent2 = rent2;
            _utilType = utilType;
            base.SetUp(name, cost);
        }

        protected override void SetCard()
        {
            var prefabName = "UtilElec";
            if (_utilType == "Water")
            {
                prefabName = "UtilWater";
            }
            
            Card = Instantiate(Resources.Load("Prefabs/Cards/" + prefabName)) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
            }
        }

        protected override void PayRent(Player player)
        {
            // figure out rent based on dice roll, this code needs testing
            var amount = CurrentRent * player.DiceRoll;
            player.TakeMoney(amount);
            OwnedBy.GiveMoney(amount);
        }
    }
}