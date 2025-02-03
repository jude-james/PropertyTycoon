using System.Collections.Generic;
using UnityEngine;

namespace Jude
{
    /// <summary>
    /// Property Tycoon board, acts as a game manager. Tracks board spaces, bank, cards, and players
    /// </summary>
    public class Board : MonoBehaviour
    {
        [SerializeField] private Space[] spaces;
        [SerializeField] private Player[] players;

        private Bank _bank;

        private Dictionary<string, string> _opportunityKnocksCardData = new();
        private Dictionary<string, string> _potLuckCardData = new();
        
        private int _currentSpaceIndex = 0;
        private int _currentPlayerIndex = 0;
        private Player _currentPlayer;
        
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
            players[0].Money = 1500;

            players[1] = new GameObject(pl2Name).AddComponent<Human>().GetComponent<Human>();
            players[1].Name = pl2Name;
            players[1].Money = 1500;
            
            // set all players to go
            for (var i = 0; i < players.Length; i++)
            {
                players[i].CurrentSpace = spaces[_currentSpaceIndex];
            }
            
            // TODO move below into update method and add state machine 
            
            // way of deciding who goes first, for now will be first index
            _currentPlayer = players[_currentPlayerIndex % players.Length];
            _currentPlayer.StartTurn();

            // once player is completely finished with turn AKA they press "end turn", increment and start over
            _currentPlayerIndex++;
            
            // when in player turn state:
            // player can roll, mortgage, sell, build
            // once rolled and moving squares is finished, player can still mortgage, sell build, until player chooses end turn option
            
            PrintValues();
        }

        private void PrintValues()
        {
        }
    }
}