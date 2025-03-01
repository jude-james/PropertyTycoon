using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Type of property that belongs to a colour set, and can have houses and hotels built
    /// </summary>
    public class Street : Property
    {
        private string _set;
        private int _initialRent;
        private int _rentWithColourSet;
        private int[] _improvedRent;
        private int _houseCost;
        private int _hotelCost;

        private int _currentHouses;
        private int _currentHotels;
        
        public void SetUp(string name, int cost, string set, int initialRent, int rentWithColourSet, int[] improvedRent, int houseCost, int hotelCost)
        {
            _set = set;
            _initialRent = initialRent;
            _rentWithColourSet = rentWithColourSet;
            _improvedRent = improvedRent;
            _houseCost = houseCost;
            _hotelCost = hotelCost;
            CurrentRent = _initialRent;
            base.SetUp(name, cost);
        }

        protected override void SetCard()
        {
            Card = Instantiate(Resources.Load("Prefabs/Cards/" + _set)) as GameObject;
            if (Card != null)
            {
                var cardSprite = Card.transform.GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("£"+_initialRent);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("£"+_rentWithColourSet);
                for (int i = 0; i < _improvedRent.Length; i++)
                {
                    cardSprite.GetChild(3 + i).GetComponent<TMP_Text>().SetText("£"+_improvedRent[i]);
                }
                cardSprite.GetChild(8).GetComponent<TMP_Text>().SetText("£"+_houseCost + " each");
                cardSprite.GetChild(9).GetComponent<TMP_Text>().SetText("£"+_hotelCost + " each");
            }
        }

        protected override void PayRent(Player player)
        {
            // TODO figure out rent based on houses, hotels and if OwnedBy owns the set
            player.TakeMoney(CurrentRent);
            OwnedBy.GiveMoney(CurrentRent);
            
            base.PayRent(player);
        }
    }
}