using System;
using TMPro;
using UnityEngine;

namespace Tiles
{
    /// <summary>
    /// Tile is the base class that all squares on the board derive from 
    /// </summary>
    public class Tile : MonoBehaviour
    {
        public string Name { get; protected set; }
        public GameObject Card { get; protected set; }
        
        public bool InMortgageSelection { get; set; }
        public bool InUnmortgageSelection { get; set; }

        private BoxCollider2D _boxCollider;
        private LineRenderer _lineRenderer;

        private Color _lineStartColour;
        private Color _lineEndColour;
        
        private void Start()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _lineRenderer = GetComponent<LineRenderer>();

            _lineStartColour = _lineRenderer.startColor;
            _lineEndColour = _lineRenderer.endColor;
            
            Outline();
        }

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

        /// <summary>
        /// Draws an outline using the tiles box collider
        /// </summary>
        private void Outline()
        {
            _lineRenderer.startWidth = 0.15f;
            _lineRenderer.endWidth = 0.15f;
            _lineRenderer.positionCount = 4;
            _lineRenderer.loop = true;
            _lineRenderer.numCornerVertices = 5;
            
            var corners = new Vector3[4];
            corners[0] = _boxCollider.offset + new Vector2(-_boxCollider.size.x, -_boxCollider.size.y) * 0.5f;
            corners[1] = _boxCollider.offset + new Vector2(-_boxCollider.size.x, _boxCollider.size.y) * 0.5f;
            corners[2] = _boxCollider.offset + new Vector2(_boxCollider.size.x, _boxCollider.size.y) * 0.5f;
            corners[3] = _boxCollider.offset + new Vector2(_boxCollider.size.x, -_boxCollider.size.y) * 0.5f;
            
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = transform.TransformPoint(corners[i]);
            }

            _lineRenderer.SetPositions(corners);
        }

        protected virtual void ShowCard()
        {
            if (Card != null)
            {
                Card.SetActive(true);
            }
        }

        protected virtual void HideCard()
        {
            if (Card != null)
            {
                Card.SetActive(false);
            }
        }

        public void ShowOutline()
        {
            _lineRenderer.startColor = _lineStartColour;
            _lineRenderer.endColor = _lineEndColour;
            _lineRenderer.enabled = true;
        }
        
        public void ShowOutline(Color startColor, Color endColour)
        {
            _lineRenderer.startColor = startColor;
            _lineRenderer.endColor = endColour;
            _lineRenderer.enabled = true;
        }

        public void HideOutline()
        {
            _lineRenderer.enabled = false;
        }
        
        protected virtual void OnMouseEnter()
        {
            ShowCard();

            if (!InMortgageSelection && !InUnmortgageSelection)
            {
                ShowOutline();
            }
        }

        protected virtual void OnMouseExit()
        {
            HideCard();

            if (!InMortgageSelection && !InUnmortgageSelection)
            {
                HideOutline();
            }
        }
    }
}