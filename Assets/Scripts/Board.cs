using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using TMPro;
using UnityEngine;

/// <summary>
/// Property Tycoon board, acts as a game manager. Tracks board tiles, bank, cards, and players
/// </summary>
public class Board : Singleton<Board>
{
    [SerializeField] private Transform boardTiles;
    [SerializeField] private Transform jailPosition;
    [SerializeField] private GameObject playerPrefab;
    
    private GameObject _bankInfoPanel;
    private TMP_Text _freeParkingSumText;
    
    [SerializeField] private Sprite[] tokens; // TODO remove once menu is done

    public Vector2 JailPosition => jailPosition.position;
    
    public List<Tile> Tiles { get; private set; }
    
    public Property[] TitleDeeds { get; private set; }
    
    public Queue<ActionCard> PotLuckCards { get; private set; }
    public Queue<ActionCard> OpportunityKnocksCards { get; private set; }
    
    private int _freeParkingSum;

    public int FreeParkingSum
    {
        get => _freeParkingSum;
        set
        {
            _freeParkingSum = value;
            UIManager.Instance.AnimateMoney(_freeParkingSumText, _freeParkingSum);
        }
    }
    
    public List<Player> Players { get; private set; }
    private Player _currentPlayer;
    private int _currentPlayerIndex;

    private List<Player> _bidders = new();
    private Player _currentBidder;
    private int _currentBidderIndex;
    public int AuctionPrice { get; private set; }
    public int BidAmount => 20;
    public Property _auctionProperty;
    
    private readonly WaitForSeconds _timeBetweenTurns = new(1);
    private readonly WaitForSeconds _timeBetweenBids = new(0.5f);

    private void Start()
    {
        var dataReader = new DataReader();
        
        dataReader.ReadBoardData(boardTiles);
        Tiles = dataReader.Tiles;
        
        TitleDeeds = new Property[dataReader.Properties.Count];
        for (var i = 0; i < dataReader.Properties.Count; i++)
        {
            TitleDeeds[i] = dataReader.Properties[i];
        }
        
        dataReader.ReadCardData();
        var rng = new System.Random();
        PotLuckCards = new Queue<ActionCard>(dataReader.PotLuckCards.ToList().OrderBy(_ => rng.Next()));
        OpportunityKnocksCards = new Queue<ActionCard>(dataReader.OpportunityKnocksCards.ToList().OrderBy(_ => rng.Next()));
        
        _bankInfoPanel = UIManager.Instance.BankInfoPanel;
        _freeParkingSumText = UIManager.Instance.FreeParkingInfoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        
        // Manually assigning players for testing purposes, will get players from main menu later
        Players = new List<Player>();

        var pl1 = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Player>();
        pl1.SetSprite(tokens[0]);
        pl1.Name = tokens[0].name;
        
        var pl2 = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Player>();
        pl2.SetSprite(tokens[1]);
        pl2.Name = tokens[1].name;
        
        var pl3 = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Player>();
        pl3.SetSprite(tokens[2]);
        pl3.Name = tokens[2].name;
        
        Players.Add(pl1);
        Players.Add(pl2);
        Players.Add(pl3);
        
        _currentPlayer = Players[_currentPlayerIndex];
        _currentPlayer.StartTurn();
    }

    /// <summary>
    /// Ends the current players turn and starts the next players turn.
    /// </summary>
    public void EndTurn()
    {
        StartCoroutine(StartNextTurn());
    }
    
    /// <summary>
    /// Loops through the list of players and starts their turn, wrapping back to the first player if necessary
    /// </summary>
    private IEnumerator StartNextTurn()
    {
        yield return _timeBetweenTurns;
        _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        _currentPlayer = Players[_currentPlayerIndex];
        _currentPlayer.StartTurn();
    }

