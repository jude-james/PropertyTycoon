using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tiles
{
    public class Utility : Property
    {
        public int Rent1 { get; set; }
        public int Rent2 { get; set; }
        public string UtilType { get; set; }
        
        public Utility(string name, int cost, int rent1, int rent2, string utilType) : base(name, cost)
        {
            Rent1 = rent1;
            Rent2 = rent2;
            UtilType = utilType;
        }

        public override void SetCard()
        {
            var prefabName = "UtilElec";
            if (UtilType == "Water")
            {
                prefabName = "UtilWater";
            }
            
            Card = Object.Instantiate(Resources.Load("Prefabs/" + prefabName)) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0).GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
            }
        }

        protected override void PayRent(Player player)
        {
            // figure out rent based on dice roll
            int amount = 0; // = player.diceRoll
            player.UpdateMoney(-amount); // temporary
            OwnedBy.UpdateMoney(amount);
        }
    }
}