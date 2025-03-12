using TMPro;
using UnityEngine;

namespace Tiles
{
    public class Station : Property
    {
        private int _rent1;
        private int _rent2;
        private int _rent3;
        private int _rent4;
        
        public void SetUp(string name, int cost, int propertyNumber, int rent1, int rent2, int rent3, int rent4)
        {
            _rent1 = rent1;
            _rent2 = rent2;
            _rent3 = rent3;
            _rent4 = rent4;
            CurrentRent = _rent1;
            base.SetUp(name, cost, propertyNumber);
        }

        protected override void SetCard()
        {
            Card = Instantiate(Resources.Load("Prefabs/Cards/Station")) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("£"+_rent1);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("£"+_rent2);
                cardSprite.GetChild(3).GetComponent<TMP_Text>().SetText("£"+_rent3);
                cardSprite.GetChild(4).GetComponent<TMP_Text>().SetText("£"+_rent4);
            }
        }

        protected override void PayRent(Player player)
        {
            // TODO figure out rent based on how many stations OwnedBy owns
            player.TakeMoney(CurrentRent);
            OwnedBy.GiveMoney(CurrentRent);
            
            base.PayRent(player);
        }
    }
}