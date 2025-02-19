public class MenuPlayer
{
    public string name { get; private set; }
    public int token { get; private set; } // 0=Boot 1=Smartphone 2=Ship 3=Hatstand 4=Cat 5=Iron
    public bool isBot { get; private set; }

    public MenuPlayer(string name, int token, bool isBot)
    {
        this.name = name;
        this.token = token;
        this.isBot = isBot;
    }
}
