using System.Collections;
using System.Collections.Generic;
using Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Represents a playable game object throughout the game, which can either be a human player or a bot.
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
    
    private bool _inJail;
    private int _roundsInJail;
    private const int RoundsInJailLimit = 2;
    private const int PostBailAmount = 50;
    private int _getOutOfJailFreeCards;
    
    public int DiceRoll { get; private set; }
    private int _diceRoll1;
    private int _diceRoll2;
    
    private bool _rolledADouble;
    private int _doubleCount;
    private const int DoubleLimit = 3;

    private int _houses; // These might not be needed
    private int _hotels;
    
    private Tile _currentTile;
    private int _currentTileIndex;
    private int _newTileIndex;
    
    private bool _passedGo;
    private const int PassedGoAmount = 200;
    
    private const int MoveSpeed = 10;
    
    private readonly WaitForSeconds _reactionTime = new(0.5f);
    private readonly WaitForSeconds _pauseBetweenTileTime = new(0.1f);
    private readonly WaitForSeconds _jailPopupTime = new(1.5f);
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        AssignButtonEventListeners();
        SetInfoPanel();
    }
    
    /// <summary>
    /// Starts the player's turn, setting them as the active player and updating the UI.
    /// </summary>
    public void StartTurn()
    {
        _activePlayer = this;

        UIManager.Instance.SetActivePlayerInfo(Name, _sprite);
        
        if (_inJail)
        {
            DetermineJailAction();
        }
        else
        {
            RollDiceDecision();
        }
    }

    /// <summary>
    /// Determines what should happen to the player when they begin the round in jail
    /// </summary>
    private void DetermineJailAction()
    {
        if (_roundsInJail == 0)
        {
            _roundsInJail++;
            InJailDecision();
        }
        else if (_roundsInJail == RoundsInJailLimit+1)
        {
            _roundsInJail = 0;
            LeaveJail();
        }
        else
        {
            _roundsInJail++;
            OnEndTurn();
        }
    }
    
    /// <summary>
    /// Decision point for rolling dice
    /// </summary>
    protected virtual void RollDiceDecision()
    {
        UIManager.Instance.ShowRollDicePrompt();
    }
    
    /// <summary>
    /// Handles the rolling of dice and the action to take depending on the dice roll
    /// </summary>
    protected void OnRollDice()
    {
        if (this != _activePlayer) return;

        StartCoroutine(OnRollDiceCoroutine());
    }

    private IEnumerator OnRollDiceCoroutine()
    {
        UIManager.Instance.HideRollDicePrompt();
        
        _diceRoll1 = Random.Range(1, 7);
        _diceRoll2 = Random.Range(1, 7);
        DiceRoll = _diceRoll1 + _diceRoll2;

        if (_diceRoll1 == _diceRoll2)
        {
            _rolledADouble = true;
            _doubleCount++;
        }
        
        yield return UIManager.Instance.AnimateDiceRoll(_diceRoll1, _diceRoll2);
        yield return _reactionTime;

        if (_doubleCount == DoubleLimit)
        {
            GoToJail();
        }
        else
        {
            ShiftTileIndex(DiceRoll);
            StartCoroutine(MoveToTile(false));
        }
    }
    
    /// <summary>
    /// Sets the newTileIndex depending on the offset from the currentTileIndex, e.g. offset of -3 sets newTileIndex back 3 spaces
    /// </summary>
    /// <param name="offset"> The number of spaces +- from the currentTileIndex </param>
    private void ShiftTileIndex(int offset)
    {
        var newIndex = _currentTileIndex + offset;
        _newTileIndex = Maths.Mod(newIndex, Board.Instance.Tiles.Count);
    }
    
    private void SetNewTileIndex(int newTileIndex)
    {
        _newTileIndex = Maths.Mod(newTileIndex, Board.Instance.Tiles.Count);
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
            yield return MoveBetweenPositions(Board.Instance.Tiles[i].transform.position);
            
            if (i != _newTileIndex) // Don't pause between tile if on the last tile
                yield return _pauseBetweenTileTime;

            if (i == 0)
                _passedGo = true;
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
    /// Updates players current tile to the newTileIndex and triggers tile-specific functionality
    /// </summary>
    private void LandOnTile()
    {
        if (_passedGo)
        {
            Debug.Log("Passed go");
            GiveMoney(PassedGoAmount);
            _passedGo = false;
        }
        
        _currentTileIndex = _newTileIndex;
        _currentTile = Board.Instance.Tiles[_currentTileIndex];
        
        _currentTile.OnLanded(this);
    }

    /// <summary>
    /// Called once tile functionality is complete and either ends the turn or allows for another turn
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

    /// <summary>
    /// Decision point for ending turn
    /// </summary>
    protected virtual void EndTurnDecision()
    {
        UIManager.Instance.ShowEndTurnPrompt();
    }
    
    /// <summary>
    /// Handles the player's turn ending and signaling the board to end the turn.
    /// </summary>
    protected void OnEndTurn()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        Board.Instance.EndTurn();
    }

    /// <summary>
    /// Sends the player to the Jail position and ends their turn
    /// </summary>
    public void GoToJail()
    {
        StartCoroutine(GoToJailCoroutine());
    }

    private IEnumerator GoToJailCoroutine()
    {
        _rolledADouble = false;
        _doubleCount = 0;
        
        UIManager.Instance.ShowGoToJailPopup();
        yield return _jailPopupTime; 
        UIManager.Instance.HideGoToJailPopup();
        
        StartAnimation();
        yield return MoveBetweenPositions(Board.Instance.JailPosition);
        StopAnimation();
        
        _inJail = true;
        
        EndTurnDecision();
    }

    /// <summary>
    /// Sends player to the just visiting tile and lands on that tile
    /// </summary>
    private void LeaveJail()
    {
        StartCoroutine(LeaveJailCoroutine());
    }

    private IEnumerator LeaveJailCoroutine()
    {
        _inJail = false;
        _roundsInJail = 0;
        
        SetNewTileIndex(Board.Instance.justVisitingIndex);
        yield return MoveBetweenPositions(Board.Instance.Tiles[Board.Instance.justVisitingIndex].transform.position);
        LandOnTile();
    }

    /// <summary>
    /// Decision point for in jail, shows jail prompt and disables buttons accordingly
    /// </summary>
    protected virtual void InJailDecision()
    {
        UIManager.Instance.ShowInJailPrompt(_money >= PostBailAmount, _getOutOfJailFreeCards > 0, true);
    }

    /// <summary>
    /// Player pays fine and leaves jail
    /// </summary>
    protected void OnPostBail()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideInJailPrompt();
        
        TakeMoney(PostBailAmount);
        Board.Instance.FreeParkingSum += PostBailAmount;
        
        LeaveJail();
    }

    /// <summary>
    /// Player uses get out of jail card and leaves jail 
    /// </summary>
    protected void OnGetOutOfJailFree()
    {
        if (_activePlayer != this) return;

        UIManager.Instance.HideInJailPrompt();
        
        // TODO make UpdateGetOutOfJailFree() or something that changes the UI element
        _getOutOfJailFreeCards--;
        LeaveJail();
    }
    
    protected void OnRemainInJail()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideInJailPrompt();

        OnEndTurn();
    }
    
    /// <summary>
    /// Decision point for purchasing a property, shows for sale prompt and disables buttons accordingly
    /// </summary>
    /// <param name="property"></param>
    public virtual void ForSaleDecision(Property property)
    {
        UIManager.Instance.ShowForSalePrompt(_money >= property.Cost, true, property);
    }

    /// <summary>
    /// Buys the property that the player is currently on
    /// </summary>
    protected void OnBuy()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideForSalePrompt();
        
        Debug.Log("Buying property...");

        var property = (Property)_currentTile;
        
        // TODO finish buy logic
        // TakeMoney(property.Cost);
        // ... 
        
        CompleteTurn();
    }

    /// <summary>
    /// Auctions the property that the player is currently on
    /// </summary>
    protected void OnAuction()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideForSalePrompt();
        
        Debug.Log("Auctioning property...");
        
        // TODO auction
        
        CompleteTurn();
    }

    /// <summary>
    /// Gives the player a specified amount of money.
    /// </summary>
    /// <param name="amount">The amount of money to give the player.</param>
    public void GiveMoney(int amount)
    {
        _money += amount;
        UpdateInfoPanel();
    }

    /// <summary>
    /// Takes a specified amount of money from the player.
    /// </summary>
    /// <param name="amount">The amount of money to take from the player.</param>
    public void TakeMoney(int amount)
    {
        var newMoney = _money - amount;

        if (newMoney < 0)
        {
            Debug.Log("Mortgage or go bankrupt");
        }
        else
        {
            _money = newMoney;
            UpdateInfoPanel();
        }
    }
    
    private void AssignButtonEventListeners()
    {
        UIManager.Instance.rollDiceButton.onClick.AddListener(OnRollDice);
        UIManager.Instance.endTurnButton.onClick.AddListener(OnEndTurn);
        
        UIManager.Instance.buyButton.onClick.AddListener(OnBuy);
        UIManager.Instance.auctionButton.onClick.AddListener(OnAuction);
        
        UIManager.Instance.postBailButton.onClick.AddListener(OnPostBail);
        UIManager.Instance.getOutOfJailFreeButton.onClick.AddListener(OnGetOutOfJailFree);
        UIManager.Instance.remainInJailButton.onClick.AddListener(OnRemainInJail);
    }
    
    /// <summary>
    /// Gets the player info panel from the UI Manager, and then updates the UI to the token, name and money values
    /// </summary>
    private void SetInfoPanel()
    {
        _infoPanel = UIManager.Instance.GetInfoPanel();

        var token = _infoPanel.transform.GetChild(0).GetComponent<Image>();
        token.sprite = _sprite;
        
        var nameText = _infoPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        nameText.SetText(Name);

        var moneyText = _infoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        moneyText.SetText("£"+_money);
    }

    private void UpdateInfoPanel()
    {
        // TODO make variables global and separate into multiple methods
        var moneyText = _infoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        moneyText.SetText("£"+_money);

        var getOutOfJailFreeCardsText = _infoPanel.transform.GetChild(3).GetComponent<TMP_Text>();
        getOutOfJailFreeCardsText.SetText(_getOutOfJailFreeCards.ToString());
        
        // TODO loop through title deeds and update titledeedmini UI list
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