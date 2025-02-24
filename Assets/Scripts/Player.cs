using System;
using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Represents a playable game object throughout the game, which can either be a human or a bot.
/// This script handles player movement, dice rolling, and interactions with the game board.
/// </summary>
public class Player : MonoBehaviour
{
    private static Player _activePlayer;

    [SerializeField] private Animator animator;
    
    [field: SerializeField] public string Name { get; set; }
    [SerializeField] private int money = 1500;
    [SerializeField] private List<Property> titleDeeds;
    [field: SerializeField] public Tile CurrentTile { get; set; }
    [SerializeField] private int getOutOfJailFreeCards;
    [SerializeField] private bool inJail;
    
    private Sprite _sprite;

    public int DiceRoll { get; private set; }
    private int _diceRoll1;
    private int _diceRoll2;
    
    // TODO use these for rolling a double logic
    private bool _rolledADouble;
    private int _doubleCount;
    
    private int _houses;
    private int _hotels;
    
    private int _currentTileIndex;
    
    private const int MoveSpeed = 10;

    private readonly WaitForSeconds _reactionTime = new(0.5f);
    private readonly WaitForSeconds _pauseBetweenTileTime = new(0.1f);

    /// <summary>
    /// Initializes the player's UI event listeners and sets up the game object.
    /// </summary>
    private void Start()
    {
        UIManager.Instance.rollDiceButton.onClick.AddListener(OnRollDice);
        UIManager.Instance.endTurnButton.onClick.AddListener(OnEndTurn);
        
        // TODO Assign PlayerInfoPanel Here
    }

    /// <summary>
    /// Starts the player's turn, setting them as the active player and updating the UI.
    /// </summary>
    public void StartTurn()
    {
        _activePlayer = this;
        UIManager.Instance.SetActivePlayerInfo(Name, _sprite);
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
    /// <summary>
    /// Handles the rolling of dice, either by the player or automatically for a bot.
    /// </summary>
    private void OnRollDice() // Bot would call this automatically instead of it being assigned to a button
    {
        if (this != _activePlayer) return;
        UIManager.Instance.rollDicePanel.SetActive(false);
        _diceRoll1 = Random.Range(1, 7);
        _diceRoll2 = Random.Range(1, 7);
        DiceRoll = _diceRoll1 + _diceRoll2;
        StartCoroutine(AnimateDiceRoll());
    }

    /// <summary>
    /// Animates the dice roll and then moves the player to the new tile.
    /// </summary>
    private IEnumerator AnimateDiceRoll()
    {
        var diceRollTime = UIManager.Instance.AnimateDiceRoll(_diceRoll1, _diceRoll2);
        yield return diceRollTime;
        yield return _reactionTime;
        StartCoroutine(MoveToTile());
    }

    /// <summary>
    /// Moves the player to the new tile based on the dice roll.
    /// </summary>
    private IEnumerator MoveToTile()
    {
        animator.enabled = true;
        for (int i = _currentTileIndex; i <= _currentTileIndex + DiceRoll; i++)
        {
            yield return StartCoroutine(MoveBetweenPositions(Board.Instance.Tiles[i % Board.Instance.Tiles.Count].transform.position));
            if (i < _currentTileIndex + DiceRoll) // Don't pause between tiles if it's on the last tile
            {
                yield return _pauseBetweenTileTime;
            }
        }
        animator.enabled = false;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
        LandOnTile();
    }

    /// <summary>
    /// Smoothly moves the player between two positions.
    /// </summary>
    /// <param name="targetPosition">The target position to move to.</param>
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

    /// <summary>
    /// Handles the player landing on a new tile, updating their position and triggering tile-specific actions.
    /// </summary>
    private void LandOnTile()
    {
        _currentTileIndex = (_currentTileIndex + DiceRoll) % Board.Instance.Tiles.Count;
        CurrentTile = Board.Instance.Tiles[_currentTileIndex];
        CurrentTile.OnLanded(this);
        // move this line to tile class so tile controls when the round ends
        UIManager.Instance.endTurnPanel.SetActive(true);
    }

    /// <summary>
    /// Handles the player's turn ending, hiding the end turn panel and signaling the board to end the turn.
    /// </summary>
    private void OnEndTurn()
    {
        if (this != _activePlayer) return;
        UIManager.Instance.endTurnPanel.SetActive(false);
        Board.Instance.EndTurn();
    }

    /// <summary>
    /// Gives the player a specified amount of money.
    /// </summary>
    /// <param name="amount">The amount of money to give the player.</param>
    public void GiveMoney(int amount)
    {
        money += amount;
        // TODO update UI money text, with event?
    }

    /// <summary>
    /// Takes a specified amount of money from the player.
    /// </summary>
    /// <param name="amount">The amount of money to take from the player.</param>
    public void TakeMoney(int amount)
    {
        money -= amount;
        // TODO update UI money text 

        if (money <= 0)
        {
            Debug.Log("Mortgage or go bankrupt");
        }
    }

    /// <summary>
    /// Sets the sprite for the player game object.
    /// </summary>
    /// <param name="sprite">The sprite to set for the player.</param>
    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
        GetComponent<SpriteRenderer>().sprite = _sprite;
    }
}