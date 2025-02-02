using System.Collections.Generic;
using UnityEngine;

namespace Jude
{
    public class Player : MonoBehaviour // Interface?
    {
        public string Name { get; set; }
        public Token Token { get; set; }
        public int Money { get; set; }
        public List<Property> TitleDeeds { get; set; }
        public Space CurrentSpace { get; set; }
        public int GetOutOfJailFreeCards { get; set; }
        public bool InJail { get; set; }
        
        public Player(string name, int money) // Temporary
        {
            Name = name;
            Money = money;
        }
        
        public virtual void StartTurn() // Temporary
        {
        }
    }

    public enum Token
    {
        Boot, Smartphone, Ship, HatStand, Hat, Iron 
    }
}