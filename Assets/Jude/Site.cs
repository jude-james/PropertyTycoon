namespace Jude
{
    public class Site : Property
    {
        public string Set { get; set; } // will use string for now, might change to enum
        public int InitialRent { get; set; }
        public int[] ImprovedRent { get; set; }
        public int HouseHotelCost { get; set; }

        public int CurrentHouses { get; set; }
        public int CurrentHotels { get; set; }
        
        public Site(string name, int cost, string set, int initialRent, int[] improvedRent, int houseHotelCost) : base(name, cost)
        {
            Set = set;
            InitialRent = initialRent;
            ImprovedRent = improvedRent;
            HouseHotelCost = houseHotelCost;
        }
        
        public override void OnLanded(Player player)
        {
        }
    }
    
    public enum Set { Brown, Blue, Purple, Orange, Red, Yellow, Green, DeepBlue }
}