namespace Jude
{
    /// <summary>
    /// Space is the base class that all squares on the board derive from 
    /// </summary>
    public class Space
    {
        public string Name { get; set; }
        
        public Space(string name)
        {
            Name = name;
        }

        public virtual void OnLanded(Player player)
        {
            // this might not be needed here since theres no default for on landed
        }
    }
}