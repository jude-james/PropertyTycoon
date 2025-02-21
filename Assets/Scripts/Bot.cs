using System.Collections;
using UnityEngine;

public class Bot : Player
{
    protected override void RollDiceDecision()
    {
        // Bot can choose to 'click' roll dice, or do the other options... auction, build, sell, trade...
        OnRollDice();
    }

    protected override void EndTurnDecision()
    {
        // Bot can choose to 'click' end turn, or do the other options... auction, build, sell, trade...
        OnEndTurn();
    }

    public override void ForSaleDecision(int cost)
    {
        // bot can choose to buy or auction, currently it is buying
        
        // bot should check it has enough money first
        
        // Show the prompt even though it's a bot, the user should see it still
        StartCoroutine(ForSaleDecisionCoroutine(cost));
    }

    private IEnumerator ForSaleDecisionCoroutine(int cost)
    {
        // TODO make buttons uninteractable
        UIManager.Instance.ShowForSalePrompt(cost);
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HideForSalePrompt();
        
        OnBuy();
    }
}