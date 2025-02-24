using TMPro;
using UnityEngine;

namespace Tiles
{
    public class Station : Property
    {
        [SerializeField] private int rent1;
        [SerializeField] private int rent2;
        [SerializeField] private int rent3;
        [SerializeField] private int rent4;
        
        public void SetUp(string name, int cost, int rent1, int rent2, int rent3, int rent4)
        {
            this.rent1 = rent1;
            this.rent2 = rent2;
            this.rent3 = rent3;
            this.rent4 = rent4;
            base.SetUp(name, cost);
        }

        protected override void SetCard()
        {
            Card = Instantiate(Resources.Load("Prefabs/Cards/Station")) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("£"+rent1);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("£"+rent2);
                cardSprite.GetChild(3).GetComponent<TMP_Text>().SetText("£"+rent3);
                cardSprite.GetChild(4).GetComponent<TMP_Text>().SetText("£"+rent4);
            }
        }

        protected override void PayRent(Player player)
        {
            // TODO figure out rent based on how many stations OwnedBy owns
            player.TakeMoney(CurrentRent);
            OwnedBy.GiveMoney(CurrentRent);
        }
    }
}