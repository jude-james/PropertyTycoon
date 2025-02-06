using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Tile is the base class that all squares on the board derive from 
    /// </summary>
    [System.Serializable]
    public class Tile
    {
        [field: SerializeField] public string Name { get; private set; }
        
        public Tile(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Handles functionality for player landing on this tile
        /// </summary>
        /// <param name="player"> The player that landed on the tile </param>
        public virtual void OnLanded(Player player)
        {
            // this might not be needed here since theres no default for on landed, e.g. free parking and GO do nothing
        }
    }
}