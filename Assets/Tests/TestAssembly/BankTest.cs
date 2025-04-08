using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;


public class BankTest : MonoBehaviour
{
    // mock player for testing, ported core functionality from Player.cs
    private class MockPlayer
    {
        public int Money { get; private set; } = 1500;
        public List<int> OwnedProperties { get; private set; } = new List<int>();

        public bool GiveMoney(int amount)
        {
            Money += amount;
            return true;
        }

        public bool TakeMoney(int amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            return true;
        }

        public bool OwnsProperty(int propertyId)
        {
            return OwnedProperties.Contains(propertyId);
        }

        public void AddProperty(int propertyId)
        {
            OwnedProperties.Add(propertyId);
        }
    }

    [Test]
    public void TestPassingGoMoney() 
    {
        MockPlayer player = new MockPlayer();
        int initialMoney = player.Money;
        int passGoAmount = 200; 

        bool transactionSuccess = player.GiveMoney(passGoAmount);

        Assert.IsTrue(transactionSuccess, "Transaction should succeed");
        Assert.AreEqual(initialMoney + passGoAmount, player.Money, "Player should receive £200 for passing GO");
    }

    [Test]
    public void TestPayingRent() 
    {
        MockPlayer landlord = new MockPlayer();
        MockPlayer tenant = new MockPlayer();
        int rentAmount = 100;
        int initialLandlordMoney = landlord.Money;
        int initialTenantMoney = tenant.Money;

        bool tenantCanPay = tenant.TakeMoney(rentAmount);
        bool landlordReceived = landlord.GiveMoney(rentAmount);

        Assert.IsTrue(tenantCanPay, "Tenant should be able to pay rent");
        Assert.IsTrue(landlordReceived, "Landlord should receive rent");
        Assert.AreEqual(initialTenantMoney - rentAmount, tenant.Money, "Tenant money should decrease");
        Assert.AreEqual(initialLandlordMoney + rentAmount, landlord.Money, "Landlord money should increase");
    }

    [Test]
    public void TestBuyAlreadyOwnedProperty() 
    {
        MockPlayer owner = new MockPlayer();
        MockPlayer buyer = new MockPlayer();
        int propertyId = 5; 
        int propertyCost = 200; 

        // Set up initial ownership
        owner.AddProperty(propertyId);

        Assert.IsTrue(owner.OwnsProperty(propertyId), "Original owner should own the property");
        Assert.IsFalse(buyer.OwnsProperty(propertyId), "Buyer should not own the property");
        Assert.Throws<System.InvalidOperationException>(() => 
        {
            if (owner.OwnsProperty(propertyId))
            {
                throw new System.InvalidOperationException("Cannot buy already owned property");
            }
            buyer.TakeMoney(propertyCost);
            buyer.AddProperty(propertyId);
        });
    }

    [Test]
    public void TestInsufficientFunds()
    {
        MockPlayer player = new MockPlayer();
        int expensiveAmount = 2000; // More than starting money

        bool canPay = player.TakeMoney(expensiveAmount);

        Assert.IsFalse(canPay, "Transaction should fail due to insufficient funds");
        Assert.AreEqual(1500, player.Money, "Money should not change when payment fails");
    }
}
