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
    protected static Player _currentBidder;

    public Sprite Sprite { get; private set; }
    private Animator _animator;
    private GameObject _infoPanel;

    private TMP_Text _moneyText;
    private TMP_Text _getOutOfJailFreeCardsText;

    public string Name { get; set; }
    private int _money = 1500;

    private Property[] TitleDeeds { get; set; }
    
    public bool InJail { get; private set; }
    private int _roundsInJail;
    private const int RoundsInJailLimit = 2;
    private const int PostBailAmount = 50;
    private List<ActionCard> _getOutOfJailFreeCards;
    
    public int DiceRoll { get; private set; }
    private int _diceRoll1;
    private int _diceRoll2;
    
    private bool _rolledADouble;
    private int _doubleCount;
    private const int DoubleLimit = 3;

    public int Houses { get; private set; } 
    public int Hotels { get; private set; }
    
    private Tile _currentTile;
    private int _currentTileIndex;
    private int _newTileIndex;
    
    private const int PassedGoAmount = 200;
    public bool PassedGo { get; private set; }
    
    private const int MoveSpeed = 10;
    
    private readonly WaitForSeconds _reactionTime = new(0.5f);
    private readonly WaitForSeconds _pauseBetweenTileTime = new(0.1f);
    private readonly WaitForSeconds _jailPopupTime = new(1.5f);
    
    private void Start()
    {
        _animator = GetComponent<Animator>();

        _getOutOfJailFreeCards = new List<ActionCard>();
        TitleDeeds = new Property[Board.Instance.TitleDeeds.Length];
        
        AssignButtonEventListeners();
        SetInfoPanel();
    }
    
    /// <summary>
    /// Starts the player's turn, setting them as the active player and updating the UI.
    /// </summary>
    public void StartTurn()
    {
        _activePlayer = this;

        UIManager.Instance.SetActivePlayerInfo(Name, Sprite);
        
        if (InJail)
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
        else if (_roundsInJail == RoundsInJailLimit)
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
    
    /// @cond
    protected void OnRollDice()
    {
        if (this != _activePlayer) return;

        StartCoroutine(OnRollDiceCoroutine());
    }
    /// @endcond

    /// <summary>
    /// Handles the rolling of dice and the action to take depending on the dice roll
    /// </summary>
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
            GoToJail(true);
        }
        else
        {
            ShiftTileIndex(DiceRoll);
            MoveToTile(Direction.Shortest);
        }
    }
    
    /// <summary>
    /// Sets the newTileIndex depending on the offset from the currentTileIndex, e.g. offset of -3 sets newTileIndex back 3 spaces
    /// </summary>
    /// <param name="offset"> The number of spaces +- from the currentTileIndex </param>
    public void ShiftTileIndex(int offset)
    {
        var newIndex = _currentTileIndex + offset;
        _newTileIndex = Maths.Mod(newIndex, Board.Instance.Tiles.Count);
    }
    
    /// <summary>
    /// Sets the new tile index for the player, ensuring it wraps around the board if necessary.
    /// </summary>
    /// <param name="newTileIndex">The new tile index to set.</param>
    public void SetNewTileIndex(int newTileIndex)
    {
        _newTileIndex = Maths.Mod(newTileIndex, Board.Instance.Tiles.Count);
    }
    
    /// @cond
    public void MoveToTile(Direction direction)
    {
        StartCoroutine(MoveToTileCoroutine(direction));
    }
    /// @endcond

    /// <summary>
    /// Animates player from currentTileIndex to newTileIndex, then lands player on tile
    /// </summary>
    /// <param name="direction">
    /// Determines the direction around the board the player moves.
    /// If Shortest is picked the direction is determined by the shortest calculated distance
    /// </param>
    private IEnumerator MoveToTileCoroutine(Direction direction)
    {
        StartAnimation();

        var tileCount = Board.Instance.Tiles.Count;
        var forwardDistance = (_newTileIndex - _currentTileIndex + tileCount) % tileCount;
        var backwardDistance =  (_currentTileIndex  - _newTileIndex + tileCount) % tileCount;

        var step = direction switch
        {
            Direction.Clockwise => 1,
            Direction.Anticlockwise => -1,
            _ => forwardDistance <= backwardDistance ? 1 : -1
        };

        for (int i = Maths.Mod(_currentTileIndex + step,tileCount); i != Maths.Mod(_newTileIndex + step,tileCount); i = (i + step + tileCount) % tileCount)
        {
            yield return MoveBetweenPositions(Board.Instance.Tiles[i].transform.position);
            
            if (i != _newTileIndex) // Don't pause between tile if on the last tile
                yield return _pauseBetweenTileTime;

            if (i == Board.Instance.GetTileIndex("Go"))
            {
                PassedGo = true;
                GiveMoney(PassedGoAmount);
            }
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

    /// @cond
    public void GoToJail(bool showPopup)
    {
        StartCoroutine(GoToJailCoroutine(showPopup));
    }
    /// @endcond

    /// <summary>
    /// Sends the player to the Jail position and ends their turn
    /// </summary>
    private IEnumerator GoToJailCoroutine(bool showPopup)
    {
        _rolledADouble = false;
        _doubleCount = 0;

        if (showPopup)
        {
            UIManager.Instance.ShowGoToJailPopup();
            yield return _jailPopupTime; 
            UIManager.Instance.HideGoToJailPopup();
        }
        
        StartAnimation();
        yield return MoveBetweenPositions(Board.Instance.JailPosition);
        StopAnimation();
        
        InJail = true;
        
        EndTurnDecision();
    }

    /// @cond
    private void LeaveJail()
    {
        StartCoroutine(LeaveJailCoroutine());
    }
    /// @endcond

    /// <summary>
    /// Sends player to the just visiting tile and lands on that tile
    /// </summary>
    private IEnumerator LeaveJailCoroutine()
    {
        InJail = false;
        _roundsInJail = 0;

        var justVisitingIndex = Board.Instance.GetTileIndex("Jail/Just visiting");
        SetNewTileIndex(justVisitingIndex);
        yield return MoveBetweenPositions(Board.Instance.Tiles[justVisitingIndex].transform.position);
        LandOnTile();
    }

    /// <summary>
    /// Decision point for in jail, shows jail prompt and disables buttons accordingly
    /// </summary>
    protected virtual void InJailDecision()
    {
        UIManager.Instance.ShowInJailPrompt(_money >= PostBailAmount, _getOutOfJailFreeCards.Count > 0, true);
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
    /// Player returns get out of jail free card to board card pile and leaves jail 
    /// </summary>
    protected void OnGetOutOfJailFree()
    {
        if (_activePlayer != this) return;

        UIManager.Instance.HideInJailPrompt();
        
        var actionCard = _getOutOfJailFreeCards[0];
        
        if (actionCard.CardType == CardType.PotLuck)
            Board.Instance.PotLuckCards.Enqueue(actionCard);
        else if (actionCard.CardType == CardType.OppKnock) 
            Board.Instance.OpportunityKnocksCards.Enqueue(actionCard);
        
        RemoveGetOutOfJailFreeCard(actionCard);
        
        LeaveJail();
    }
    
    /// <summary>
    /// Handles the player's decision to remain in jail, hiding the jail prompt and ending the turn.
    /// </summary>
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
        // TODO auctionButtonEnabled, board.CanAuction or something
        UIManager.Instance.ShowForSalePrompt(_money >= property.Cost, true, property);
    }

    /// <summary>
    /// Buys the property that the player is currently on and gives the player the title deed card
    /// </summary>
    protected void OnBuy()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideForSalePrompt();
        
        var property = (Property)_currentTile;
        
        TakeMoney(property.Cost);

        TitleDeeds[property.PropertyNumber] = property;
        UIManager.Instance.UpdateTitleDeedUI(TitleDeeds, _infoPanel);
        
        Board.Instance.TakeTitleDeed(property);
        
        property.OwnedBy = this;
        
        CompleteTurn();
    }

    /// <summary>
    /// Auctions the property that the player is currently on
    /// </summary>
    protected void OnAuction()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideForSalePrompt();
        
        var property = (Property) _currentTile;
        UIManager.Instance.ShowAuctionPrompt(property);
        Board.Instance.StartAuction(property);
    }

    /// <summary>
    /// Sets this player as the current bidder and gives player button choices
    /// </summary>
    public virtual void BidDecision()
    {
        _currentBidder = this;
        
        var canBid = _money >= Board.Instance.AuctionPrice + Board.Instance.BidAmount;
        UIManager.Instance.UpdateAuctionPrompt(canBid, true, Name, Sprite);
    }

    protected void OnBid()
    {
        if (_currentBidder != this) return;
        
        Board.Instance.EndBid(false, Board.Instance.BidAmount);
    }

    protected void OnFold()
    {
        if (_currentBidder != this) return;
        
        Board.Instance.EndBid(true);
    }

    /// <summary>
    /// Gives the player the property that was auctioned for the auction price and gives them the title deed card
    /// </summary>
    /// <param name="property">The auctioned property</param>
    /// <param name="price">The auction price</param>
    public void WinAuction(Property property, int price)
    {
        TakeMoney(price);
        
        TitleDeeds[property.PropertyNumber] = property;
        UIManager.Instance.UpdateTitleDeedUI(TitleDeeds, _infoPanel);
        
        Board.Instance.TakeTitleDeed(property);

        property.OwnedBy = this;
    }
    
    /// <summary>
    /// Gives the player a specified amount of money.
    /// </summary>
    /// <param name="amount">The amount of money to give the player.</param>
    public void GiveMoney(int amount)
    {
        _money += amount;
        UIManager.Instance.AnimateMoney(_moneyText, _money);
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
            // TODO deal with what to do when they go below zero, maybe return a bool to signal if take money is possible
            Debug.Log("Mortgage or go bankrupt");
        }
        else
        {
            _money = newMoney;
            UIManager.Instance.AnimateMoney(_moneyText, _money);
        }
    }

    /// <summary>
    /// Adds a Get Out Of Jail Free card to the player's collection.
    /// </summary>
    /// <param name="actionCard">The Get Out Of Jail Free card to add.</param>
    public void AddGetOutOfJailFreeCard(ActionCard actionCard)
    {
        _getOutOfJailFreeCards.Add(actionCard);
        UpdateGetOutOfJailFreeCardNumber();
    }

    private void RemoveGetOutOfJailFreeCard(ActionCard actionCard)
    {
        _getOutOfJailFreeCards.Remove(actionCard);
        UpdateGetOutOfJailFreeCardNumber();
    }
    
    private void AssignButtonEventListeners()
    {
        UIManager.Instance.rollDiceButton.onClick.AddListener(OnRollDice);
        UIManager.Instance.endTurnButton.onClick.AddListener(OnEndTurn);
        
        UIManager.Instance.buyButton.onClick.AddListener(OnBuy);
        UIManager.Instance.auctionButton.onClick.AddListener(OnAuction);
        
        UIManager.Instance.bidButton.onClick.AddListener(OnBid);
        UIManager.Instance.foldButton.onClick.AddListener(OnFold);
        
        UIManager.Instance.postBailButton.onClick.AddListener(OnPostBail);
        UIManager.Instance.getOutOfJailFreeButton.onClick.AddListener(OnGetOutOfJailFree);
        UIManager.Instance.remainInJailButton.onClick.AddListener(OnRemainInJail);
    }
    
    /// <summary>
    /// Gets the player info panel from the UI Manager, and then initialises the UI to the player values
    /// </summary>
    private void SetInfoPanel()
    {
        _infoPanel = UIManager.Instance.GetInfoPanel();

        var token = _infoPanel.transform.GetChild(0).GetComponent<Image>();
        token.sprite = Sprite;
        
        var nameText = _infoPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        nameText.SetText(Name);

        _moneyText = _infoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        _moneyText.SetText("£"+_money);
        
        _getOutOfJailFreeCardsText = _infoPanel.transform.GetChild(4).GetComponent<TMP_Text>();
        UpdateGetOutOfJailFreeCardNumber();
    }

    private void UpdateGetOutOfJailFreeCardNumber()
    {
        _getOutOfJailFreeCardsText.SetText(_getOutOfJailFreeCards.Count.ToString());
    }

    /// <summary>
    /// Sets the sprite for the player game object.
    /// </summary>
    /// <param name="sprite">The sprite to set for the player.</param>
    public void SetSprite(Sprite sprite)
    {
        Sprite = sprite;
        GetComponent<SpriteRenderer>().sprite = Sprite;
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