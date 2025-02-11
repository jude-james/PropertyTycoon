using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Inherits from tile class, describes a purchasable tiles that can be owned by a player or the bank
    /// </summary>
    [System.Serializable]
    public class Property : Tile
    {
        public Player OwnedBy { get; set; } // initially owned by the bank, null can be the bank for now
        public int Cost { get; private set; }
        public bool Mortgaged { get; private set; }
        
        [field: SerializeField] public GameObject MortgagedCard { get; private set; } // Each property can be turned over to see the mortgage into

        public Property(string name, int cost) : base(name)
        {
            Cost = cost;
        }

        public override void SetTileUI()
        {
            base.SetTileUI();
            if (TileUI.transform.childCount > 0)
            {
                var cost = TileUI.transform.GetChild(1).GetComponent<TMP_Text>();
                cost.SetText("£"+Cost);
            }
        }
        
        public override void OnLanded(Player player)
        {
            // Structure for what I think the code might end up looking like, this is mostly temporary
            if (Mortgaged || OwnedBy == player)
            {
                // do nothing
            }
            else if (OwnedBy != null)
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
        /// Handles paying rent to the owner of this property. This functionality is specific to properties and is
        /// overridden for stations, utilities and sites
        /// </summary>
        /// <param name="player"> The player that needs to pay rent to the owner </param>
        protected virtual void PayRent(Player player)
        {
        }
    }
}