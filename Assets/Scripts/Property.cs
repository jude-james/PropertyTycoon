[System.Serializable]
public class Property : Space
{
    public Player OwnedBy { get; set; } // initially owned by the bank, null can be the bank for now
    public int Cost { get; set; }
        
    public Property(string name, int cost) : base(name)
    {
        Cost = cost;
    }
}