using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Property Tycoon board, acts as a game manager. Tracks board spaces, bank, cards, and players
/// </summary>
public class Board : MonoBehaviour
{
    [SerializeField] private Space[] spaces;
    [SerializeField] private Player[] players;   
    [SerializeField] private Transform waypointPrefab;
    [SerializeField] private float[,] positions = new float[2,40];

    //holds the player sprites, currently only 2 players in the game
    [SerializeField] private Sprite PlayerSprites1;
    [SerializeField] private Sprite PlayerSprites2;

    private Bank _bank;

    private Dictionary<string, string> _opportunityKnocksCardData = new();
    private Dictionary<string, string> _potLuckCardData = new();
    
    private int _currentPlayerIndex = 0;
    private Player _currentPlayer;

    private bool endTurn;
    
    private void Start()
    {
        // For now this is the beginning of the game
        var gameData = DataInitialiser.InitGameData();
        spaces = gameData.Spaces;
        _opportunityKnocksCardData = gameData.OpportunityKnocksCards;
        _potLuckCardData = gameData.PotLuckCards;
        
        var titleDeeds = gameData.Properties;
        _bank = new Bank(32, 12, titleDeeds);
        
        // For now, we will start with 2 players who are humans
        players = new Player[2];
        var pl1Name = "Mark";
        var pl2Name = "Sarah";
        
        players[0] = new GameObject(pl1Name).AddComponent<Human>().GetComponent<Human>();
        players[0].Name = pl1Name;

        //Sets the player's sprite and starting position (24,-24 is Go)
        players[0].setSprite(PlayerSprites1);
        players[0].transform.position = new Vector2(24, -24);

        players[1] = new GameObject(pl2Name).AddComponent<Human>().GetComponent<Human>();
        players[1].Name = pl2Name;


        players[1].setSprite(PlayerSprites2);
        players[1].transform.position = new Vector2(24, -24);
        
        foreach (var player in players)
        {
            player.CurrentSpace = spaces[0];
        }


        positionWaypoints();
        giveSpacesPositions();

        
        // TODO add some sort of state machine to switch states to avoid endless if statements and booleans
        // when in player turn state:
        // player can roll, mortgage, trade, build
        // once rolled and moving squares is finished, player can still mortgage, trade, build, until player chooses end turn option

        //builds the waypoints
        
        PrintValues();

        StartCoroutine(Game());
    }
    
    private IEnumerator Game() // This function and NextTurn are quite shitty but it's is all I could get working - It will be changed
    {
        while (true)
        {
            // loop through players
            _currentPlayer = players[_currentPlayerIndex % players.Length];

            endTurn = false;
            //Starts turn then waits for endTurn to become true
            StartCoroutine(NextTurn(_currentPlayer));
            while (true) 
            {
                if (endTurn == true)
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
        int landedPos = player.Move(RollDice()) % spaces.Length;
        Space landedSpace = spaces[landedPos];
        _currentPlayer.CurrentSpace = landedSpace;
        _currentPlayer.setPosition(_currentPlayer.CurrentSpace.getPosition());
        // The plan was to implement spaces using a linked list which we will do if needed when coding the space class

        Debug.Log(_currentPlayer.Name + " Landed at position: " + landedPos);
        Debug.Log(_currentPlayer.Name + " Landed at space: " + _currentPlayer.CurrentSpace.Name);

        Debug.Log("Press space to end turn");
        while (true) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                yield return null; 
                break;
            } 
            yield return null;  
        }
        endTurn = true;
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

    private void PrintValues()
    {
        foreach (var keyValuePair in _opportunityKnocksCardData)
        {
            Debug.Log("Description:" + keyValuePair.Key + " - Action:" + keyValuePair.Value);
        }
        
        foreach (var keyValuePair in _potLuckCardData)
        {
            Debug.Log("Description:" + keyValuePair.Key + " - Action:" + keyValuePair.Value);
        }
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
            spaces[i].setPosition(positions[0,i],positions[1,i]);
        }
    }
}