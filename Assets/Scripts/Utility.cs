public class Utility : Property
{
    public Utility(string name, int cost) : base(name, cost)
    {
    }
    
    public override void PayRent(Player player)
    {
        // figure out rent based on dice roll
        int amount = 0; // = player.diceRoll
        player.UpdateMoney(-amount); // temporary
        OwnedBy.UpdateMoney(amount);
    }
}