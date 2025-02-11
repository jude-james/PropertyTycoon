using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Property Tycoon board, acts as a game manager. Tracks board tiles, bank, cards, and players
/// </summary>
public class Board : MonoBehaviour
{
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private Player[] players;
    
    private Bank _bank;

    private Dictionary<string, string> _opportunityKnocksCardData = new();
    private Dictionary<string, string> _potLuckCardData = new();

    [SerializeField] private GameObject tileUIs;
    
    private int _currentPlayerIndex = 0;
    private Player _currentPlayer;

    private bool _endTurn;
    
    private void Start()
    {
        var dataReader = new DataReader();
        dataReader.ReadData();
        tiles = dataReader.Tiles;
        _opportunityKnocksCardData = dataReader.OpportunityKnocksCards;
        _potLuckCardData = dataReader.PotLuckCards;

        var titleDeeds = dataReader.Properties;
        _bank = new Bank(32, 12, titleDeeds);
        
        // Assign each internal tile with a tileUI gameObject, and vice versa. And set each tile it's card
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].TileUI = tileUIs.transform.GetChild(i).GetComponent<TileUI>();
            tileUIs.transform.GetChild(i).GetComponent<TileUI>().Tile = tiles[i];
            tiles[i].SetCard();
            tiles[i].SetTileUI();
        }
        
        // For now, we will start with 2 players who are humans
        players = new Player[2];
        var pl1Name = "Mark";
        var pl2Name = "Sarah";

        players[0] = new GameObject(pl1Name).AddComponent<Human>().GetComponent<Human>();
        players[0].Name = pl1Name;

        players[1] = new GameObject(pl2Name).AddComponent<Human>().GetComponent<Human>();
        players[1].Name = pl2Name;

        foreach (var player in players)
        {
            player.CurrentTile = tiles[0];
        }

        // TODO add some sort of state machine to switch states to avoid endless if statements and booleans
        // when in player turn state:
        // player can roll, mortgage, trade, build
        // once rolled and moving squares is finished, player can still mortgage, trade, build, until player chooses end turn option

        StartCoroutine(Game());
    }
    
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
        int landedPos = player.Move(RollDice()) % tiles.Count;
        Tile landedTile = tiles[landedPos];
        _currentPlayer.CurrentTile = landedTile;
        _currentPlayer.transform.position = _currentPlayer.CurrentTile.Position;
        // The plan was to implement spaces using a linked list which we will do if needed when coding the space class

        Debug.Log(_currentPlayer.Name + " Landed at position: " + landedPos);
        Debug.Log(_currentPlayer.Name + " Landed at space: " + _currentPlayer.CurrentTile.Name);

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
}