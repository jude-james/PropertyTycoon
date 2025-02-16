using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Type of property that belongs to a colour set, and can have houses and hotels built
    /// </summary>
    public class Street : Property
    {
        [SerializeField] private string set;
        [SerializeField] private int initialRent;
        [SerializeField] private int rentWithColourSet;
        [SerializeField] private int[] improvedRent;
        [SerializeField] private int houseCost;
        [SerializeField] private int hotelCost;

        private int _currentHouses;
        private int _currentHotels;
        
        public void SetUp(string name, int cost, string set, int initialRent, int rentWithColourSet, int[] improvedRent, int houseCost, int hotelCost)
        {
            this.set = set;
            this.initialRent = initialRent;
            this.rentWithColourSet = rentWithColourSet;
            this.improvedRent = improvedRent;
            this.houseCost = houseCost;
            this.hotelCost = hotelCost;
            base.SetUp(name, cost);
        }

        protected override void SetCard()
        {
            card = Instantiate(Resources.Load("Prefabs/Cards/" + set)) as GameObject;
            if (card != null)
            {
                var cardSprite = card.transform.GetChild(0).GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("£"+initialRent);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("£"+rentWithColourSet);
                for (int i = 0; i < improvedRent.Length; i++)
                {
                    cardSprite.GetChild(3 + i).GetComponent<TMP_Text>().SetText("£"+improvedRent[i]);
                }
                cardSprite.GetChild(8).GetComponent<TMP_Text>().SetText("£"+houseCost + " each");
                cardSprite.GetChild(9).GetComponent<TMP_Text>().SetText("£"+hotelCost + " each");
            }
        }

        protected override void PayRent(Player player)
        {
            // TODO figure out rent based on houses, hotels and if OwnedBy owns the set
            player.TakeMoney(CurrentRent);
            ownedBy.GiveMoney(CurrentRent);
        }
    }
}