namespace Tiles
{
    public class ActionCard : Tile
    {
        private string _cardType;
        
        public ActionCard(string name, string cardType) : base(name)
        {
            _cardType = cardType;
        }

        public override void OnLanded(Player player)
        {
        }
    }
}