using TMPro;
using UnityEngine;

namespace Tiles
{
    public class Station : Property
    {
        public int Rent1 { get; private set; }
        public int Rent2 { get; private set; }
        public int Rent3 { get; private set; }
        public int Rent4 { get; private set; }
        
        public Station(string name, int cost, int rent1, int rent2, int rent3, int rent4) : base(name, cost)
        {
            Rent1 = rent1;
            Rent2 = rent2;
            Rent3 = rent3;
            Rent4 = rent4;
        }

        public override void SetCard()
        {
            Card = Object.Instantiate(Resources.Load("Prefabs/Station")) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0).GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("£"+Rent1);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("£"+Rent2);
                cardSprite.GetChild(3).GetComponent<TMP_Text>().SetText("£"+Rent3);
                cardSprite.GetChild(4).GetComponent<TMP_Text>().SetText("£"+Rent4);
            }
        }

        protected override void PayRent(Player player)
        {
            // figure out rent based on how many stations OwnedBy owns
            player.UpdateMoney(-Rent1); // temporary
            OwnedBy.UpdateMoney(Rent1);
        }
    }
}