using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tiles
{
    public class Utility : Property
    {
        [SerializeField] private int rent1;
        [SerializeField] private int rent2;
        [SerializeField] private string utilType;
        
        public void SetUp(string name, int cost, int rent1, int rent2, string utilType)
        {
            this.rent1 = rent1;
            this.rent2 = rent2;
            this.utilType = utilType;
            base.SetUp(name, cost);
        }

        protected override void SetCard()
        {
            var prefabName = "UtilElec";
            if (utilType == "Water")
            {
                prefabName = "UtilWater";
            }
            
            card = Instantiate(Resources.Load("Prefabs/" + prefabName)) as GameObject;
            if (card != null)
            {
                var cardSprite = card.transform.GetChild(0).GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(name);
            }
        }

        protected override void PayRent(Player player)
        {
            // figure out rent based on dice roll
            int amount = 0; // = player.diceRoll
            player.UpdateMoney(-amount); // temporary
            ownedBy.UpdateMoney(amount);
        }
    }
}