using System.Collections.Generic;
using Tiles;
using UnityEngine;

/// <summary>
/// Playable gameObject throughout the game, can either be a human or a bot
/// </summary>
public class Player : MonoBehaviour
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Token Token { get; private set; }
    [field: SerializeField] public int Money { get; private set; } = 1500;
    [field: SerializeField] public List<Property> TitleDeeds { get; private set; }
    [field: SerializeField] public Tile CurrentTile { get; set; }
    [field: SerializeField] public int GetOutOfJailFreeCards { get; set; }
    [field: SerializeField] public bool InJail { get; set; }
    
    public int Houses { get; set; }
    public int Hotels { get; set; }
    
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
        if (amount < 0 && Money < (amount*-1))
        {
            // If they can't pay, player has to mortgage or go bankrupt
        }
        else
        {
            Money += amount;
        }
    }
}

public enum Token
{
    Boot, Smartphone, Ship, HatStand, Hat, Iron 
}