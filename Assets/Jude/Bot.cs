using UnityEngine;

namespace Jude
{
    public class Bot : Player
    {
        // Code for bot making decisions, will be random for now

        public override void StartTurn()
        {
            Debug.Log(Name + " makes automated decision");
        }

        public Bot(string name, int money) : base(name, money)
        {
        }
    }
}