public class ActionCard
{
    public string Description { get; private set; }
    public ActionType ActionType { get; }
    public CardType CardType { get; private set; }
    private readonly int _amount;
    private readonly int _houseRepairAmount;
    private readonly int _hotelRepairAmount;
    private readonly string _tileName;
    private readonly Direction _direction;
    private readonly int _moveAmount;
    
    public ActionCard(string description, ActionType actionType, CardType cardType, int amount = 0, int houseRepairAmount = 0,
        int hotelRepairAmount = 0, string tileName = null, Direction direction = Direction.Shortest, int moveAmount = 0)
    {
        ActionType = actionType;
        CardType = cardType;
        Description = description;
        _amount = amount;
        _houseRepairAmount = houseRepairAmount;
        _hotelRepairAmount = hotelRepairAmount;
        _tileName = tileName;
        _direction = direction;
        _moveAmount = moveAmount;
    }

    public string getName() {
        return Description;
    }

    /// <summary>
    /// Performs the description of the action card on the player
    /// </summary>
    /// <param name="player"> The player that took the action card </param>
    public void PerformAction(Player player)
    {
        switch (ActionType)
        {
            case ActionType.GiveMoney:
                player.GiveMoney(_amount);
                player.CompleteTurn();
                break;
            
            case ActionType.TakeMoney:
                player.TakeMoney(_amount);
                player.CompleteTurn();
                break;
            
            case ActionType.CollectMoney:
                foreach (var otherPlayer in Board.Instance.Players)
                {
                    if (otherPlayer == player) continue;
                    otherPlayer.TakeMoney(_amount);
                    player.GiveMoney(_amount);
                }
                player.CompleteTurn();
                break;
            
            case ActionType.TakeBuildingMoney:
                player.TakeMoney(_houseRepairAmount * player.Houses);
                player.TakeMoney(_hotelRepairAmount * player.Hotels);
                player.CompleteTurn();
                break;
            
            case ActionType.MoveTo:
                player.SetNewTileIndex(Board.Instance.GetTileIndex(_tileName));
                player.MoveToTile(_direction);
                break;
            
            case ActionType.MoveBy:
                player.ShiftTileIndex(_moveAmount);
                player.MoveToTile(_direction);
                break;
            
            case ActionType.AddToFreeParking:
                Board.Instance.FreeParkingSum += _amount;
                player.TakeMoney(_amount);
                player.CompleteTurn();
                break;
            
            case ActionType.GoToJail:
                player.GoToJail(false);
                break;
            
            default:
                player.CompleteTurn();
                break;
        }
    }
}