namespace Tiles
{
    /// <summary>
    /// Type of property that belongs to a colour set, and can have houses and hotels built
    /// </summary>
    public class Site : Property
    {
        public string Set { get; private set; } // will use string for now, might change to enum
        public int InitialRent { get; private set; }
        public int[] ImprovedRent { get; private set; }
        public int HouseHotelCost { get; private set; }

        public int CurrentHouses { get; set; }
        public int CurrentHotels { get; set; }
        
        public Site(string name, int cost, string set, int initialRent, int[] improvedRent, int houseHotelCost) : base(name, cost)
        {
            Set = set;
            InitialRent = initialRent;
            ImprovedRent = improvedRent;
            HouseHotelCost = houseHotelCost;
        }
    
        protected override void PayRent(Player player)
        {
            // figure out rent based on houses, hotels and if OwnedBy also owns the set
            player.UpdateMoney(-InitialRent); // temporary
            OwnedBy.UpdateMoney(InitialRent);
        }
    }
    
    public enum Set { Brown, Blue, Purple, Orange, Red, Yellow, Green, DeepBlue }
}