using System.Collections;
using System.Collections.Generic;
using Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Playable gameObject throughout the game, can either be a human or a bot
/// </summary>
public class Player : MonoBehaviour
{
    private static Player _activePlayer;

    private Animator _animator;
    private Sprite _sprite;
    private GameObject _infoPanel;

    public string Name { get; set; }
    private int _money = 1500;
    private List<Property> _titleDeeds;
    private int _getOutOfJailFreeCards;
    private bool _inJail;
    
    public int DiceRoll { get; private set; }
    private int _diceRoll1;
    private int _diceRoll2;
    
    private bool _rolledADouble;
    private int _doubleCount;
    
    private int _houses; // These might not be needed
    private int _hotels;
    
    private Tile _currentTile;
    private int _currentTileIndex;
    private int _newTileIndex;
    
    private const int MoveSpeed = 10;

    private const int PassedGoAmount = 200;
    private const int DoubleLimit = 3;
    
    private readonly WaitForSeconds _reactionTime = new(0.5f);
    private readonly WaitForSeconds _pauseBetweenTileTime = new(0.1f);
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        UIManager.Instance.rollDiceButton.onClick.AddListener(OnRollDice);
        UIManager.Instance.endTurnButton.onClick.AddListener(OnEndTurn);
        UIManager.Instance.buyButton.onClick.AddListener(OnBuy);
        UIManager.Instance.auctionButton.onClick.AddListener(OnAuction);

