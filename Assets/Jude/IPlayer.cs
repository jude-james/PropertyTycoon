using System.Collections.Generic;

namespace Jude
{
    public interface IPlayer
    {
        public string Name { get; set; }
        public int Money { get; set; }
        public List<Property> OwnedProperty { get; set; }
        public Space CurrentSpace { get; set; }
        
        public void StartTurn(); // Temporary, will figure out state machine

        // when in player turn state:
        // player can roll, mortgage, sell, build
        // once rolled and moving squares is finished, player can still mortgage, sell build, until player chooses end turn option
    }
}