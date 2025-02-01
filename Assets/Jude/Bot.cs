using System.Collections.Generic;
using UnityEngine;

namespace Jude
{
    public class Bot : MonoBehaviour, IPlayer
    {
        // Code for bot making decisions, will be random for now
        
        public string Name { get; set; }
        public int Money { get; set; }
        public List<Property> OwnedProperty { get; set; }
        public Space CurrentSpace { get; set; }

        public void StartTurn()
        {
            Debug.Log(Name + " makes automated decision");
        }
    }
}