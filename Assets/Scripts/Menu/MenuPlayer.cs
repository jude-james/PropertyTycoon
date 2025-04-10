/// <summary>
/// Represents a player in the game menu, including their name, token, and bot status.
/// </summary>
public class MenuPlayer
{
    /// <summary>
    /// The name of the player.
    /// </summary>
    public string name { get; private set; }
    /// <summary>
    /// The token of the player. 0=Boot, 1=Smartphone, 2=Ship, 3=Hatstand, 4=Cat, 5=Iron.
    /// </summary>
    public int token { get; private set; }
    /// <summary>
    /// Indicates if the player is a bot.
    /// </summary>
    public bool isBot { get; private set; }
    public bool isSmart { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuPlayer"/> class.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="token">The token of the player.</param>
    /// <param name="isBot">Indicates if the player is a bot.</param>
    /// <param name="isSmart">Indicates if the bot uses the smart class</param>
    public MenuPlayer(string name, int token, bool isBot,bool isSmart)
    {
        this.name = name;
        this.token = token;
        this.isBot = isBot;
        this.isSmart = isSmart;
    }
}
