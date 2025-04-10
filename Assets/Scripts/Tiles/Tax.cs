using TMPro;

namespace Tiles
{
    public class Tax : Tile
    {
        private int _amount;
        
        public void SetUp(string name, int amount)
        {
            _amount = amount;
            base.SetUp(name);
        }
        
        protected override void SetBoardTile()
        {
            base.SetBoardTile();
            if (transform.childCount > 0)
            {
                var amountText = transform.GetChild(1).GetComponent<TMP_Text>();
                amountText.SetText("PAY £"+_amount);
            }
        }
        
        public override void OnLanded(Player player)
        {
            AudioManager.Instance.Play("taxSound");
            player.TakeMoney(_amount);
            player.CompleteTurn();
        }
    }
}