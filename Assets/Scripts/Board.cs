using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    [SerializeField] private Sprite[] tokens; // temporary

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
    
    public Player[] Players { get; private set; }
    private Player _currentPlayer;
    private int _currentPlayerIndex;

    private readonly List<Player> _bidders = new();
    private Player _currentBidder;
    private int _currentBidderIndex;
    public int AuctionPrice { get; private set; }
    public int BidAmount { get; private set; } = 20;
    private Property _auctionProperty;
    
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
        // players will also become a list so they can be added and removed once a player declares bankruptcy 
        Players = new Player[4];

        Players[0] = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Player>();
        Players[0].SetSprite(tokens[0]);
        Players[0].Name = tokens[0].name;
        
        Players[1] = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Player>();
        Players[1].SetSprite(tokens[1]);
        Players[1].Name = tokens[1].name;
        
        Players[2] = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Bot>();
        Players[2].SetSprite(tokens[2]);
        Players[2].Name = tokens[2].name;
        
        Players[3] = Instantiate(playerPrefab, Tiles[0].transform.position, Quaternion.identity).AddComponent<Bot>();
        Players[3].SetSprite(tokens[3]);
        Players[3].Name = tokens[3].name;
        
        _currentPlayer = Players[_currentPlayerIndex % Players.Length];
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
        _currentPlayerIndex++;
        _currentPlayer = Players[_currentPlayerIndex % Players.Length];
        _currentPlayer.StartTurn();
    }
    
    /// <summary>
    /// Gets a list of all the players who can auction, and starts from the current player
    /// </summary>
    public void StartAuction(Property property)
    {
        _auctionProperty = property;
        AuctionPrice = 20;
        UIManager.Instance.UpdateBidButtonAmount(AuctionPrice, BidAmount);
        
        foreach (var player in Players)
        {
            if (!player.InJail) // TODO && player.PassedGo...
            {
                _bidders.Add(player);
            }
        }
        
        _currentBidderIndex = _bidders.IndexOf(_currentPlayer);
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