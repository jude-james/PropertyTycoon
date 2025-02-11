using System.Collections.Generic;
using Tiles;

public class DataReader
{
    private const string Path = "Assets/ExternalFiles/";
    private const string BoardDataFileName = "PropertyTycoonBoardDataImproved(Sheet1).csv";
    private const string CardDataFileName = "PropertyTycoonCardData(Sheet1).csv";

    public List<Tile> Tiles { get; private set; }
    public List<Property> Properties { get; private set; }
    public Dictionary<string, string> OpportunityKnocksCards { get; private set; }
    public Dictionary<string, string> PotLuckCards { get; private set; }

    public void ReadData()
    {
        ReadBoardData();
        ReadCardData();
    }
    
    private void ReadBoardData()
    {
        // Hard coded values by looking at Exel spreadsheet
        const int startColumn = 1;

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
        
        for (int i = startColumn; i < boardDataMatrix.Count; i++)
        {
            var name = boardDataMatrix[i][nameRow];
            var type = boardDataMatrix[i][typeRow];
            
            if (type == "Property")
            {
                var cost = int.Parse(boardDataMatrix[i][costRow]);
                var subtype = boardDataMatrix[i][subtypeRow];
                if (subtype == "Street")
                {
                    var set = boardDataMatrix[i][setRow];
                    var initialRent = int.Parse(boardDataMatrix[i][initialRentRow]);
                    var rentWithColourSet = initialRent * 2;
                    
                    var improvedRent = new int[improvedRentRowEnd - improvedRentRowStart + 1];
                    for (int j = improvedRentRowStart; j <= improvedRentRowEnd; j++)
                    {
                        var rent = int.Parse(boardDataMatrix[i][j]);
                        improvedRent[j - improvedRentRowStart] = rent;
                    }

                    var houseCost = int.Parse(boardDataMatrix[i][houseCostRow]);
                    var hotelCost = int.Parse(boardDataMatrix[i][hotelCostRow]);

                    Tiles.Add(new Street(name, cost, set, initialRent, rentWithColourSet, improvedRent, houseCost, hotelCost));
                }
                else if (subtype == "Station")
                {
                    var rent1 = int.Parse(boardDataMatrix[i][stationRent1Row]);
                    var rent2 = int.Parse(boardDataMatrix[i][stationRent2Row]);
                    var rent3 = int.Parse(boardDataMatrix[i][stationRent3Row]);
                    var rent4 = int.Parse(boardDataMatrix[i][stationRent4Row]);
                    
                    Tiles.Add(new Station(name, cost, rent1, rent2, rent3, rent4));
                }
                else if (subtype == "Utility")
                {
                    var rent1 = int.Parse(boardDataMatrix[i][utilRent1Row]);
                    var rent2 = int.Parse(boardDataMatrix[i][utilRent2Row]);
                    var utilType = boardDataMatrix[i][specificRow];

                    Tiles.Add(new Utility(name, cost, rent1, rent2, utilType));
                }
                
                Properties.Add((Property) Tiles[^1]);
            }
            else if (type == "Tax")
            {
                int amount = int.Parse(boardDataMatrix[i][taxAmountRow]);
                Tiles.Add(new Tax(name, amount));
            }
            else if (type == "Action")
            {
                var cardType = boardDataMatrix[i][subtypeRow];
                Tiles.Add(new ActionCard(name, cardType));
            }
            else if (type == "Jail")
            {
                Tiles.Add(new Jail(name));
            }
            else
            {
                // just visiting & free parking are left
                Tiles.Add(new Tile(name));
            }
        }
    }
        
    private void ReadCardData()
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