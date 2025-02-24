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

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuPlayer"/> class.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="token">The token of the player.</param>
    /// <param name="isBot">Indicates if the player is a bot.</param>
    public MenuPlayer(string name, int token, bool isBot)
    {
        this.name = name;
        this.token = token;
        this.isBot = isBot;
    }
}
