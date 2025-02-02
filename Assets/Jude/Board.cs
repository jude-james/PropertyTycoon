using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jude
{
    /// <summary>
    /// Property Tycoon board, acts as a game manager. Tracks board spaces, bank, cards, and players
    /// </summary>
    public class Board : MonoBehaviour
    {
        private Space[] _spaces;
        
        //TODO turn into Dictionary of string and event action, so action is stored as actual code instead of a string like currently 
        private Dictionary<string, string> _opportunityKnocksCardData = new();
        private Dictionary<string, string> _potLuckCardData = new();

        private Bank _bank;
        private Player[] _players;

        private int _currentSpaceIndex = 0;
        private int _currentPlayerIndex = 0;
        private Player _currentPlayer;
        
        private void Start()
        {
            // For now this is the beginning of the game
            GameData gameData = DataInitialiser.InitGameData();
            _spaces = gameData.Spaces;
            _opportunityKnocksCardData = gameData.OpportunityKnocksCards;
            _potLuckCardData = gameData.PotLuckCards;

            var properties = gameData.Properties;
            _bank = new Bank(50_000, 32, 12, properties);
            
            // For now, we will start with 2 players who are humans
            _players = new Player[2];
            _players[0] = gameObject.AddComponent<Human>();
            _players[0].Name = "Mark";
            _players[0].Money = 1500;
            _players[1] = gameObject.AddComponent<Human>();
            _players[1].Name = "Sarah";
            _players[0].Money = 1500;
            
            for (var i = 0; i < _players.Length; i++)
            {
                _players[i].CurrentSpace = _spaces[_currentSpaceIndex];
            }
            
            // TODO move below into update method and add state machine 
            
            // way of deciding who goes first, for now will be first index
            _currentPlayer = _players[_currentPlayerIndex % _players.Length];
            _currentPlayer.StartTurn();

            // once player is completely finished with turn AKA they press "end turn", increment
            _currentPlayerIndex++;
            
            // when in player turn state:
            // player can roll, mortgage, sell, build
            // once rolled and moving squares is finished, player can still mortgage, sell build, until player chooses end turn option
            
            PrintValues();
        }

        private void PrintValues()
        {
            // Testing
            foreach (var site in _spaces.OfType<Site>())
            {
                Debug.Log(site.Name);
                foreach (var rent in site.ImprovedRent)
                {
                    Debug.Log(rent);
                }
            }
        }
    }
}