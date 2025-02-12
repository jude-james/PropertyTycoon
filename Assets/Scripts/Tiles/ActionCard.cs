using UnityEngine;

namespace Tiles
{
    public class ActionCard : Tile
    {
        [SerializeField] private string cardType;
        
        public void SetUp(string name, string cardType)
        {
            this.cardType = cardType;
            base.SetUp(name);
        }

        public override void OnLanded(Player player)
        {
        }
    }
}