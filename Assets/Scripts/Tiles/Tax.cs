using UnityEngine;

namespace Tiles
{
    public class Tax : Tile
    {
        [SerializeField] private int amount;
        
        public void SetUp(string name, int amount)
        {
            this.amount = amount;
            base.SetUp(name);
        }
        
        public override void OnLanded(Player player)
        {
            // TODO override setCard so tax has a card, then show the card for X amount of seconds
            player.TakeMoney(amount);
        }
    }
}