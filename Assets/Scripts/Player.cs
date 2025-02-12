using System.Collections.Generic;
using Tiles;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Playable gameObject throughout the game, can either be a human or a bot
/// </summary>
public class Player : MonoBehaviour
{
    [field: SerializeField] public string Name { get; set; }
    [SerializeField] private Sprite token;
    [SerializeField] private int money = 1500;
    [SerializeField] private List<Property> titleDeeds;
    [field: SerializeField] public Tile CurrentTile { get; set; }
    [SerializeField] private int getOutOfJailFreeCards;
    [SerializeField] private bool inJail;

    [field: SerializeField] public SpriteRenderer spriteR { get; set; }

    private int _houses;
    private int _hotels;
    
    private int _currentTileIndex;
    
    public int Move(int diceValue)
    {
        _currentTileIndex = _currentTileIndex + diceValue;
        return _currentTileIndex;
        // Returns position to allow board to handle finding the correct space and then update the UI
        // I think that the board will only need to know the players position after movement, but if not a GetSpace function will be added
    }

    public void UpdateMoney(int amount)
    {
        // If amount is negative, checks that there is money to take
        if (amount < 0 && money < (amount*-1))
        {
            // If they can't pay, player has to mortgage or go bankrupt
        }
        else
        {
            money += amount;
        }
    }

    public void setSprite(Sprite sprite)
    {
        transform.AddComponent<SpriteRenderer>();
        spriteR = transform.GetComponent<SpriteRenderer>();
        spriteR.sortingLayerName = "High";
        spriteR.sprite = sprite;
    }
}