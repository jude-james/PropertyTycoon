namespace Tiles
{
    public class Jail : Tile
    {
        public override void OnLanded(Player player)
        {
            player.CompleteTurn();
        }
    }
}