using TMPro;
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
        
        [field: SerializeField] public GameObject Card { get; protected set; }
        [field: SerializeField] public TileUI TileUI { get; set; }

        public Vector2 Position => TileUI.transform.position;

        public Tile(string name)
        {
            Name = name;
        }
        
        /// <summary>
        /// Sets the visual tile card from a prefab depending on the type of the tile
        /// </summary>
        public virtual void SetCard()
        {
        }

        /// <summary>
        /// Sets the visual name/cost of this tile on the board
        /// </summary>
        public virtual void SetTileUI()
        {
            if (TileUI.transform.childCount > 0)
            {
                var nameText = TileUI.transform.GetChild(0).GetComponent<TMP_Text>();
                nameText.SetText(Name);
            }
        }

        public void ShowCard()
        {
            if (Card != null)
            {
                Card.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        public void HideCard()
        {
            if (Card != null)
            {
                Card.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Handles functionality for player landing on this tile
        /// </summary>
        /// <param name="player"> The player that landed on the tile </param>
        public virtual void OnLanded(Player player)
        {
        }
    }
}