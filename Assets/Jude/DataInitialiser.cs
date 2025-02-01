using System.Collections.Generic;

namespace Jude
{
    /// <summary>
    /// Reads Property Tycoon Board and Card Data matrix arrays
    /// </summary>
    public static class DataInitialiser
    {
        private const string Path = "Assets/Jude/"; // Temporary path
        private const string CardDataFileName = "PropertyTycoonCardData(Sheet1).csv";
        private const string BoardDataFileName = "PropertyTycoonBoardData(Sheet1).csv";

        private static GameData _gameData;
        
        /// <summary>
        /// Initialises spaces and cards
        /// </summary>
        /// <returns> GameData struct that stores all data needed for game to start </returns>
        public static GameData InitGameData()
        {
            InitSpaces();
            InitCards();
            return _gameData;
        }
        
        private static void InitSpaces()
        {
            // Hard coded values by looking at Exel spreadsheet
            const int boardSpaces = 40;
            const int startColumn = 4;
            const int nameRow = 1;
            const int groupRow = 3;
            const int canBeBoughtRow = 5;

            var spaces = new Space[boardSpaces];
            var properties = new List<Property>();
            
            var boardDataMatrix = CSVParser.ReadCSV(Path + BoardDataFileName);
            
            // TODO continue filtering through varies types of spaces
            
            for (int i = 0; i < boardSpaces; i++)
            {
                var name = boardDataMatrix[i + startColumn][nameRow];
                var canBeBought = boardDataMatrix[i + startColumn][canBeBoughtRow];
                var group = boardDataMatrix[i + startColumn][groupRow];
                
                if (canBeBought == "Yes")
                {
                    if (group == "Station")
                    {
                        spaces[i] = new Station(name);
                    }
                    else if (group == "Utilities")
                    {
                        spaces[i] = new Utility(name);
                    }
                    else
                    {
                        spaces[i] = new Site(name);
                    }
                    
                    properties.Add((Property) spaces[i]);
                }
                else
                {
                    spaces[i] = new Space(name);
                }
            }

            _gameData.Spaces = spaces;
            _gameData.Properties = properties;
        }

        private static void InitCards()
        {
            // Hard coded values by looking at Exel spreadsheet
            const int descriptionRow = 0;
            const int actionRow = 3;
            
            var startColumn = 5;
            var endColumn = 21;
            
            var cardDataMatrix = CSVParser.ReadCSV(Path + CardDataFileName);
            
            var opportunityKnocksCards = new Dictionary<string, string>();
            var potLuckCards = new Dictionary<string, string>();
            
            for (int i = startColumn; i <= endColumn; i++)
            {
                potLuckCards.Add(cardDataMatrix[i][descriptionRow], cardDataMatrix[i][actionRow]);
            }
            
            startColumn = 26;
            endColumn = 41;

            for (int i = startColumn; i <= endColumn; i++)
            {
                opportunityKnocksCards.Add(cardDataMatrix[i][descriptionRow], cardDataMatrix[i][actionRow]);
            }

            _gameData.PotLuckCards = potLuckCards;
            _gameData.OpportunityKnocksCards = opportunityKnocksCards;
        }
    }
}