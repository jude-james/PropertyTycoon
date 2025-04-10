using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    protected static Player CurrentBidder;

    public Sprite Sprite { get; private set; }
    private Animator _animator;
    private GameObject _infoPanel;

    private TMP_Text _moneyText;
    private TMP_Text _getOutOfJailFreeCardsText;

    public string Name { get; set; }
    public int Money { get; private set; } = 1500;

    public Property[] TitleDeeds { get; private set; }
    
    public bool InJail { get; private set; }
    protected int RoundsInJail;
    private const int RoundsInJailLimit = 2;
    protected const int PostBailAmount = 50;
    protected List<ActionCard> GetOutOfJailFreeCards;
    
    public int DiceRoll { get; private set; }
    private int _diceRoll1;
    private int _diceRoll2;
    
    private bool _rolledADouble;
    private int _doubleCount;
    private const int DoubleLimit = 3;
    
    private Tile _currentTile;
    private int _currentTileIndex;
    private int _newTileIndex;
    
    private const int PassedGoAmount = 200;
    public bool PassedGo { get; private set; }
    
    private const int MoveSpeed = 10;
    
    private readonly WaitForSeconds _reactionTime = new(0.5f);
    private readonly WaitForSeconds _pauseBetweenTileTime = new(0.1f);
    private readonly WaitForSeconds _jailPopupTime = new(1.5f);
    private readonly WaitForSeconds _bankruptPopupTime = new(3f);
    
    protected void Start()
    {
        _animator = GetComponent<Animator>();

        GetOutOfJailFreeCards = new List<ActionCard>();
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
        if (RoundsInJail == 0)
        {
            RoundsInJail++;
            InJailDecision();
        }
        else if (RoundsInJail == RoundsInJailLimit)
        {
            RoundsInJail = 0;
            LeaveJail();
        }
        else
        {
            RoundsInJail++;
            OnEndTurn();
        }
    }
    
    /// <summary>
    /// Shows prompt to payer or manually rolls if bot
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
        
        AudioManager.Instance.Play("diceRollSound");
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
            AudioManager.Instance.Play("tokenSound");
            
            if (i != _newTileIndex) // Don't pause between tile if on the last tile
                yield return _pauseBetweenTileTime;

            if (i == Board.Instance.GetTileIndex("Go"))
            {
                AudioManager.Instance.Play("passedGoSound");
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
    /// Called once tile functionality is complete and either ends the turn or allows for another turn, ensuring player has positive money
    /// </summary>
    public void CompleteTurn()
    {
        if (Money < 0)
        {
            if (IsBankrupt())
            {
                GoBankrupt();
            }
            else
            {
                UIManager.Instance.ShowRaiseFundsDialog(this);
                RaiseFundsDecision();
            }
        }
        else
        {
            UIManager.Instance.HideRaiseFundsDialog();
            
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
    }

    /// <summary>
    /// Shows prompts to player or manually decides action if bot 
    /// </summary>
    protected virtual void EndTurnDecision()
    {
        UIManager.Instance.ShowEndTurnPrompt();
        
        if (CanMortgage())
        {
            UIManager.Instance.EnableMortgageButton();
        }

        if (CanUnmortgage())
        {
            UIManager.Instance.EnableUnmortgageButton();
        }

        if (CanSellProperty())
        {
            UIManager.Instance.EnableSellPropertyButton();
        }

        if (CanBuild())
        {
            UIManager.Instance.EnableBuildButton();
        }

        if (CanSellBuildings())
        {
            UIManager.Instance.EnableSellBuildingsButton();
        }
    }
    
    /// <summary>
    /// Handles the player's turn ending and signaling the board to end the turn.
    /// </summary>
    protected void OnEndTurn()
    {
        if (this != _activePlayer) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        UIManager.Instance.DisableSideButtons();
        
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
            AudioManager.Instance.Play("goToJailSound");
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
        RoundsInJail = 0;

        AudioManager.Instance.Play("leaveJailSound");
        
        var justVisitingIndex = Board.Instance.GetTileIndex("Jail/Just visiting");
        SetNewTileIndex(justVisitingIndex);
        yield return MoveBetweenPositions(Board.Instance.Tiles[justVisitingIndex].transform.position);
        LandOnTile();
    }

    /// <summary>
    /// Shows prompt to player or manually decides action if bot
    /// </summary>
    protected virtual void InJailDecision()
    {
        UIManager.Instance.ShowInJailPrompt(Money >= PostBailAmount, GetOutOfJailFreeCards.Count > 0, true);
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
        
        var actionCard = GetOutOfJailFreeCards[0];
        
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
    /// Shows for sale prompt to player or manually decides action if bot
    /// </summary>
    /// <param name="property">The property the player landed on</param>
    public virtual void ForSaleDecision(Property property)
    {
        UIManager.Instance.ShowForSalePrompt(Money >= property.Cost, Board.Instance.CanAuction(), property);
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
        
        GiveTitleDeed(property);
        Board.Instance.TakeTitleDeed(property);
        
        property.OwnedBy = this;
        
        AudioManager.Instance.Play("buySound");
        
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
        CurrentBidder = this;
        
        var canBid = Money >= Board.Instance.AuctionPrice + Board.Instance.BidAmount;
        UIManager.Instance.UpdateAuctionPrompt(canBid, true, Name, Sprite);
    }

    protected void OnBid()
    {
        if (CurrentBidder != this) return;
        
        Board.Instance.EndBid(false);
    }

    protected void OnFold()
    {
        if (CurrentBidder != this) return;
        
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
        
        GiveTitleDeed(property);
        Board.Instance.TakeTitleDeed(property);

        property.OwnedBy = this;
    }

    /// <summary>
    /// Gets and outlines all properties that are unmortgaged and allows the player to click on them
    /// </summary>
    private void OnMortgage()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        UIManager.Instance.DisableSideButtons();
        
        UIManager.Instance.ShowMortgagePrompt();

        var mortgageableProperties = GetMortgageableProperties();
        foreach (var property in mortgageableProperties)
        {
            property.ShowOutline(Color.white, Color.blue);
            property.InMortgageSelection = true;
        }
    }

    private void OnEndMortgage()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideMortgagePrompt();
        CompleteTurn();
        
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                property.HideOutline();
                property.InMortgageSelection = false;
            }
        }
    }

    /// <summary>
    /// Gets and outlines all properties that are mortgaged and within the funds of the player to unmortgage
    /// </summary>
    private void OnUnmortgage()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        UIManager.Instance.DisableSideButtons();
        
        UIManager.Instance.ShowUnmortgagePrompt();

        var unmortgageableProperties = GetUnmortgageableProperties();
        foreach (var property in unmortgageableProperties)
        {
            property.ShowOutline(Color.white, Color.blue);
            property.InUnmortgageSelection = true;
        }
    }

    private void OnEndUnmortgage()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideUnmortgagePrompt();
        EndTurnDecision();
        
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                property.HideOutline();
                property.InUnmortgageSelection = false;
            }
        }
    }

    private void OnSellProperty()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        UIManager.Instance.DisableSideButtons();
        
        UIManager.Instance.ShowSellPropertyPrompt();

        var sellableProperties = GetSellableProperties();
        foreach (var property in sellableProperties)
        {
            property.ShowOutline(Color.white, Color.blue);
            property.InSellPropertySelection = true;
        }
    }

    private void OnEndSellProperty()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideSellPropertyPrompt();
        CompleteTurn();
        
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                property.HideOutline();
                property.InSellPropertySelection = false;
            }
        }
    }

    private void OnBuild()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        UIManager.Instance.DisableSideButtons();

        UIManager.Instance.ShowBuildPrompt();
        
        var buildableProperties = GetBuildableProperties();
        foreach (var property in buildableProperties)
        {
            property.ShowOutline(Color.white, Color.blue);
            property.InBuildSelection = true;
        }
    }

    private void OnEndBuild()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideBuildPrompt();
        EndTurnDecision();
        
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                property.HideOutline();
                property.InBuildSelection = false;
            }
        }
    }

    private void OnSellBuildings()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideEndTurnPrompt();
        UIManager.Instance.DisableSideButtons();

        UIManager.Instance.ShowSellBuildingsPrompt();
        
        var unbuildableProperties = GetSellableBuildingProperties();
        foreach (var property in unbuildableProperties)
        {
            property.ShowOutline(Color.white, Color.blue);
            property.InSellBuildingsSelection = true;        
        }
    }

    private void OnEndSellBuildings()
    {
        if (_activePlayer != this) return;
        
        UIManager.Instance.HideSellBuildingsPrompt();
        CompleteTurn();
        
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                property.HideOutline();
                property.InSellBuildingsSelection = false;
            }
        }
        
    }

    public int TotalHouses()
    {
        return TitleDeeds.OfType<Street>().Where(street => !street.HasMaxBuildings()).Sum(street => street.CurrentHouses);
    }

    public int TotalHotels()
    {
        return TitleDeeds.OfType<Street>().Sum(street => street.CurrentHotels);
    }
    
    /// <summary>
    /// Gives the player a specified amount of money.
    /// </summary>
    /// <param name="amount">The amount of money to give the player.</param>
    public void GiveMoney(int amount)
    {
        Money += amount;
        UIManager.Instance.AnimateMoney(_moneyText, Money);
    }

    /// <summary>
    /// Takes a specified amount of money from the player.
    /// </summary>
    /// <param name="amount">The amount of money to take from the player.</param>
    public void TakeMoney(int amount)
    {
        Money -= amount;
        UIManager.Instance.AnimateMoney(_moneyText, Money);
    }
    
    /// <summary>
    /// Determines if the player can raise enough funds to avoid bankruptcy 
    /// </summary>
    private bool IsBankrupt()
    {
        var buildingFunds = TitleDeeds.OfType<Street>().Where(street => street != null).Sum(street => street.GetBuildingValue());
        var propertyFunds = GetSellableProperties().Sum(property => property.Mortgaged ? property.MortgagedValue : property.Cost);
        var totalFunds = buildingFunds + propertyFunds;
        return totalFunds < -Money;
    }

    /// <summary>
    /// Calculates the total value of this player and all their assets
    /// </summary>
    /// <returns> The value of the players cash, properties and buildings</returns>
    public int GetTotalValue()
    {
        var totalValue = 0;

        totalValue += Money;
        
        foreach (var property in TitleDeeds)
        {
            if (property == null) continue;
            
            if (property.Mortgaged)
                totalValue += property.MortgagedValue;
            else
                totalValue += property.Cost;

            if (property is Street street)
            {
                totalValue += street.GetBuildingValue();
            }
        }
        
        return totalValue;
    }
    
    // @cond
    private void GoBankrupt()
    {
        StartCoroutine(GoBankruptCoroutine());
    }
    // @endcond

    /// <summary>
    /// Returns all properties owned by the player to the bank and signals to the board to remove the player
    /// </summary>
    private IEnumerator GoBankruptCoroutine()
    {
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                Board.Instance.GiveTitleDeed(property);
                TakeTitleDeed(property);
                property.OwnedBy = null;
            }
        }
        
        AudioManager.Instance.Play("bankruptSound");
        UIManager.Instance.ShowBankruptPopup();
        yield return _bankruptPopupTime;
        UIManager.Instance.HideBankruptPopup();
        
        Board.Instance.RemovePlayer();
    }
    
    /// <summary>
    /// Shows prompt to player or manually raises funds if bot
    /// </summary>
    protected virtual void RaiseFundsDecision()
    {
        if (CanMortgage())
        {
            UIManager.Instance.EnableMortgageButton();
        }

        if (CanSellProperty())
        {
            UIManager.Instance.EnableSellPropertyButton();
        }

        if (CanSellBuildings())
        {
            UIManager.Instance.EnableSellBuildingsButton();
        }
    }
    
    protected List<Property> GetMortgageableProperties()
    {
        return TitleDeeds.Where(property => property != null && !property.Mortgaged).ToList();
    }

    protected List<Property> GetUnmortgageableProperties()
    {
        return TitleDeeds.Where(property => property != null && property.Mortgaged && Money >= property.UnmortgagedValue).ToList();
    }

    protected List<Property> GetSellableProperties()
    {
        var sellableProperties = new List<Property>();
        
        foreach (var property in TitleDeeds)
        {
            if (property != null)
            {
                if (property is Street street)
                {
                    if (street.HasNoBuildings())
                    {
                        sellableProperties.Add(property);
                    }
                }
                else
                {
                    sellableProperties.Add(property);
                }
            }
        }

        return sellableProperties;
    }

    protected List<Street> GetBuildableProperties()
    {
        var buildableProperties = new List<Street>();
        var propertySet = new List<Street>();
        
        var count = 0;
        var sets = Enum.GetValues(typeof(Set)).Cast<Set>();
        foreach (var set in sets)
        {
            foreach (var street in TitleDeeds.OfType<Street>())
            {
                if (street.Set == set)
                {
                    count++;
                    if (!street.HasMaxBuildings() && Money >= street.HouseCost)
                    {
                        propertySet.Add(street);
                    }
                }
            }

            if (set is Set.Brown or Set.DeepBlue)
            {
                if (count == 2)
                {
                    buildableProperties.AddRange(propertySet);
                }
            }
            else if (count == 3)
            {
                buildableProperties.AddRange(propertySet);
            }

            count = 0;
            propertySet = new List<Street>();
        }
        
        return buildableProperties;
    }

    protected List<Street> GetSellableBuildingProperties()
    {
        return TitleDeeds.OfType<Street>().Where(street => !street.HasNoBuildings()).ToList();
    }
    
    protected bool CanMortgage()
    {
        return GetMortgageableProperties().Count > 0;
    }

    protected bool CanUnmortgage()
    {
        return GetUnmortgageableProperties().Count > 0;
    }

    protected bool CanSellProperty()
    {
        return GetSellableProperties().Count > 0;
    }

    protected bool CanBuild()
    {
        return GetBuildableProperties().Count > 0;
    }

    protected bool CanSellBuildings()
    {
        return GetSellableBuildingProperties().Count > 0;
    }

    /// <summary>
    /// Adds a Get Out Of Jail Free card to the player's collection.
    /// </summary>
    /// <param name="actionCard">The Get Out Of Jail Free card to add.</param>
    public void AddGetOutOfJailFreeCard(ActionCard actionCard)
    {
        GetOutOfJailFreeCards.Add(actionCard);
        UpdateGetOutOfJailFreeCardNumber();
    }

    private void RemoveGetOutOfJailFreeCard(ActionCard actionCard)
    {
        GetOutOfJailFreeCards.Remove(actionCard);
        UpdateGetOutOfJailFreeCardNumber();
    }

    private void GiveTitleDeed(Property property)
    {
        TitleDeeds[property.PropertyNumber] = property;
        UIManager.Instance.UpdateTitleDeedUI(TitleDeeds, _infoPanel);
    }

    public void TakeTitleDeed(Property property)
    {
        TitleDeeds[property.PropertyNumber] = null;
        UIManager.Instance.UpdateTitleDeedUI(TitleDeeds, _infoPanel);
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
        
        UIManager.Instance.mortgageButton.onClick.AddListener(OnMortgage);
        UIManager.Instance.unmortgageButton.onClick.AddListener(OnUnmortgage);
        
        UIManager.Instance.endMortgageButton.onClick.AddListener(OnEndMortgage);
        UIManager.Instance.endUnmortgageButton.onClick.AddListener(OnEndUnmortgage);
        
        UIManager.Instance.sellPropertyButton.onClick.AddListener(OnSellProperty);
        UIManager.Instance.endSellPropertyButton.onClick.AddListener(OnEndSellProperty);
        
        UIManager.Instance.buildButton.onClick.AddListener(OnBuild);
        UIManager.Instance.endBuildButton.onClick.AddListener(OnEndBuild);
        
        UIManager.Instance.sellBuildingsButton.onClick.AddListener(OnSellBuildings);
        UIManager.Instance.endSellBuildingsButton.onClick.AddListener(OnEndSellBuildings);
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
        _moneyText.SetText("£"+Money);
        
        _getOutOfJailFreeCardsText = _infoPanel.transform.GetChild(4).GetComponent<TMP_Text>();
        UpdateGetOutOfJailFreeCardNumber();
    }

    private void UpdateGetOutOfJailFreeCardNumber()
    {
        _getOutOfJailFreeCardsText.SetText(GetOutOfJailFreeCards.Count.ToString());
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
    
    /// <summary>
    /// Gets the current tile index of the player. This method is primarily used for unit testing
    /// to verify player position on the board and movement logic.
    /// </summary>
    /// <returns>The index of the tile the player is currently on</returns>
    public int GetCurrentTileIndex()
    {
        return _currentTileIndex;
    }
}