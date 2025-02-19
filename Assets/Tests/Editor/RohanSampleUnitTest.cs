using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Tiles;

public class TestDiceRoll
{
    private Board board;
    private Player player;

    [SetUp]
    public void Setup()
    {
        GameObject boardObject = new GameObject();
        board = boardObject.AddComponent<Board>();

        GameObject playerObject = new GameObject();
        player = playerObject.AddComponent<Player>();

        board.Tiles = new List<Tile>
        {
            new Tile { name = "Start" },
            new Tile { name = "Property 1" },
            new Tile { name = "Property 2" },
            new Tile { name = "Property 3" },
            new Tile { name = "Property 4" }
        };

        player.CurrentTile = board.Tiles[0]; // is this right?
    }

    [Test]
    public void PlayerMovesThreeSpots()
    {
        // manually roll dice to 3
        player.DiceRoll = 3;

        // move the player
        player.LandOnTile();

        Assert.AreEqual("Property 3", player.CurrentTile.name); // todo: get exact property 
    }
}