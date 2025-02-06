[System.Serializable]
public class Property : Space
{
    public Player OwnedBy { get; set; } // initially owned by the bank, null can be the bank for now
    public int Cost { get; set; }
    public bool Mortgaged { get; set; }
    
    public Property(string name, int cost) : base(name)
    {
        Cost = cost;
    }

    public override void OnLanded(Player player)
    {
        if (Mortgaged || OwnedBy == player)
        {
            // do nothing
        }
        else if (OwnedBy != null)
        {
            PayRent(player);
            // player pays rent to OwnedBy
        }
        else
        {
            // buy for the Cost, or auction
        }
    }

    public virtual void PayRent(Player player)
    {
    }
}