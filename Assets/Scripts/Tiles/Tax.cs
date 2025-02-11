namespace Tiles
{
    public class Tax : Tile
    {
        private int _amount;
        
        public Tax(string name, int amount) : base(name)
        {
            _amount = amount;
        }

        public override void OnLanded(Player player)
        {
        }
    }
}