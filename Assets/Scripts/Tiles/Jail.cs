namespace Tiles
{
    public class Jail : Tile
    {
        public Jail(string name) : base(name)
        {
        }

        public override void OnLanded(Player player)
        {
            // player goes to jail
        }
    }
}