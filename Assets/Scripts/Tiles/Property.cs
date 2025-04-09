using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Inherits from tile class, describes a purchasable tile that can be owned by a player or the bank
    /// </summary>
    //[System.Serializable]
    public class Property : Tile
    {
        public Player OwnedBy { get; set; }
        public int Cost { get; private set; }
        public int PropertyNumber { get; private set; }
        protected int CurrentRent;
        
        public bool Mortgaged { get; set; }
        public int MortgagedValue { get; private set; }
        public int UnmortgagedValue { get; private set; }
        
        private GameObject _mortgagedCard;
        
        private readonly WaitForSeconds _payRentPopupTime = new(3);
        
        protected void SetUp(string name, int cost, int propertyNumber)
        {
            Cost = cost;
            PropertyNumber = propertyNumber;
            MortgagedValue = cost / 2;
            UnmortgagedValue = (int) (MortgagedValue * 1.1f);
            Name = name;
            SetMortgagedCard();
            base.SetUp(name);
        }

        private void SetMortgagedCard()
        {
            _mortgagedCard = Instantiate(Resources.Load("Prefabs/Cards/Mortgaged")) as GameObject;
            if (_mortgagedCard != null)
            {
                var cardSprite = _mortgagedCard.transform.GetChild(0);
                cardSprite.GetChild(0).GetComponent<TMP_Text>().SetText(Name);
                cardSprite.GetChild(1).GetComponent<TMP_Text>().SetText("MORTGAGE VALUE £" + MortgagedValue);
                cardSprite.GetChild(2).GetComponent<TMP_Text>().SetText("TO UNMORTGAGE, PAY £" + UnmortgagedValue);
            }
        }
        
        protected override void SetBoardTile()
        {
            base.SetBoardTile();
            if (transform.childCount > 0)
            {
                var costText = transform.GetChild(1).GetComponent<TMP_Text>();
                costText.SetText("£"+Cost);
            }
        }

        protected override void ShowCard()
        {
            if (Mortgaged)
            {
                if (_mortgagedCard != null) _mortgagedCard.SetActive(true);
            }
            else
            {
                if (Card != null) Card.SetActive(true);
            }
        }
        
        protected override void HideCard()
        {
            if (Mortgaged)
            {
                if (_mortgagedCard != null) _mortgagedCard.SetActive(false);
            }
            else
            {
                if (Card != null) Card.SetActive(false);
            }
        }
        
        public override void OnLanded(Player player)
        {
            if (OwnedBy != null) 
            {
                if (Mortgaged || OwnedBy == player || OwnedBy.InJail)
                {
                    player.CompleteTurn();
                }
                else
                {
                    PayRent(player);
                }
            }
            else if (player.PassedGo)
            {
                // if in the rare case the player can't auction or buy the property,
                // don't show the prompt at all because neither button will be available
                if (!Board.Instance.CanAuction() && player.Money < Cost)
                {
                    player.CompleteTurn();
                }
                else
                {
                    player.ForSaleDecision(this);
                }
            }
            else
            {
                player.CompleteTurn();
            }
        }
        
        /// <summary>
        /// Handles paying rent to the owner of this property. This functionality is specific to properties
        /// </summary>
        /// <param name="player"> The player that needs to pay rent to the owner </param>
        protected virtual void PayRent(Player player)
        {
            StartCoroutine(PayRentCoroutine(player));
        }

        private IEnumerator PayRentCoroutine(Player player)
        {
            UIManager.Instance.ShowPayRentPopup(CurrentRent, OwnedBy.Name, OwnedBy.Sprite);
            yield return _payRentPopupTime;
            UIManager.Instance.HidePayRentPopup();
            player.CompleteTurn();
        }

        /// <summary>
        /// Mortgages this property
        /// </summary>
        public void Mortgage()
        {
            Mortgaged = true;
            OwnedBy.GiveMoney(MortgagedValue);
            
            HideOutline();
            Card.SetActive(false);
            InMortgageSelection = false;
        }

        /// <summary>
        /// Unmortgages this property
        /// </summary>
        private void Unmortgage()
        {
            Mortgaged = false;
            OwnedBy.TakeMoney(UnmortgagedValue);
            
            HideOutline();
            _mortgagedCard.SetActive(false);
            InUnmortgageSelection = false;
            
            // Once player clicks unmortgage, check the player can afford to unmortgage the other properties
            foreach (var property in OwnedBy.TitleDeeds)
            {
                if (property != null && property.Mortgaged && OwnedBy.Money < property.UnmortgagedValue)
                {
                    property.HideOutline();
                    property.InUnmortgageSelection = false;
                }
            }
        }

        /// <summary>
        /// Sells this property to the bank
        /// </summary>
        public void SellProperty()
        {
            OwnedBy.GiveMoney(Mortgaged ? MortgagedValue : Cost);
            
            Board.Instance.GiveTitleDeed(this);
            OwnedBy.TakeTitleDeed(this);

            OwnedBy = null;
            
            HideOutline();
            InSellPropertySelection = false;
        }
        
        protected virtual void OnMouseDown()
        {
            if (InMortgageSelection)
            {
                Mortgage();
            }

            if (InUnmortgageSelection)
            {
                Unmortgage();
            }

            if (InSellPropertySelection)
            {
                SellProperty();
            }
        }
    }
}