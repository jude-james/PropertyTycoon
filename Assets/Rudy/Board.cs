using System.Collections.Generic;
using UnityEngine;

namespace Rudy
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

        private Player[] _players = new Player[2]; // Minimum of one player and one computer, will start with 2 players
        private int _currentPlayerIndex = 0;
        private Player _currentPlayer;

        bool endTurn;

        private void Start()
        {
            // For now this is the beginning of the game
            InitData();

            // Logic for initialising players, for now will be done manually until a start of game is implemented
            _players[0] = new Player("cunt1", false);
            _players[1] = new Player("cunt2", false);
            // Will make a menu screen to select amount of players/bots and choose names
        }

        private void Update()
        {
            // way of deciding who goes first, for now will be first index
            _currentPlayer = _players[_currentPlayerIndex % _players.Length];

            NextTurn();

            // once player is completely finished with turn, increment
            _currentPlayerIndex++;
        }

        private void NextTurn()
        {
            endTurn = false;
            // 
        }

        private int RollDice()
        {
            // Returns result of rolling two dice and allows current turn to end
            endTurn = true;
            int dice1 = Random.Range(1, 6);
            int dice2 = Random.Range(1, 6);
            // Might also add screen output showing each dice value
            return dice1 + dice2;
        }

        private void GiveMoney(Player player, int m)
        {
            if (_money >= m)
            {
                _money -= m;
                player.UpdateMoney(m);
            }
            // Not sure what to do if the bank runs out of money
        }

        private void TakeMoney(Player player, int m)
        {
            if (player.GetMoney() >= m)
            {
                _money += m;
                player.UpdateMoney(-m);
            }
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