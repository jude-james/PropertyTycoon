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
        public Player OwnedBy { get; set; } // initially owned by the bank, null can be the bank for now
        public int Cost { get; private set; }
        public int PropertyNumber { get; private set; }
        protected int CurrentRent;
        
        protected bool Mortgaged;
        private GameObject _mortgagedCard; // Each property can be turned over to see the mortgage into
        
        private readonly WaitForSeconds _payRentPopupTime = new(3);

        protected void SetUp(string name, int cost, int propertyNumber)
        {
            Cost = cost;
            PropertyNumber = propertyNumber;
            base.SetUp(name);
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
        
        public override void OnLanded(Player player)
        {
            if (OwnedBy == null)
            {
                // if property is owned by bank, player can buy for the Cost, or auction
                player.ForSaleDecision(this);
            }
            else if (Mortgaged || OwnedBy == player || OwnedBy.InJail)
            {
                // If property is mortgaged, player is already the owner, or the owner is in jail do nothing
                player.CompleteTurn();
            }
            else
            {
                PayRent(player);
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
    }
}