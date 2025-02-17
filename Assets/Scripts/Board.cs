using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;

/// <summary>
/// Property Tycoon board, acts as a game manager. Tracks board tiles, bank, cards, and players
/// </summary>
public class Board : Singleton<Board>
{
    [field: SerializeField] public List<Tile> Tiles { get; private set; }
    
    [SerializeField] private Player[] players;
    
    [SerializeField] private Sprite[] tokens; // temporary until character select is done

    [SerializeField] private Transform boardTiles;
    [SerializeField] private Player playerPrefab;

    private Bank _bank;
    private Dictionary<string, string> _opportunityKnocksCardData = new();
    private Dictionary<string, string> _potLuckCardData = new();
    
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
        
        // Initially give the bank all the titleDeeds (properties), whilst the player titleDeeds start empty
        var titleDeeds = dataReader.Properties;
        _bank = new Bank(32, 12, titleDeeds);
        
        dataReader.ReadCardData();
        _opportunityKnocksCardData = dataReader.OpportunityKnocksCards;
        _potLuckCardData = dataReader.PotLuckCards;
        
        // For now, we will start with humans, and testing all 6 tokens
        players = new Player[tokens.Length];

        for (var i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(playerPrefab, Tiles[0].transform.position, transform.rotation);
            players[i].SetSprite(tokens[i]);
            players[i].Name = tokens[i].name;
            players[i].CurrentTile = Tiles[0];
        }
        
        _currentPlayer = players[_currentPlayerIndex % players.Length];
        _currentPlayer.StartTurn();
        
        positionWaypoints();
        giveSpacesPositions();
        
        // StartCoroutine(Game());
    }

    public void EndTurn()
    {
        StartCoroutine(StartNextTurn());
    }
    
    private IEnumerator StartNextTurn()
    {
        yield return _timeBetweenTurns;
        _currentPlayerIndex++;
        _currentPlayer = players[_currentPlayerIndex % players.Length];
        _currentPlayer.StartTurn();
    }
    
    /*
    private IEnumerator Game() // This function and NextTurn are quite shitty but it's is all I could get working - It will be changed
    {
        while (true)
        {
            // loop through players
            _currentPlayer = players[_currentPlayerIndex % players.Length];

            _endTurn = false;
            //Starts turn then waits for endTurn to become true
            StartCoroutine(NextTurn(_currentPlayer));
            while (true) 
            {
                if (_endTurn == true)
                {
                    break; 
                } 
                yield return null; 
            }
            Debug.Log(_currentPlayer.Name + " turn over");

            // once player is completely finished with turn AKA they press "end turn", increment and start over
            _currentPlayerIndex++;
        }
    }
    
    private IEnumerator NextTurn(Player player)
    {
        // Input will be added later, for now the player will just move

        // Movement
        int landedPos = player.Move(RollDice()) % Tiles.Count;
        Tile landedTile = Tiles[landedPos];
        _currentPlayer.CurrentTile = landedTile;
        
        // I have swapped to the manual points here, makes it a little easier, and for squares like just visiting the players sit in the corner
        _currentPlayer.transform.position = _currentPlayer.CurrentTile.transform.position;
        // _currentPlayer.transform.position = _currentPlayer.CurrentTile.getPosition();
        
        // The plan was to implement spaces using a linked list which we will do if needed when coding the space class

        Debug.Log(_currentPlayer.Name + " Landed at position: " + landedPos);
        //Debug.Log(_currentPlayer.Name + " Landed at space: " + _currentPlayer.CurrentTile.Name);

        Debug.Log("Press space to end turn");
        while (true) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yield return null; 
                break;
            } 
            yield return null;  
        }
        _endTurn = true;
    }

    private int RollDice()
    {
        // Returns result of rolling two dice
        int dice1 = Random.Range(1, 6);
        int dice2 = Random.Range(1, 6);
        Debug.Log("Dice 1: " + dice1);
        Debug.Log("Dice 2: " + dice2);
        // Will add screen output showing each dice value
        return dice1 + dice2;
    }
    */
    
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