        SetInfoPanel();
    }
    
    public void StartTurn()
    {
        _activePlayer = this;

        UIManager.Instance.SetActivePlayerInfo(Name, _sprite);
        
        // Decide here which buttons to gray out or which UI elements to pop up, e.g. if they are in jail
        if (_inJail)
        {
            // show specific in jail options
        }
        else
        {
            RollDiceDecision();
        }
    }

    protected virtual void RollDiceDecision()
    {
        UIManager.Instance.ShowRollDicePrompt();
    }
    
    /// <summary>
    /// Event function called when human clicks Roll Dice button
    /// </summary>
    protected void OnRollDice()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideRollDicePrompt();
        
        _diceRoll1 = Random.Range(1, 7);
        _diceRoll2 = Random.Range(1, 7);
        DiceRoll = _diceRoll1 + _diceRoll2;

        if (_diceRoll1 == _diceRoll2)
        {
            _rolledADouble = true;
            _doubleCount++;
        }
        
        SetNewTileIndex(DiceRoll);
        StartCoroutine(AnimateDiceRoll());
    }
    
    /// <summary>
    /// Sets the newTileIndex depending on the offset from the currentTileIndex, e.g. offset of -3 sets newTileIndex back 3 spaces
    /// </summary>
    /// <param name="offset"> The number of spaces +- from the currentTileIndex </param>
    private void SetNewTileIndex(int offset)
    {
        var newIndex = _currentTileIndex + offset;
        
        if (newIndex >= Board.Instance.Tiles.Count) // Use >= if we are including GO tile 
        {
            // Player has looped around the board, and therefore passed go
            Debug.Log("Passed go");
            GiveMoney(PassedGoAmount);
        }
        
        _newTileIndex = Maths.Mod(newIndex, Board.Instance.Tiles.Count);
    }
    
    private IEnumerator AnimateDiceRoll()
    {
        var diceRollTime = UIManager.Instance.AnimateDiceRoll(_diceRoll1, _diceRoll2);
        yield return diceRollTime;
        
        if (_doubleCount == DoubleLimit)
        {
            _doubleCount = 0;
            Debug.Log("Go to jail");
            // Set position to jail position
            // EndTurn()
        }
        else
        {
            yield return _reactionTime;
            StartCoroutine(MoveToTile(false));
        }
    }
    
    /// <summary>
    /// Animates player from currentTileIndex to newTileIndex, then lands player on tile
    /// </summary>
    /// <param name="clockwiseOnly">
    /// Toggles whether the player should move only clockwise around the board or can move anticlockwise.
    /// Otherwise the direction is determined by the shorter distance
    /// </param>
    /// <returns></returns>
    private IEnumerator MoveToTile(bool clockwiseOnly)
    {
        StartAnimation();

        var tileCount = Board.Instance.Tiles.Count;
        var forwardDistance = (_newTileIndex - _currentTileIndex + tileCount) % tileCount;
        var backwardDistance =  (_currentTileIndex  - _newTileIndex + tileCount) % tileCount;

        var direction = clockwiseOnly || forwardDistance <= backwardDistance ? 1 : -1;

        for (int i = Maths.Mod(_currentTileIndex + direction,tileCount); i != Maths.Mod(_newTileIndex + direction,tileCount); i = (i + direction + tileCount) % tileCount)
        {
            yield return StartCoroutine(MoveBetweenPositions(Board.Instance.Tiles[i].transform.position));
            if (i != _newTileIndex) // Don't pause between tile if on the last tile
                yield return _pauseBetweenTileTime;
        }
        
        StopAnimation();
        
        LandOnTile();
    }
    
    /// <summary>
    /// Moves transform from it's current position to the targetPosition
    /// </summary>
    /// <param name="targetPosition"> The vector this transform moves to </param>
    /// <returns></returns>
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
    /// Updates players current tile to the newTileIndex and calls tile functionality
    /// </summary>
    private void LandOnTile()
    {
        _currentTileIndex = _newTileIndex;
        _currentTile = Board.Instance.Tiles[_currentTileIndex];
        
        _currentTile.OnLanded(this);
        
        // CompleteTurn();
    }

    /// <summary>
    /// Method is called once tile functionality is completed and either ends the turn or allows for another roll
    /// </summary>
    public void CompleteTurn()
    {
        if (_rolledADouble)
        {
            _rolledADouble = false;
            StartTurn();
        }
        else
        {
            _doubleCount = 0;
            EndTurnDecision();
        }
    }

    protected virtual void EndTurnDecision()
    {
        UIManager.Instance.ShowEndTurnPrompt();
    }
    
    /// <summary>
    /// Event function called when player clicks End Turn button 
    /// </summary>
    protected void OnEndTurn()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        Board.Instance.EndTurn();
    }

    public virtual void ForSaleDecision(int cost)
    {
        UIManager.Instance.ShowForSalePrompt(cost);
    }

    /// <summary>
    /// Event function called when player clicks Buy button 
    /// </summary>
    protected void OnBuy()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideForSalePrompt();
        
        Debug.Log("Player has chosen to buy");
        
        // TODO check if have enough money first
        ((Property) _currentTile).Buy(this);
        
        CompleteTurn();
    }

    /// <summary>
    /// Event function called when player clicks Auction button 
    /// </summary>
    protected void OnAuction()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideForSalePrompt();
        
        Debug.Log("Player has chosen to auction");
        
        CompleteTurn();
    }
    
    public void GiveMoney(int amount)
    {
        _money += amount;
        // TODO update UI money text, with event or with an animation
    }

    public void TakeMoney(int amount)
    {
        _money -= amount;
        // TODO update UI money text 

        if (_money <= 0)
        {
            Debug.Log("Mortgage or go bankrupt");
        }
    }
    
    private void SetInfoPanel()
    {
        // TODO make an updateInfoPanel for when money or properties or jail card number changes 
        
        _infoPanel = UIManager.Instance.GetInfoPanel();

        var token = _infoPanel.transform.GetChild(0).GetComponent<Image>();
        token.sprite = _sprite;
        
        var nameText = _infoPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        nameText.SetText(Name);

        var moneyText = _infoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        moneyText.SetText("£"+_money);
    }
    
    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
        GetComponent<SpriteRenderer>().sprite = _sprite;
    }

    private void StartAnimation()
    {
        _animator.enabled = true;
    }

    private void StopAnimation()
    {
        _animator.enabled = false;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
    }
}