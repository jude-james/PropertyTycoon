using System.Collections.Generic;
using Tiles;

/// <summary>
/// Reads Property Tycoon Board and Card Data matrix arrays
/// </summary>
public static class DataInitialiser
{
    private const string Path = "Assets/ExternalFiles/";
    private const string CardDataFileName = "PropertyTycoonCardData(Sheet1).csv";
    private const string BoardDataFileName = "PropertyTycoonBoardData(Sheet1).csv";

    private static GameData _gameData;
    
    public static GameData InitGameData()
    {
        InitSpaces();
        InitCards();
        return _gameData;
    }
        
    private static void InitSpaces()
    {
        // Hard coded values by looking at Exel spreadsheet
        const int boardTiles = 40;
        const int startColumn = 4;
        const int nameRow = 1;
        const int groupRow = 3;
        const int canBeBoughtRow = 5;
        const int costRow = 7;
        const int initialRentRow = 8;
        const int improvedRentRowStart = 10;
        const int improvedRentRowEnd = 14;

        var tiles = new Tile[boardTiles];
        var properties = new List<Property>();
            
        var boardDataMatrix = CSVParser.ReadCSV(Path + BoardDataFileName);
            
        for (int i = 0; i < boardTiles; i++)
        {
            var name = boardDataMatrix[i + startColumn][nameRow];
            var canBeBought = boardDataMatrix[i + startColumn][canBeBoughtRow];
            var group = boardDataMatrix[i + startColumn][groupRow];
                
            if (canBeBought == "Yes")
            {
                var cost = int.Parse(boardDataMatrix[i + startColumn][costRow]);
                    
                if (group == "Station")
                {
                    tiles[i] = new Station(name, cost);
                }
                else if (group == "Utilities")
                {
                    tiles[i] = new Utility(name, cost);
                }
                else
                {
                    var initialRent = int.Parse(boardDataMatrix[i + startColumn][initialRentRow]);
                    var improvedRent = new int[5];
                    for (int j = improvedRentRowStart; j <= improvedRentRowEnd; j++)
                    {
                        var rent = int.Parse(boardDataMatrix[i + startColumn][j]);
                        improvedRent[j - improvedRentRowStart] = rent;
                    }

                    var houseHotelCost = 0;
                    switch (group)
                    {
                        case "Brown":
                        case "Blue":
                            houseHotelCost = 50;
                            break;
                        case "Purple":
                        case "Orange":
                            houseHotelCost = 100;
                            break;
                        case "Red":
                        case "Yellow":
                            houseHotelCost = 150;
                            break;
                        case "Green":
                        case "Deep Blue":
                            houseHotelCost = 200;
                            break;
                    }

                    tiles[i] = new Site(name, cost, group, initialRent, improvedRent, houseHotelCost);
                }
                    
                properties.Add((Property) tiles[i]);
            }
            else
            {
                // TODO continue filtering through other types of tiles- tax space, card space, and odd ones out
                tiles[i] = new Tile(name);
            }
        }

        _gameData.Tiles = tiles;
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