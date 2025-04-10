using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Type of property that belongs to a colour set, and can have houses and hotels built
    /// </summary>
    public class Street : Property
    {
        // public string Set { get; private set; }
        public Set Set { get; private set; }
        private int _initialRent;
        private int _rentWithColourSet;
        private int[] _improvedRent;
        public int HouseCost { get; private set; }
        public int HotelCost { get; private set; }

        public int CurrentHouses { get; private set; }
        public int CurrentHotels { get; private set; }

        private GameObject[] _houseSprites;
        private GameObject _hotelSprite;
        
        public void SetUp(string name, int cost, int propertyNumber, Set set, int initialRent, int rentWithColourSet, int[] improvedRent, int houseCost, int hotelCost)
        {
            Set = set;
            _initialRent = initialRent;
            _rentWithColourSet = rentWithColourSet;
            _improvedRent = improvedRent;
            HouseCost = houseCost;
            HotelCost = hotelCost;
            CurrentRent = _initialRent;
            GetBuildingSprites();
            base.SetUp(name, cost, propertyNumber);
        }

        /// <summary>
        /// Gets the house and hotel sprites on this tile
        /// </summary>
        private void GetBuildingSprites()
        {
            _houseSprites = new GameObject[4];
            
            for (var i = 0; i < _houseSprites.Length; i++)
            {
                _houseSprites[i] = transform.GetChild(2 + i).gameObject;
            }

            _hotelSprite = transform.GetChild(6).gameObject;
        }
        
        protected override void SetCard()
        {
            Card = Instantiate(Resources.Load("Prefabs/Cards/" + Set)) as GameObject;
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
                cardSprite.GetChild(8).GetComponent<TMP_Text>().SetText("£"+HouseCost + " each");
                cardSprite.GetChild(9).GetComponent<TMP_Text>().SetText("£"+HotelCost + " each");
            }
        }
        
        protected override void PayRent(Player player)
        {
            if (HasNoBuildings())
            {
                var count = OwnedBy.TitleDeeds.OfType<Street>().Count(street => street.Set == Set);
                if (Set is Set.Brown or Set.DeepBlue)
                {
                    if (count == 2)
                    {
                        CurrentRent = _rentWithColourSet;
                    }
                }
                else if (count == 3)
                {
                    CurrentRent = _rentWithColourSet;
                }
                else
                {
                    CurrentRent = _initialRent;
                }
            }
            else
            {
                if (HasMaxBuildings())
                {
                    CurrentRent = _improvedRent[4];
                }
                else
                {
                    CurrentRent = _improvedRent[CurrentHouses - 1];
                }
            }
            
            player.TakeMoney(CurrentRent);
            OwnedBy.GiveMoney(CurrentRent);
            
            base.PayRent(player);
        }

        /// <summary>
        /// Builds a house or hotel on this street
        /// </summary>
        public void Build()
        {
            if (CurrentHouses < 4)
            {
                CurrentHouses++;
                for (var i = 0; i < CurrentHouses; i++)
                {
                    _houseSprites[i].SetActive(true);
                }
                OwnedBy.TakeMoney(HouseCost);
            }
            else
            {
                CurrentHotels++;
                for (var i = 0; i < _houseSprites.Length; i++)
                {
                    _houseSprites[i].SetActive(false);
                }
                _hotelSprite.SetActive(true);
                OwnedBy.TakeMoney(HotelCost);
                
                HideOutline();
                InBuildSelection = false;
            }
            
            // Once player builds a house, check they have enough money to build other houses
            foreach (var street in OwnedBy.TitleDeeds.OfType<Street>())
            {
                if (OwnedBy.Money < street.HouseCost)
                {
                    street.HideOutline();
                    street.InBuildSelection = false;
                }
            }
        }

        /// <summary>
        /// Sells a house or hotel on this street back to the bank
        /// </summary>
        public void SellBuilding()
        {
            if (HasMaxBuildings())
            {
                CurrentHotels--;
                for (var i = 0; i < _houseSprites.Length; i++)
                {
                    _houseSprites[i].SetActive(true);
                }
                _hotelSprite.SetActive(false);
                
                OwnedBy.GiveMoney(HotelCost);
            }
            else if (CurrentHouses > 0)
            {
                CurrentHouses--;
                for (var i = 3; i >= CurrentHouses; i--)
                {
                    _houseSprites[i].SetActive(false);
                }
                
                OwnedBy.GiveMoney(HouseCost);

                if (CurrentHouses == 0)
                {
                    HideOutline();
                    InSellBuildingsSelection = false;
                }
            }
        }
        
        /// <summary>
        /// Gets the value of all houses and hotels on this street
        /// </summary>
        public int GetBuildingValue()
        {
            if (HasMaxBuildings())
            {
                return CurrentHotels * HotelCost;
            }
            
            return CurrentHouses * HouseCost;
        }

        public bool HasMaxBuildings()
        {
            return CurrentHotels > 0;
        }

        public bool HasNoBuildings()
        {
            return CurrentHouses == 0 && CurrentHotels == 0;
        }

        protected override void OnMouseDown()
        {
            base.OnMouseDown();
            
            if (InBuildSelection)
            {
                Build();
            }

            if (InSellBuildingsSelection)
            {
                SellBuilding();
            }
        }
    }
}