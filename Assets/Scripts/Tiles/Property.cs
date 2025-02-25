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
        protected Player OwnedBy; // initially owned by the bank, null can be the bank for now
        public int Cost { get; private set; }
        protected bool Mortgaged;

        private GameObject _mortgagedCard; // Each property can be turned over to see the mortgage into

        protected int CurrentRent; // Although each property manages rent differently, they all still have a current rent value
        
        protected void SetUp(string name, int cost)
        {
            Cost = cost;
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
            if (Mortgaged || OwnedBy == player)
            {
                // do nothing
                player.CompleteTurn();
            }
            else if (OwnedBy != null)
            {
                // player pays rent to OwnedBy
                PayRent(player);
            }
            else
            {
                // player buy for the Cost, or auctions
                player.ForSaleDecision(this);
            }
        }
        
        /// <summary>
        /// Handles paying rent to the owner of this property. This functionality is specific to properties
        /// </summary>
        /// <param name="player"> The player that needs to pay rent to the owner </param>
        protected virtual void PayRent(Player player)
        {
            // TODO check if player is in jail first, or do this in the give money part, since i think
            // if they are in jail, there is no situation where they can get money
        }
    }
}