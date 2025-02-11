using Tiles;
using UnityEngine;

/// <summary>
/// Monobehaviour gameobject for the visual representation of the tile on the board
/// </summary>
public class TileUI : MonoBehaviour
{
    [field: SerializeField] public Tile Tile { get; set; }
    
    private void OnMouseEnter()
    {
        Tile.ShowCard();
    }

    private void OnMouseExit()
    {
        Tile.HideCard();
    }
}