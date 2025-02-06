namespace Tiles
{
    public class Station : Property
    {
        private const int Rent1 = 25;
        private const int Rent2 = 50;
        private const int Rent3 = 100;
        private const int Rent4 = 200;
        
        public Station(string name, int cost) : base(name, cost)
        {
        }
        
        protected override void PayRent(Player player)
        {
            // figure out rent based on how many stations OwnedBy owns
            player.UpdateMoney(-Rent1); // temporary
            OwnedBy.UpdateMoney(Rent1);
        }
    }
}