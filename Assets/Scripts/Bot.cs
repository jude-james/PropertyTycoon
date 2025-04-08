using System.Collections;
using Tiles;
using UnityEngine;

/// <summary>
/// AI Agent inherited from player class. Makes its own moves and decisions
/// </summary>
public class Bot : Player
{
    private readonly WaitForSeconds _decisionMakingTime = new(2.5f); // Simulate bot thinking and give time for UI elements to show
    private readonly WaitForSeconds _bidDecisionMakingTime = new(1f);
    
    protected override void RollDiceDecision()
    {
        // Bot can choose to 'click' roll dice, that is the only thing it can do here
        // Instead of waiting for user input, this function is overriden to manually press the button, same goes for all functions in the bot class
        OnRollDice();
    }

    protected override void EndTurnDecision()
    {
        // Bot can choose to 'click' end turn, or at this point, it can choose the other options:
        // build, sell buildings, mortgage, unmortgage and sell property
        
        // For this simple bot, it will always end its turn
        OnEndTurn();
    }

    protected override void InJailDecision()
    {
        UIManager.Instance.ShowInJailPrompt(false, false, false);
        UIManager.Instance.ShowBotDecisionDialog();
        
        StartCoroutine(InJailDecisionCoroutine());
    }

    private IEnumerator InJailDecisionCoroutine()
    {
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();

        // Bot can choose to post bail, use card or stay in jail
        
        // In this simple version, the bot will try and use its card first, then try and pay 50, then finally remain in jail
        if (GetOutOfJailFreeCards.Count > 0) // <- Make sure bot checks it can do things, or game breaks without if statements
        {
            OnGetOutOfJailFree();
        }
        else if (Money >= PostBailAmount)
        {
            OnPostBail();
        }
        else
        {
            OnRemainInJail();
        }
    }
    
    public override void ForSaleDecision(Property property)
    {
        UIManager.Instance.ShowForSalePrompt(false, false, property);
        UIManager.Instance.ShowBotDecisionDialog();

        StartCoroutine(ForSaleDecisionCoroutine(property));
    }

    private IEnumerator ForSaleDecisionCoroutine(Property property)
    {
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // Bot can choose to buy the property or auction it
        
        // In this simple version, the bot will always buy if it can, if it can't it will then auction
        if (Money >= property.Cost)
        {
            OnBuy();
        }
        else
        {
            OnAuction();
        }
    }

    public override void BidDecision()
    {
        CurrentBidder = this;
        
        UIManager.Instance.UpdateAuctionPrompt(false, false, Name, Sprite);
        UIManager.Instance.ShowBotDecisionDialog();

        StartCoroutine(BidDecisionCoroutine());
    }

    private IEnumerator BidDecisionCoroutine()
    {
        yield return _bidDecisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // Bot can chose to bid or fold
        
        // In this simple version the bot will always bid if it can
        var canBid = Money >= Board.Instance.AuctionPrice + Board.Instance.BidAmount;
        if (canBid)
        {
            OnBid();
        }
        else
        {
            OnFold();
        }
    }

    protected override void RaiseFundsDecision()
    {
        UIManager.Instance.ShowBotDecisionDialog();
        
        StartCoroutine(RaiseFundsDecisionCoroutine());
    }

    private IEnumerator RaiseFundsDecisionCoroutine()
    {
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // Bot must raise funds when this function is called
        
        // In this simple version, bot will loop through all mortgageable properties
        // and mortgage them one by one until it has positive money
        // then it does the same with all sellable properties
        
        if (CanMortgage())
        {
            var mortgageableProperties = GetMortgageableProperties();
            foreach (var property in mortgageableProperties)
            {
                property.Mortgage();
                if (Money >= 0)
                {
                    CompleteTurn();
                    yield break; // <- equivalent to return for IEnumerator 
                }
            }
        }
        
        if (CanSellProperty())
        {
            var sellableProperties = GetSellableProperties();
            foreach (var property in sellableProperties)
            {
                property.SellProperty();
                if (Money >= 0)
                {
                    CompleteTurn();
                    yield break;
                }
            }
        }
        
        // bot should also loop through all houses and hotels first but that has not been implemented yet
    }
}