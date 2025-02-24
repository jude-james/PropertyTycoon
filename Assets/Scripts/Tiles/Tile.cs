using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Tile is the base class that all squares on the board derive from 
    /// </summary>
    public class Tile : MonoBehaviour
    {
        protected string Name { get; private set; }
        public GameObject Card { get; protected set; }
        
        [SerializeField] private Vector2 position;

        public void SetUp(string name)
        {
            Name = name;
            SetCard();
            SetBoardTile();
        }
        
        /// <summary>
        /// Handles functionality for player landing on this tile
        /// </summary>
        /// <param name="player"> The player that landed on the tile </param>
        public virtual void OnLanded(Player player)
        {
            player.CompleteTurn();
        }
        
        /// <summary>
        /// Sets the visual tile card from a prefab depending on the type of the tile
        /// </summary>
        protected virtual void SetCard()
        {
        }

        /// <summary>
        /// Sets the name/cost text of this tile on the board
        /// </summary>
        protected virtual void SetBoardTile()
        {
            if (transform.childCount > 0)
            {
                var nameText = transform.GetChild(0).GetComponent<TMP_Text>();
                nameText.SetText(Name);
            }
        }

        private void ShowCard()
        {
            if (Card != null)
            {
                Card.SetActive(true);
            }
        }

        private void HideCard()
        {
            if (Card != null)
            {
                Card.SetActive(false);
            }
        }

        private void OnMouseEnter()
        {
            ShowCard();
        }

        private void OnMouseExit()
        {
            HideCard();
        }
        
        public void setPosition(float x,float y)
        {
            position = new Vector2(x,y);
        }

        public Vector2 getPosition()
        {
            return position;
        }
    }
}