using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;

/// <summary>
/// Playable gameObject throughout the game, can either be a human or a bot
/// </summary>
public class Player : MonoBehaviour
{
    private static Player _activePlayer;

    [field: SerializeField] public string Name { get; set; }
    [SerializeField] private int money = 1500;
    [SerializeField] private List<Property> titleDeeds;
    [field: SerializeField] public Tile CurrentTile { get; set; }
    [SerializeField] private int getOutOfJailFreeCards;
    [SerializeField] private bool inJail;
    public int DiceRoll { get; private set; }

    private Sprite _sprite;
    
    private bool _rolledADouble;
    private int _doubleCount;
    
    private int _houses;
    private int _hotels;
    
    private int _currentTileIndex;
    
    private const int MoveSpeed = 10;
    
    private void Start()
    {
        UIManager.Instance.rollDiceButton.onClick.AddListener(OnRollDice);
        UIManager.Instance.endTurnButton.onClick.AddListener(OnEndTurn);
        
        // TODO Assign PlayerInfoPanel Here
    }

    public void StartTurn()
    {
        _activePlayer = this;

        UIManager.Instance.SetActivePlayerInfo(Name, _sprite);
        
        Debug.Log(Name + " has started their turn and is now the active player");

        // Decide here which buttons to gray out or which UI elements to pop up, e.g. if they are in jail
        if (inJail)
        {
            // show specific in jail options
        }
        else
        {
            // set roll dice panel active
            UIManager.Instance.rollDicePanel.SetActive(true);
        }
    }

    private void OnRollDice() // Bot would call this automatically instead of it being assigned to a button
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.rollDicePanel.SetActive(false);

        RollDice();
        Debug.Log(Name + " rolled a " + DiceRoll);
        // TODO Animate dice roll here
        
        StartCoroutine(MoveToTile());
    }
    
    private void OnEndTurn()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.endTurnPanel.SetActive(false);
        Board.Instance.EndTurn();
    } 
    
    private IEnumerator MoveToTile()
    {
        for (int i = _currentTileIndex; i <= _currentTileIndex + DiceRoll; i++)
        {
            yield return StartCoroutine(MoveBetweenPositions(Board.Instance.Tiles[i % Board.Instance.Tiles.Count].transform.position));
            yield return new WaitForSeconds(0.1f); 
        }

        LandOnTile();
    }
    
    private IEnumerator MoveBetweenPositions(Vector2 targetPosition)
    {
        var distance = Vector2.Distance(transform.position, targetPosition);

        while (distance > Mathf.Epsilon) 
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            distance = Vector2.Distance(transform.position, targetPosition);
            yield return null;
        }
    }
    
    private void LandOnTile()
    {
        _currentTileIndex = (_currentTileIndex + DiceRoll) % Board.Instance.Tiles.Count;
        CurrentTile = Board.Instance.Tiles[_currentTileIndex];
        
        CurrentTile.OnLanded(this);
        
        UIManager.Instance.endTurnPanel.SetActive(true);
    }
    
    private void RollDice()
    {
        var diceRoll1 = Random.Range(1, 7);
        var diceRoll2 = Random.Range(1, 7);
        DiceRoll = diceRoll1 + diceRoll2;
    }
    
    public void GiveMoney(int amount)
    {
        money += amount;
        // TODO update UI money text, with event?
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        // TODO update UI money text 

        if (money <= 0)
        {
            Debug.Log("Mortgage or go bankrupt");
        }
    }

    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
        GetComponent<SpriteRenderer>().sprite = _sprite;
    }
}