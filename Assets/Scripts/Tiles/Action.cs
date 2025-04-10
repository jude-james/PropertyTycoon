using System.Collections;
using TMPro;
using UnityEngine;

namespace Tiles
{
    public class Action : Tile
    {
        private CardType _cardType;
        private TMP_Text _cardDescription;

        private readonly WaitForSeconds _animationTime = new(4f);
        
        public void SetUp(string name, CardType cardType)
        {
            _cardType = cardType;
            Name = name;
            SetBoardTile();
        }

        protected override void SetCard()
        {
            Card = Instantiate(Resources.Load("Prefabs/Cards/" + _cardType)) as GameObject;
            if (Card != null)
            {
                _cardDescription = Card.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            }
        }

        public override void OnLanded(Player player)
        {
            StartCoroutine(GetAndPerformActionCard(player));
        }

        /// <summary>
        /// Removes the action card from the top of the queue in the board class, shows the card,
        /// then places the card at the bottom of the queue unless the card needs to be retained by the player,
        /// then performs the card action
        /// </summary>
        private IEnumerator GetAndPerformActionCard(Player player)
        {
            ActionCard actionCard = _cardType switch
            {
                CardType.PotLuck => Board.Instance.PotLuckCards.Dequeue(),
                CardType.OppKnock => Board.Instance.OpportunityKnocksCards.Dequeue(),
                _ => null
            };

            if (actionCard == null) yield break;
            
            SetCard();
            AudioManager.Instance.Play("actionCardSound");
            
            var description = actionCard.Description.Substring(1, actionCard.Description.Length - 2).ToUpper();
            _cardDescription.SetText(description);

            yield return _animationTime;
            
            Destroy(Card);
            
            if (actionCard.ActionType == ActionType.GetOutOfJail)
            {
                player.AddGetOutOfJailFreeCard(actionCard);
            }
            else
            {
                if (actionCard.CardType == CardType.PotLuck)
                    Board.Instance.PotLuckCards.Enqueue(actionCard);
                else if (actionCard.CardType == CardType.OppKnock) 
                    Board.Instance.OpportunityKnocksCards.Enqueue(actionCard);
            }
            
            actionCard.PerformAction(player);
        }

        protected override void OnMouseEnter()
        {
            ShowOutline();
        }

        protected override void OnMouseExit()
        {
            HideOutline();
        }
    }
}