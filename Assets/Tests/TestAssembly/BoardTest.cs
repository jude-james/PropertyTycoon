using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;


public class BoardTest : MonoBehaviour
{
    [Test]
    public void BoardInitizedCorrectly() {
        // test board reading logic
        string[] lines = System.IO.File.ReadAllLines("Assets/CSVFiles/PropertyTycoonBoardDataImproved(Sheet1).csv");
        string[] propertyNames = new string[lines.Length - 1]; // skip headers

        for (int i = 1; i < lines.Length; i++) {
            string[] columns = lines[i].Split(',');
            propertyNames[i - 1] = columns[0];
        }

        Assert.AreEqual(40, propertyNames.Length, "Board should have 40 properties");
        Assert.AreEqual("Brighton Station", propertyNames[5], "6th property should be Brighton Station");
        Assert.AreEqual("Granger Drive", propertyNames[9], "10th property should be Granger Drive");
        Assert.AreEqual("Tesla Power Co", propertyNames[12], "13th property should be Tesla Power Co");
    }
}
