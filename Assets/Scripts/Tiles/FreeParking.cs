namespace Tiles
{
    public class FreeParking : Tile
    {
        public override void OnLanded(Player player)
        {
            player.GiveMoney(Board.Instance.FreeParkingSum);
            Board.Instance.FreeParkingSum = 0;
            player.CompleteTurn();
        }
    }
}