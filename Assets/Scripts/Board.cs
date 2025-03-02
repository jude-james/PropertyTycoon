using System.Collections;
using System.Collections.Generic;
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
    
    // TODO update bank UI
    private GameObject _bankInfoPanel;
    private TMP_Text _freeParkingSumText;
    
    [SerializeField] private Sprite[] tokens; // temporary

    public Vector2 JailPosition => jailPosition.position;
    
    public List<Tile> Tiles { get; private set; }

    public Queue<ActionCard> PotLuckCards { get; private set; }
    public Queue<ActionCard> OpportunityKnocksCards { get; private set; }
    
    public int FreeParkingSum
    {
        get => _freeParkingSum;
        set
        {
            _freeParkingSum = value;
            StartCoroutine(UIManager.Instance.AnimateMoney(_freeParkingSumText, _freeParkingSum));
        }
    }
    private int _freeParkingSum;

    private Player[] _players;
    
    private Bank _bank;
    
    private Player _currentPlayer;
    private int _currentPlayerIndex;

    private readonly WaitForSeconds _timeBetweenTurns = new(1);
    
    [SerializeField] private Transform waypointPrefab;
    [SerializeField] private float[,] positions = new float[2,40];
    
    // This is temporary and not very robust, will change when action cards are implemented
    // Key tiles
    // TODO just make some sort of findIndex(name), probably already exists
    public int justVisitingIndex = 10;
    public int goIndex = 0;

    private void Start()
    {
        var dataReader = new DataReader();
        
        dataReader.ReadBoardData(boardTiles);
        Tiles = dataReader.Tiles;
        
        dataReader.ReadCardData();
        PotLuckCards = dataReader.PotLuckCards;
        OpportunityKnocksCards = dataReader.OpportunityKnocksCards;
                
        foreach (var actionCard in OpportunityKnocksCards)
        {
            Debug.Log(actionCard.ActionType);
        }
        
        // Initially give the bank all the titleDeeds (properties), whilst the player titleDeeds start empty
        var titleDeeds = dataReader.Properties;
        _bank = new Bank(32, 12, titleDeeds);
        
        _bankInfoPanel = UIManager.Instance.BankInfoPanel;
        _freeParkingSumText = UIManager.Instance.FreeParkingInfoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        
        // Manually assigning players for testing purposes, will get players from main menu later
        _players = new Player[2];

        _players[0] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation).AddComponent<Player>();
        _players[0].SetSprite(tokens[0]);
        _players[0].Name = tokens[0].name;
        
        _players[1] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation).AddComponent<Bot>();
        _players[1].SetSprite(tokens[1]);
        _players[1].Name = tokens[1].name;
        
        _currentPlayer = _players[_currentPlayerIndex % _players.Length];
        _currentPlayer.StartTurn();
    }

    /// <summary>
    /// Ends the current player's turn and starts the next player's turn.
    /// </summary>
    public void EndTurn()
    {
        StartCoroutine(StartNextTurn());
    }
    
    private IEnumerator StartNextTurn()
    {
        yield return _timeBetweenTurns;
        _currentPlayerIndex++;
        _currentPlayer = _players[_currentPlayerIndex % _players.Length];
        _currentPlayer.StartTurn();
    }
    
    /*Creates space position on the board using absolute values, 
    this is probably not the most practical implementation
    but the easiest i could think of for now without manually creating 40 different 
    waypoint objects and placing them.

    It also has a horizontal bias added, so if the board is move horizontally, the points should move with it.
    */
    private void positionWaypoints()
    {
        //change in position from last point
        float change;
        //finds the horizontal movement of the board from the center
        float bias = transform.position.x;

        //this is bottom right corner
        Instantiate(waypointPrefab,new Vector2(24 + bias,-24),new Quaternion(),transform);
        positions[0,0] = 24 + bias;
        positions[1,0] = -24;

        //bottom right --> bottom left
        for (int i = 0; i < 9;i++)
        {
            change = i*((float)4.5);
            Instantiate(waypointPrefab,new Vector2(18 - change + bias,-24),new Quaternion(),transform);
            positions[0,i+1] = 18 - change + bias;
            positions[1,i+1] = -24;
        }

        //bottom left corner
        Instantiate(waypointPrefab,new Vector2(-24 + bias,-24),new Quaternion(),transform);
        positions[0,10] = -24 + bias;
        positions[1,10] = -24;
        //bottom left --> top left
        for (int i = 0; i < 9;i++)
        {
            change = i*((float)4.5);
            Instantiate(waypointPrefab,new Vector2(-24 + bias,-18 + change),new Quaternion(),transform);
            positions[0,i+11] = -24 + bias;
            positions[1,i+11] = -18 + change;
        }

        //top left corner
        Instantiate(waypointPrefab,new Vector2(-24 + bias,24),new Quaternion(),transform);
        positions[0,20] = -24 + bias;
        positions[1,20] = 24;
        //top left --> top right
        for (int i = 0; i < 9;i++)
        {
            change = i*((float)4.5);
            Instantiate(waypointPrefab,new Vector2(-18 + change + bias,24),new Quaternion(),transform);
            positions[0,i+21] = -18 + change + bias;
            positions[1,i+21] = 24;
        }

        //top right corner
        Instantiate(waypointPrefab,new Vector2(24 + bias,24),new Quaternion(),transform);
        positions[0,30] = 24 + bias;
        positions[1,30] = 24;
        //top right --> bottom right
        for (int i = 0; i < 9;i++)
        {
            change = i*((float)4.5);
            Instantiate(waypointPrefab,new Vector2(24 + bias,18 - change),new Quaternion(),transform);
            positions[0,i+31] = 24 + bias;
            positions[1,i+31] = 18 - change;
        }
    }

    //This just assigns the space position to the different waypoints
    private void giveSpacesPositions()
    {
        for (int i = 0; i < 40; i++)
        {
            Tiles[i].setPosition(positions[0,i],positions[1,i]);
        }
    }
}