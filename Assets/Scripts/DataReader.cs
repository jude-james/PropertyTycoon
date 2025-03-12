using System;
using System.Collections.Generic;
using Tiles;
using UnityEngine;
using Action = Tiles.Action;

/// <summary>
/// Reads and initialises board and card data into the game from editable CSV files
/// </summary>
public class DataReader
{
    private const string Path = "Assets/CSVFiles/";
    private const string BoardDataFileName = "PropertyTycoonBoardDataImproved(Sheet1).csv";
    private const string CardDataFileName = "PropertyTycoonCardDataImproved(Sheet1).csv";
    
    public List<Tile> Tiles { get; private set; }
    public List<Property> Properties { get; private set; }
    
    public Queue<ActionCard> PotLuckCards { get; private set; }
    public Queue<ActionCard> OpportunityKnocksCards { get; private set; }
    
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
        var propertyNumber = 0;

        var boardDataMatrix = CSVParser.ReadCSV(Path + BoardDataFileName);
        
        for (int i = 0; i < boardTiles.childCount; i++)
        {
            var name = boardDataMatrix[i+startCol][nameRow];
            var type = boardDataMatrix[i+startCol][typeRow];
            
            switch (type)
            {
                case "Property":
                {
                    var cost = int.Parse(boardDataMatrix[i+startCol][costRow]);
                    var subtype = boardDataMatrix[i+startCol][subtypeRow];
                    switch (subtype)
                    {
                        case "Street":
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
                            street.SetUp(name, cost, propertyNumber, set, initialRent, rentWithColourSet, improvedRent, houseCost, hotelCost);
                            Tiles.Add(street);
                            break;
                        }
                        case "Station":
                        {
                            var rent1 = int.Parse(boardDataMatrix[i+startCol][stationRent1Row]);
                            var rent2 = int.Parse(boardDataMatrix[i+startCol][stationRent2Row]);
                            var rent3 = int.Parse(boardDataMatrix[i+startCol][stationRent3Row]);
                            var rent4 = int.Parse(boardDataMatrix[i+startCol][stationRent4Row]);
                    
                            var station = boardTiles.GetChild(i).gameObject.AddComponent<Station>();
                            station.SetUp(name, cost, propertyNumber, rent1, rent2, rent3, rent4);
                            Tiles.Add(station);
                            break;
                        }
                        case "Utility":
                        {
                            var rent1 = int.Parse(boardDataMatrix[i+startCol][utilRent1Row]);
                            var rent2 = int.Parse(boardDataMatrix[i+startCol][utilRent2Row]);
                            var utilType = boardDataMatrix[i+startCol][specificRow];

                            var utility = boardTiles.GetChild(i).gameObject.AddComponent<Utility>();
                            utility.SetUp(name, cost, propertyNumber, rent1, rent2, utilType);
                            Tiles.Add(utility);
                            break;
                        }
                    }
                
                    Properties.Add((Property) Tiles[^1]);
                    propertyNumber++;
                    break;
                }
                case "Tax":
                {
                    var amount = int.Parse(boardDataMatrix[i+startCol][taxAmountRow]);
                
                    var tax = boardTiles.GetChild(i).gameObject.AddComponent<Tax>();
                    tax.SetUp(name, amount);
                    Tiles.Add(tax);
                    break;
                }
                case "Action":
                {
                    var cardTypeStr = boardDataMatrix[i+startCol][subtypeRow];
                    Enum.TryParse(cardTypeStr, out CardType cardType);

                    var action = boardTiles.GetChild(i).gameObject.AddComponent<Action>();
                    action.SetUp(name, cardType);
                    Tiles.Add(action);
                    break;
                }
                case "Jail":
                {
                    var jail = boardTiles.GetChild(i).gameObject.AddComponent<Jail>();
                    jail.SetUp(name);
                    Tiles.Add(jail);
                    break;
                }
                case "FreeParking":
                {
                    var freeParking = boardTiles.GetChild(i).gameObject.AddComponent<FreeParking>();
                    freeParking.SetUp(name);
                    Tiles.Add(freeParking);
                    break;
                }
                default:
                {
                    // Go & Just Visiting are remaining
                    var tile = boardTiles.GetChild(i).gameObject.AddComponent<Tile>();
                    tile.SetUp(name);
                    Tiles.Add(tile);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Reads the card data from a CSV file and sets up the opportunity knocks and pot luck cards.
    /// </summary>
    public void ReadCardData()
    {
        var startColumn = 2;
        var endColumn = 18;
        PotLuckCards = ReadActionCards(startColumn, endColumn, CardType.PotLuck);
        
        startColumn = 20;
        endColumn = 35;
        OpportunityKnocksCards = ReadActionCards(startColumn, endColumn, CardType.OppKnock);
    }

    private Queue<ActionCard> ReadActionCards(int startColumn, int endColumn, CardType cardType)
    {
        // Hard coded values by looking at Exel spreadsheet
        const int descriptionRow = 0;
        const int actionTypeRow = 1;
        const int amountRow = 2;
        const int houseRepairAmountRow = 3;
        const int hotelRepairAmountRow = 4;
        const int tileNameRow = 5;
        const int directionRow = 6;
        const int moveAmountRow = 7;

        var actionCards = new Queue<ActionCard>();
        
        var cardDataMatrix = CSVParser.ReadCSV(Path + CardDataFileName);

        for (int i = startColumn; i <= endColumn; i++)
        {
            ActionCard actionCard;
            
            var description = cardDataMatrix[i][descriptionRow];
            var actionTypeStr = cardDataMatrix[i][actionTypeRow];
            
            Enum.TryParse(actionTypeStr, out ActionType actionType);
            
            switch (actionType)
            {
                case ActionType.GiveMoney or ActionType.TakeMoney or ActionType.AddToFreeParking or ActionType.CollectMoney:
                {
                    var amount = int.Parse(cardDataMatrix[i][amountRow]); 
                    actionCard = new ActionCard(description, actionType, cardType, amount);
                    break;
                }
                case ActionType.TakeBuildingMoney:
                {
                    var houseRepairAmount = int.Parse(cardDataMatrix[i][houseRepairAmountRow]);
                    var hotelRepairAmount = int.Parse(cardDataMatrix[i][hotelRepairAmountRow]);
                    actionCard = new ActionCard(description, actionType, cardType, houseRepairAmount: houseRepairAmount, hotelRepairAmount: hotelRepairAmount);
                    break;
                }
                case ActionType.MoveTo:
                {
                    var tileName = cardDataMatrix[i][tileNameRow];
                    var directionStr = cardDataMatrix[i][directionRow]; 
                    Enum.TryParse(directionStr, out Direction direction);

                    actionCard = new ActionCard(description, actionType, cardType, tileName:tileName, direction: direction);
                    break;
                }
                case ActionType.MoveBy:
                {
                    var moveAmount = int.Parse(cardDataMatrix[i][moveAmountRow]);
                    actionCard = new ActionCard(description, actionType, cardType, moveAmount: moveAmount);
                    break;
                }
                default:
                    actionCard = new ActionCard(description, actionType, cardType);
                    break;
            }
            
            actionCards.Enqueue(actionCard);
        }

        return actionCards;
    }
}