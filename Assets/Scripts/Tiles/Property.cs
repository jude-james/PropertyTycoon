using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Inherits from tile class, describes a purchasable tiles that can be owned by a player or the bank
    /// </summary>
    //[System.Serializable]
    public class Property : Tile
    {
        [SerializeField] protected Player ownedBy; // initially owned by the bank, null can be the bank for now
        [SerializeField] protected int cost;
        [SerializeField] protected bool mortgaged;

        [SerializeField] private GameObject mortgagedCard; // Each property can be turned over to see the mortgage into

        protected int CurrentRent; // Although each property manages rent differently, they all still have a current rent value
        
        protected void SetUp(string name, int cost)
        {
            this.cost = cost;
            base.SetUp(name);
        }
        
        protected override void SetBoardTile()
        {
            base.SetBoardTile();
            if (transform.childCount > 0)
            {
                var cost = transform.GetChild(1).GetComponent<TMP_Text>();
                cost.SetText("£"+this.cost);
            }
        }
        
        public override void OnLanded(Player player)
        {
            // Structure for what I think the code might end up looking like, this is mostly temporary
            if (mortgaged || ownedBy == player)
            {
                // do nothing
            }
            else if (ownedBy != null)
            {
                PayRent(player);
                // player pays rent to OwnedBy
            }
            else
            {
                // player buy for the Cost, or auction
            }
        }
        
        /// <summary>
        /// Handles paying rent to the owner of this property. This functionality is specific to properties
        /// </summary>
        /// <param name="player"> The player that needs to pay rent to the owner </param>
        protected virtual void PayRent(Player player)
        {
        }
    }
}