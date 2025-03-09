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
    
    // TODO update bank UI
    private GameObject _bankInfoPanel;
    private TMP_Text _freeParkingSumText;
    
    [SerializeField] private Sprite[] tokens; // temporary

    public Vector2 JailPosition => jailPosition.position;
    
    public List<Tile> Tiles { get; private set; }

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
    
    private Bank _bank;
    
    public Player[] Players { get; private set; }
    private Player _currentPlayer;
    private int _currentPlayerIndex;

    private readonly WaitForSeconds _timeBetweenTurns = new(1);
    
    [SerializeField] private Transform waypointPrefab;
    [SerializeField] private float[,] positions = new float[2,40];
    
    private void Start()
    {
        var dataReader = new DataReader();
        
        dataReader.ReadBoardData(boardTiles);
        Tiles = dataReader.Tiles;
        
        dataReader.ReadCardData();
        var rng = new System.Random();
        PotLuckCards = new Queue<ActionCard>(dataReader.PotLuckCards.ToList().OrderBy(_ => rng.Next()));
        OpportunityKnocksCards = new Queue<ActionCard>(dataReader.OpportunityKnocksCards.ToList().OrderBy(_ => rng.Next()));
        
        // Initially give the bank all the titleDeeds (properties), whilst the player titleDeeds start empty
        var titleDeeds = dataReader.Properties;
        _bank = new Bank(32, 12, titleDeeds);
        
        _bankInfoPanel = UIManager.Instance.BankInfoPanel;
        _freeParkingSumText = UIManager.Instance.FreeParkingInfoPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        
        // Manually assigning players for testing purposes, will get players from main menu later
        // players will also become a list so they can be added and removed once a player declares bankruptcy 
        Players = new Player[4];

        Players[0] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation).AddComponent<Player>();
        Players[0].SetSprite(tokens[0]);
        Players[0].Name = tokens[0].name;
        
        Players[1] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation).AddComponent<Bot>();
        Players[1].SetSprite(tokens[1]);
        Players[1].Name = tokens[1].name;
        
        Players[2] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation).AddComponent<Bot>();
        Players[2].SetSprite(tokens[2]);
        Players[2].Name = tokens[2].name;
        
        Players[3] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation).AddComponent<Bot>();
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
    
    private IEnumerator StartNextTurn()
    {
        yield return _timeBetweenTurns;
        _currentPlayerIndex++;
        _currentPlayer = Players[_currentPlayerIndex % Players.Length];
        _currentPlayer.StartTurn();
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