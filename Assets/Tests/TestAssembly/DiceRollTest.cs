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
    int dice1 = Random.Range(1, 7);  // Simulates first die roll (1-6)
    int dice2 = Random.Range(1, 7);  // Simulates second die roll (1-6)
    
    // Act
    int total = dice1 + dice2;
    
    // Assert
    TestContext.Out.WriteLine($"Dice 1: {dice1}");
    TestContext.Out.WriteLine($"Dice 2: {dice2}");
    TestContext.Out.WriteLine($"Sum: {total}");
    
    Assert.GreaterOrEqual(dice1, 1, "First die should be at least 1");
    Assert.LessOrEqual(dice1, 6, "First die should be at most 6");
    Assert.GreaterOrEqual(dice2, 1, "Second die should be at least 1");
    Assert.LessOrEqual(dice2, 6, "Second die should be at most 6");
    Assert.GreaterOrEqual(total, 2, "Total should be at least 2");
    Assert.LessOrEqual(total, 12, "Total should be at most 12");
    Assert.AreEqual(dice1 + dice2, total, "Total should equal sum of dice");

}
}
