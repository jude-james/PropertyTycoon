public class Station : Property
{
    public const int Rent1 = 25;
    public const int Rent2 = 50;
    public const int Rent3 = 100;
    public const int Rent4 = 200;
        
    public Station(string name, int cost) : base(name, cost)
    {
    }
        
    public override void OnLanded(Player player)
    {
    }
}