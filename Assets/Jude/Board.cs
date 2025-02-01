using System.Collections.Generic;
using UnityEngine;

namespace Jude
{
    /// <summary>
    /// Property Tycoon board, acts as a game manager. Tracks board spaces, money, cards, and players
    /// </summary>
    public class Board : MonoBehaviour //TODO separate this and a bank class? 
    {
        // 40 spaces on a monopoly board so hard coding this for now
        private Space[] _spaces = new Space[40];
        
        //TODO turn these into Dictionary of string and event action, so action is stored as actual code instead of a string like currently 
        private Dictionary<string, string> _potLuckCardData = new();
        private Dictionary<string, string> _opportunityKnocksCardData = new();

        // These variables are more for the bank, for now they will stay in the board class
        private int _money = 50_000; // The money the bank has, according to EM breakdown bank has total £50k so will start with that
        private List<Property> _ownedProperty; // must be list so properties can be removed/added and allocated to players 
        
        private IPlayer[] _players = new IPlayer[2]; // Minimum of one player and one computer, will start with 2 players
        private int _currentPlayerIndex = 0;
        private IPlayer _currentPlayer;
        
        private void Start()
        {
            // For now this is the beginning of the game
            InitData();
            
            // Logic for initialising players, for now will be done manually until a start of game is implemented
            _players[0] = gameObject.AddComponent<Human>();
            _players[0].Name = "Mark";
            _players[1] = gameObject.AddComponent<Human>();
            _players[1].Name = "Sarah";
            
            // TODO move below into update method and add state machine 
            
            // way of deciding who goes first, for now will be first index
            _currentPlayer = _players[_currentPlayerIndex % _players.Length];
            _currentPlayer.StartTurn(); // temporary

            // once player is completely finished with turn, increment
            _currentPlayerIndex++;
        }

        private void InitData()
        {
            GameData gameData = DataInitialiser.InitGameData();
            _spaces = gameData.Spaces;
            _ownedProperty = gameData.Properties;
            _opportunityKnocksCardData = gameData.OpportunityKnocksCards;
            _potLuckCardData = gameData.PotLuckCards;
            
            // Print values
            foreach (var property in _ownedProperty)
            {
                Debug.Log("Name: " + property.GetName() + " - Type: " + property.GetType());
            }
            
            foreach (var space in _spaces)
            {
                Debug.Log("Name: " + space.GetName() + " Type: " + space.GetType());
            }
        
            foreach (var keyValuePair in _potLuckCardData)
            {
                Debug.Log("Description:" + keyValuePair.Key + " Action: " + keyValuePair.Value);
            }
            
            foreach (var keyValuePair in _opportunityKnocksCardData)
            {
                Debug.Log("Description:" + keyValuePair.Key + " Action: " + keyValuePair.Value);
            }
        }
    }
}