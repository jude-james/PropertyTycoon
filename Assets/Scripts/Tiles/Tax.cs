namespace Tiles
{
    public class Tax : Tile
    {
        private int _amount;
        
        public void SetUp(string name, int amount)
        {
            _amount = amount;
            base.SetUp(name);
        }
        
        public override void OnLanded(Player player)
        {
            // TODO override setCard so tax has a card, then show the card for X amount of seconds
            player.TakeMoney(_amount);
            player.CompleteTurn();
        }
    }
}