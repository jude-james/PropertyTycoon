using System.Linq;
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
        
        public void SetUp(string name, int cost, int propertyNumber, int rent1, int rent2, string utilType)
        {
            _rent1 = rent1;
            _rent2 = rent2;
            _utilType = utilType;
            CurrentRent = _rent1;
            base.SetUp(name, cost, propertyNumber);
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
            var numberOfUtilitiesOwned = OwnedBy.TitleDeeds.Count(property => property != null && property is Utility);
            if (numberOfUtilitiesOwned == 1)
                CurrentRent = _rent1 * player.DiceRoll;
            else if (numberOfUtilitiesOwned == 2) 
                CurrentRent = _rent2 * player.DiceRoll;
            
            player.TakeMoney(CurrentRent);
            OwnedBy.GiveMoney(CurrentRent);
            
            base.PayRent(player);
        }
    }
}