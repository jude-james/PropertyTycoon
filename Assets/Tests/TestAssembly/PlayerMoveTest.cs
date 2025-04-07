using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;


public class PlayerMoveTest : MonoBehaviour
{
    [Test]
    public void TestPlayerMovement() 
    {

        int currentPosition = 0; // Starting at GO
        
        // hardcode dice roll
        int dice1 = 3;
        int dice2 = 2;
        int moveAmount = dice1 + dice2;
        
        // Calculate new position (with wrap-around logic)
        int newPosition = (currentPosition + moveAmount) % 40; // 40 == board size
        
        TestContext.Out.WriteLine($"Starting Position: {currentPosition}");
        TestContext.Out.WriteLine($"Dice Roll: {dice1} + {dice2} = {moveAmount}");
        TestContext.Out.WriteLine($"New Position: {newPosition}");
        
        Assert.AreEqual(5, newPosition, "Player should move to position 5 after rolling 5");
        Assert.AreEqual(5, moveAmount, "Dice total should be 5");
    }

    [Test]
    public void TestPlayerLandsOnCorrectProperty() 
    {

        string[] propertyNames = new string[] {
            "Go",
            "The Old Creek",
            "Pot Luck",
            "Gangsters Paradise",
            "Income Tax",
            "Brighton Station",
            "The Angels Delight",
            "Opportunity Knocks",
            "Potter Avenue",
            "Granger Drive"
        }; // first 10 props
        
        int currentPosition = 0; // Starting at GO
        
       
        int dice1 = 3;  
        int dice2 = 2; 
        int moveAmount = dice1 + dice2;
        
        int newPosition = (currentPosition + moveAmount) % propertyNames.Length;
        string landedPropertyName = propertyNames[newPosition];
        
        TestContext.Out.WriteLine($"Starting Position: {currentPosition} ({propertyNames[currentPosition]})");
        TestContext.Out.WriteLine($"Dice Roll: {dice1} + {dice2} = {moveAmount}");
        TestContext.Out.WriteLine($"New Position: {newPosition} ({landedPropertyName})");
        
        Assert.GreaterOrEqual(dice1, 1, "First die should be at least 1");
        Assert.LessOrEqual(dice1, 6, "First die should be at most 6");
        Assert.GreaterOrEqual(dice2, 1, "Second die should be at least 1");
        Assert.LessOrEqual(dice2, 6, "Second die should be at most 6");
        
        Assert.AreEqual(5, moveAmount, "Total movement should be 5");
        Assert.AreEqual(5, newPosition, "Player should land on position 5");
        Assert.AreEqual("Brighton Station", landedPropertyName, "Player should land on Brighton Station");
    }
}
