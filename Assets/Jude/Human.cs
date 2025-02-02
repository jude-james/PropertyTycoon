using UnityEngine;

namespace Jude
{
    public class Human : Player
    {
        // Code for awaiting user input
        
        public override void StartTurn()
        {
            Debug.Log(Name + " must make a decision! Roll dice, mortgage or trade or ...");
        }

        public Human(string name, int money) : base(name, money)
        {
        }
    }
}