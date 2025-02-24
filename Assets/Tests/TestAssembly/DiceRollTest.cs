using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;


public class DiceRollTest : MonoBehaviour
{
[Test]
public void CorrectDiceRoll() {
    // basic passing test -- if this doesn't compile it's not a code error
    // it's a unity setting/package
    int dice1 = 3;
    int dice2 = 2;
    int total = dice1 + dice2;
    TestContext.Out.WriteLine($"Sum: {total}"); // for testrunner
    Assert.AreEqual(5, total);

}

// below does not work -- need to ask Rudy/Jude abt adding new namespace for player/board/tile etc
// public void PlayerMovesToCorrectTile()
// {
//     // Arrange
//     Player player = new Player();
//     player.CurrentTile = Board.Instance.Tiles[0]; // Start on the first tile
//     board.Tiles = new List<Tile>
//     {
//         new Tile { name = "Start" },
//         new Tile { name = "Property 1" },
//         new Tile { name = "Property 2" },
//         new Tile { name = "Property 3" },
//         new Tile { name = "Property 4" }
//     };
//     player.CurrentTile = board.Tiles[0]; // Start on the first tile

//     // Act
//     Tile landedTile = player._testMoveToTile(2, 3); // Move 5 spaces

//     TestContext.Out.WriteLine($"landedTile: {landedTile}");

//     // Assert
//     Assert.AreEqual("Brighton Station", landedTile.name); // Check if the player landed on the correct tile
// }
}
