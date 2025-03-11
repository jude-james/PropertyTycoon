using System.Collections;
using Tiles;
using UnityEngine;

/// <summary>
/// AI Agent inherited from player class. Makes its own moves and decisions
/// </summary>
public class Bot : Player
{
    private readonly WaitForSeconds _decisionMakingTime = new(2.5f); // Simulate bot thinking and give time for UI elements to show
    
    protected override void RollDiceDecision()
    {
        // Bot can choose to 'click' roll dice
        OnRollDice();
    }

    protected override void EndTurnDecision()
    {
        // Bot can choose to 'click' end turn, or, at this point, it can choose the other options... auction, build, sell, trade...
        OnEndTurn();
    }

    protected override void InJailDecision()
    {
        UIManager.Instance.ShowInJailPrompt(false, false, false);

        StartCoroutine(InJailDecisionCoroutine());
    }

    private IEnumerator InJailDecisionCoroutine()
    {
        yield return _decisionMakingTime;
        
        // Bot can either post bail, use card or stay in jail
        // Bot should check if it has enough money or has enough getOutOfJailFreeCards first
        
        OnPostBail();
    }
    
    public override void ForSaleDecision(Property property)
    {
        UIManager.Instance.ShowForSalePrompt(false, false, property);

        StartCoroutine(ForSaleDecisionCoroutine(property));
    }

    private IEnumerator ForSaleDecisionCoroutine(Property property)
    {
        yield return _decisionMakingTime;
        
        // bot can choose to buy or auction
        // Bot should check it has enough money first
        
        OnBuy();
    }
}