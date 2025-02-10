using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Token Token { get; set; }
    [field: SerializeField] public int Money { get; set; } = 1500;
    [field: SerializeField] public List<Property> TitleDeeds { get; set; }
    [field: SerializeField] public Space CurrentSpace { get; set; }
    [field: SerializeField] public int GetOutOfJailFreeCards { get; set; }
    [field: SerializeField] public bool InJail { get; set; }

    [field: SerializeField] public SpriteRenderer spriteR { get; set; }
    




    
    public int Houses { get; set; }
    public int Hotels { get; set; }
    
    private int _currentSpaceIndex;
    
    public int Move(int diceValue)
    {
        _currentSpaceIndex = _currentSpaceIndex + diceValue;
        return _currentSpaceIndex;
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

    
    public void setPosition(Vector2 position)
    {
        transform.position = position;
    }

    //Set Player sprite, also makes the sprite go above the board
    public void setSprite(Sprite sprite)
    {
        transform.AddComponent<SpriteRenderer>();
        spriteR = transform.GetComponent<SpriteRenderer>();
        spriteR.sortingOrder = 1;
        spriteR.sprite = sprite;
    }
}


public enum Token
{
    Boot, Smartphone, Ship, HatStand, Hat, Iron 
}