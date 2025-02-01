using System.Collections.Generic;
using UnityEngine;

namespace Jude
{
    public class Human : MonoBehaviour, IPlayer
    {
        // Code for awaiting user input in human
        
        public string Name { get; set; }
        public int Money { get; set; }
        public List<Property> OwnedProperty { get; set; }
        public Space CurrentSpace { get; set; }

        public void StartTurn()
        {
            Debug.Log(Name + " must make a decision! Roll dice, mortgage or trade or ...");
        }
    }
}