    /// <summary>
    /// Removes the current player from the game because they went bankrupt and starts the next players turn
    /// </summary>
    public void RemovePlayer()
    {
        Players.Remove(_currentPlayer);
        Destroy(_currentPlayer.gameObject);
        if (_currentPlayerIndex == Players.Count)
        {
            _currentPlayerIndex = 0;
        }
        
        if (Players.Count == 1)
        {
            UIManager.Instance.ShowWinnerPanel(Players[0]);
        }
        else
        {
            _currentPlayer = Players[_currentPlayerIndex];
            _currentPlayer.StartTurn();
        }
    }
    
    /// <summary>
    /// Gets a list of all the players who can auction, and starts from the current player
    /// </summary>
    public void StartAuction(Property property)
    {
        _auctionProperty = property;
        AuctionPrice = BidAmount;
        UIManager.Instance.UpdateBidButtonAmount(AuctionPrice, BidAmount);
        UIManager.Instance.UpdateAuctionPrice(AuctionPrice);

        _bidders = new List<Player>();
        foreach (var player in Players)
        {
            if (!player.InJail && player.PassedGo && player.Money >= BidAmount)
            {
                _bidders.Add(player);
            }
        }
        
        _currentBidderIndex = _bidders.IndexOf(_currentPlayer);
        if (_currentBidderIndex == -1)
            _currentBidderIndex = 0;
        _currentBidder = _bidders[_currentBidderIndex];
        _currentBidder.BidDecision();
    }

    /// <summary>
    /// Ends the current bidders turn and starts the next, removing them from the list of bidders if they chose to fold
    /// or updating the auction price if they chose to bid
    /// </summary>
    /// <param name="folded">Indicates if the player chose to fold or not</param>
    /// <param name="amount">The amount the player chose to bid</param>
    public void EndBid(bool folded, int amount = 0)
    {
        UIManager.Instance.DisableAuctionButtons();
        
        AuctionPrice += amount;
        
        if (folded)
        {
            _bidders.Remove(_currentBidder);
            if (_currentBidderIndex == _bidders.Count)
            {
                _currentBidderIndex = 0;
            }
        }
        else
        {
            _currentBidderIndex = (_currentBidderIndex + 1) % _bidders.Count;
        }
        
        _currentBidder = _bidders[_currentBidderIndex];
        
        if (_bidders.Count == 1)
        {
            UIManager.Instance.HideAuctionPrompt();
            
            _bidders[0].WinAuction(_auctionProperty, AuctionPrice);
            _currentPlayer.CompleteTurn();
        }
        else
        {
            StartCoroutine(StartNextBid());
        }
    }

    /// <summary>
    /// Waits between each bid before enabling the buttons for the next bidder
    /// </summary>
    private IEnumerator StartNextBid()
    {
        yield return _timeBetweenBids;
        
        UIManager.Instance.UpdateAuctionPrice(AuctionPrice);
        UIManager.Instance.UpdateBidButtonAmount(AuctionPrice, BidAmount);

        _currentBidder.BidDecision();
    }
    
    /// <summary>
    /// Finds the index of a tile by its name.
    /// </summary>
    /// <param name="name">The name of the tile to find.</param>
    /// <returns>The index of the tile with the specified name.</returns>
    public int GetTileIndex(string name)
    {
        return Tiles.FindIndex(tile => tile.Name == name);
    }

    /// <summary>
    /// Returns true if at least 2 players have passed go and are out of jail and have the minimum amount to bid
    /// </summary>
    /// <returns>Whether or not auctioning is possible</returns>
    public bool CanAuction()
    {
        var passedGoCount = Players.Count(player => player.PassedGo);
        var outOfJailCount = Players.Count(player => !player.InJail);
        var minimumToBid = Players.Count(player => player.Money >= BidAmount);
        return passedGoCount > 1 && outOfJailCount > 1 && minimumToBid > 1;
    }
    
    public void GiveTitleDeed(Property property)
    {
        TitleDeeds[property.PropertyNumber] = property;
        UIManager.Instance.UpdateTitleDeedUI(TitleDeeds, _bankInfoPanel);
    }
    
    public void TakeTitleDeed(Property property)
    {
        TitleDeeds[property.PropertyNumber] = null;
        UIManager.Instance.UpdateTitleDeedUI(TitleDeeds, _bankInfoPanel);
    }
}