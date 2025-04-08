using System.Collections;
using Tiles;
using UnityEngine;

public class SmartBot : Player
{
    private readonly WaitForSeconds _decisionMakingTime = new(2.5f); // Simulate bot thinking and give time for UI elements to show
    
    protected override void RollDiceDecision()
    {
        OnRollDice();
    }

    protected override void EndTurnDecision()
    {
        // ----------------------CODE GOES HERE----------------------
        // Check Bot.cs for simple version
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

        // ----------------------CODE GOES HERE----------------------
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
        
        // ----------------------CODE GOES HERE----------------------
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
        yield return _decisionMakingTime;
        
        UIManager.Instance.HideBotDecisionDialog();
        
        // ----------------------CODE GOES HERE----------------------
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
        
        // ----------------------CODE GOES HERE----------------------
    }
}
