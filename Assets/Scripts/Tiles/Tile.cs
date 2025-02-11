using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Tile is the base class that all squares on the board derive from 
    /// </summary>
    public class Tile : MonoBehaviour
    {
        [SerializeField] protected string name;
        [SerializeField] protected GameObject card;
        
        public void SetUp(string name)
        {
            this.name = name;
            SetCard();
            SetBoardTile();
        }
        
        /// <summary>
        /// Handles functionality for player landing on this tile
        /// </summary>
        /// <param name="player"> The player that landed on the tile </param>
        public virtual void OnLanded(Player player)
        {
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
                nameText.SetText(name);
            }
        }

        private void ShowCard()
        {
            if (card != null)
            {
                card.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        private void HideCard()
        {
            if (card != null)
            {
                card.transform.GetChild(0).gameObject.SetActive(false);
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
    }
}