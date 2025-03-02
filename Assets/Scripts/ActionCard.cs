using UnityEngine;

public class ActionCard
{
    public string Description { get; private set; }
    public ActionType ActionType { get; private set; }
    public int Amount { get; private set; }
    public int HouseRepairAmount { get; private set; }
    public int HotelRepairAmount { get; private set; }
    public string TileName { get; private set; }
    public string Direction { get; private set; }
    public int MoveAmount { get; private set; }
    public bool Retained { get; private set; }
    
    public ActionCard(string description, ActionType actionType, int amount = 0, int houseRepairAmount = 0,
        int hotelRepairAmount = 0, string tileName = null, string direction = null, int moveAmount = 0,
        bool retained = false)
    {
        ActionType = actionType;
        Description = description;
        Amount = amount;
        HouseRepairAmount = houseRepairAmount;
        HotelRepairAmount = hotelRepairAmount;
        TileName = tileName;
        Direction = direction;
        MoveAmount = moveAmount;
        Retained = retained;
    }

    /// <summary>
    /// Performs the action card description on the player
    /// </summary>
    /// <param name="player"> The player that took the action card </param>
    public void PerformAction(Player player)
    {
        Debug.Log("Performing action on " + player.Name);
        player.CompleteTurn();
    }
}

public enum ActionType
{
    GiveMoney,
    TakeMoney,
    CollectMoney,
    TakeBuildingMoney,
    MoveTo,
    MoveBy,
    AddToFreeParking,
    GoToJail,
    GetOutOfJail
}