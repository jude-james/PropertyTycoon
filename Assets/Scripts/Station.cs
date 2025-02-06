public class Station : Property
{
    public const int Rent1 = 25;
    public const int Rent2 = 50;
    public const int Rent3 = 100;
    public const int Rent4 = 200;
        
    public Station(string name, int cost) : base(name, cost)
    {
    }
        
    public override void PayRent(Player player)
    {
        // figure out rent based on how many stations OwnedBy owns
        player.UpdateMoney(-Rent1); // temporary
        OwnedBy.UpdateMoney(Rent1);
    }
}