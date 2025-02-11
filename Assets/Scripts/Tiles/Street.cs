using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tiles
{
    /// <summary>
    /// Type of property that belongs to a colour set, and can have houses and hotels built
    /// </summary>
    public class Street : Property
    {
        public string Set { get; private set; }
        public int InitialRent { get; private set; }
        public int RentWithColourSet { get; set; }
        public int[] ImprovedRent { get; private set; }
        public int HouseCost { get; private set; }
        public int HotelCost { get; private set; }

        public int CurrentHouses { get; set; }
        public int CurrentHotels { get; set; }
        
        public Street(string name, int cost, string set, int initialRent, int rentWithColourSet, int[] improvedRent, int houseCost, int hotelCost) : base(name, cost)
        {
            Set = set;
            InitialRent = initialRent;
            RentWithColourSet = rentWithColourSet;
            ImprovedRent = improvedRent;
            HouseCost = houseCost;
            HotelCost = hotelCost;
        }

        public override void SetCard()
        {
            Card = Object.Instantiate(Resources.Load("Prefabs/" + Set)) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0).GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("£"+InitialRent);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("£"+RentWithColourSet);
                for (int i = 0; i < ImprovedRent.Length; i++)
                {
                    cardSprite.GetChild(3 + i).GetComponent<TMP_Text>().SetText("£"+ImprovedRent[i]);
                }
                cardSprite.GetChild(8).GetComponent<TMP_Text>().SetText("£"+HouseCost + " each");
                cardSprite.GetChild(9).GetComponent<TMP_Text>().SetText("£"+HotelCost + " each");
            }
        }

        protected override void PayRent(Player player)
        {
            // figure out rent based on houses, hotels and if OwnedBy also owns the set
            player.UpdateMoney(-InitialRent); // temporary
            OwnedBy.UpdateMoney(InitialRent);
        }
    }
}