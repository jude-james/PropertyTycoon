using UnityEngine;

namespace Tiles
{
    public class Action : Tile
    {
        private string _cardType;
        
        public void SetUp(string name, string cardType)
        {
            _cardType = cardType;
            base.SetUp(name);
        }

        protected override void SetCard()
        {
            Card = Instantiate(Resources.Load("Prefabs/Cards/" + _cardType)) as GameObject;
        }

        public override void OnLanded(Player player)
        {
            //ShowCard();
            
            // get queue from board class
            // take the top of the queue
            // Remove quotes and capitalise the description string
            // set the card text to the description
            // animate and show the card for x amount of seconds
            // perform action
            // place card at bottom of queue (unless it's a get out of jail card)
            // so peak (show and perform action) -> dequeue -> enqueue the dequeued card
            
            player.CompleteTurn();
        }

        protected override void OnMouseEnter()
        {
        }

        protected override void OnMouseExit()
        {
        }
    }
}