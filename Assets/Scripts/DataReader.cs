using System.Collections.Generic;
using Tiles;
using UnityEngine;

public class DataReader
{
    private const string Path = "Assets/CSVFiles/";
    private const string BoardDataFileName = "PropertyTycoonBoardDataImproved(Sheet1).csv";
    private const string CardDataFileName = "PropertyTycoonCardData(Sheet1).csv";
    
    public List<Tile> Tiles { get; private set; }
    public List<Property> Properties { get; private set; }
    public Dictionary<string, string> OpportunityKnocksCards { get; private set; }
    public Dictionary<string, string> PotLuckCards { get; private set; }

    /// <summary>
    /// Reads the board data from a CSV file and sets up the board tiles.
    /// </summary>
    /// <param name="boardTiles">The transform of the board tiles.</param>
    public void ReadBoardData(Transform boardTiles)
    {
        // Hard coded values by looking at Exel spreadsheet
        const int startCol = 1;

        const int nameRow = 0;
        const int typeRow = 1;
        const int subtypeRow = 2;
        const int costRow = 3;
        const int initialRentRow = 4;
        const int improvedRentRowStart = 5;
        const int improvedRentRowEnd = 9;
        const int setRow = 10;
        const int houseCostRow = 11;
        const int hotelCostRow = 12;
        const int utilRent1Row = 13;
        const int utilRent2Row = 14;
        const int stationRent1Row = 15;
        const int stationRent2Row = 16;
        const int stationRent3Row = 17;
        const int stationRent4Row = 18;
        const int taxAmountRow = 19;
        const int specificRow = 20;
        
        Tiles = new List<Tile>();
        Properties = new List<Property>();
        
        var boardDataMatrix = CSVParser.ReadCSV(Path + BoardDataFileName);
        
        for (int i = 0; i < boardTiles.childCount; i++)
        {
            var name = boardDataMatrix[i+startCol][nameRow];
            var type = boardDataMatrix[i+startCol][typeRow];
            
            if (type == "Property")
            {
                var cost = int.Parse(boardDataMatrix[i+startCol][costRow]);
                var subtype = boardDataMatrix[i+startCol][subtypeRow];
                if (subtype == "Street")
                {
                    var set = boardDataMatrix[i+startCol][setRow];
                    var initialRent = int.Parse(boardDataMatrix[i+startCol][initialRentRow]);
                    var rentWithColourSet = initialRent * 2;
                    
                    var improvedRent = new int[improvedRentRowEnd - improvedRentRowStart + 1];
                    for (int j = improvedRentRowStart; j <= improvedRentRowEnd; j++)
                    {
                        var rent = int.Parse(boardDataMatrix[i+startCol][j]);
                        improvedRent[j - improvedRentRowStart] = rent;
                    }

                    var houseCost = int.Parse(boardDataMatrix[i+startCol][houseCostRow]);
                    var hotelCost = int.Parse(boardDataMatrix[i+startCol][hotelCostRow]);
                    
                    var street = boardTiles.GetChild(i).gameObject.AddComponent<Street>();
                    street.SetUp(name, cost, set, initialRent, rentWithColourSet, improvedRent, houseCost, hotelCost);
                    Tiles.Add(street);
                }
                else if (subtype == "Station")
                {
                    var rent1 = int.Parse(boardDataMatrix[i+startCol][stationRent1Row]);
                    var rent2 = int.Parse(boardDataMatrix[i+startCol][stationRent2Row]);
                    var rent3 = int.Parse(boardDataMatrix[i+startCol][stationRent3Row]);
                    var rent4 = int.Parse(boardDataMatrix[i+startCol][stationRent4Row]);
                    
                    var station = boardTiles.GetChild(i).gameObject.AddComponent<Station>();
                    station.SetUp(name, cost, rent1, rent2, rent3, rent4);
                    Tiles.Add(station);
                }
                else if (subtype == "Utility")
                {
                    var rent1 = int.Parse(boardDataMatrix[i+startCol][utilRent1Row]);
                    var rent2 = int.Parse(boardDataMatrix[i+startCol][utilRent2Row]);
                    var utilType = boardDataMatrix[i+startCol][specificRow];

                    var utility = boardTiles.GetChild(i).gameObject.AddComponent<Utility>();
                    utility.SetUp(name, cost, rent1, rent2, utilType);
                    Tiles.Add(utility);
                }
                
                Properties.Add((Property) Tiles[^1]);
            }
            else if (type == "Tax")
            {
                var amount = int.Parse(boardDataMatrix[i+startCol][taxAmountRow]);
                
                var tax = boardTiles.GetChild(i).gameObject.AddComponent<Tax>();
                tax.SetUp(name, amount);
                Tiles.Add(tax);
            }
            else if (type == "Action")
            {
                var cardType = boardDataMatrix[i+startCol][subtypeRow];
                
                var actionCard = boardTiles.GetChild(i).gameObject.AddComponent<ActionCard>();
                actionCard.SetUp(name, cardType);
                Tiles.Add(actionCard);
            }
            else if (type == "Jail")
            {
                var jail = boardTiles.GetChild(i).gameObject.AddComponent<Jail>();
                jail.SetUp(name);
                Tiles.Add(jail);
            }
            else
            {
                // just visiting & free parking are left
                var tile = boardTiles.GetChild(i).gameObject.AddComponent<Tile>();
                tile.SetUp(name);
                Tiles.Add(tile);
            }
        }
    }
    
    /// <summary>
    /// Reads the card data from a CSV file and sets up the opportunity knocks and pot luck cards.
    /// </summary>
    public void ReadCardData()
    {
        // Hard coded values by looking at Exel spreadsheet
        const int descriptionRow = 0;
        const int actionRow = 3;
            
        var startColumn = 5;
        var endColumn = 21;
            
        var cardDataMatrix = CSVParser.ReadCSV(Path + CardDataFileName);
            
        OpportunityKnocksCards = new Dictionary<string, string>();
        PotLuckCards = new Dictionary<string, string>();
            
        for (int i = startColumn; i <= endColumn; i++)
        {
            PotLuckCards.Add(cardDataMatrix[i][descriptionRow], cardDataMatrix[i][actionRow]);
        }
            
        startColumn = 26;
        endColumn = 41;

        for (int i = startColumn; i <= endColumn; i++)
        {
            OpportunityKnocksCards.Add(cardDataMatrix[i][descriptionRow], cardDataMatrix[i][actionRow]);
        }
    }
}