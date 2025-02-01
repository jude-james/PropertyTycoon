namespace Jude
{
    /// <summary>
    /// Space is the base class that all squares on the board derive from 
    /// </summary>
    public class Space
    {
        private string _name;
        
        public Space(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
    }
}