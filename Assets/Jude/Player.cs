using System.Collections.Generic;
using UnityEngine;

namespace Jude
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Token Token { get; set; }
        [field: SerializeField] public int Money { get; set; }
        [field: SerializeField] public List<Property> TitleDeeds { get; set; }
        [field: SerializeField] public Space CurrentSpace { get; set; }
        [field: SerializeField] public int GetOutOfJailFreeCards { get; set; }
        [field: SerializeField] public bool InJail { get; set; }
        
        public virtual void StartTurn() // Temporary
        {
        }
    }

    public enum Token
    {
        Boot, Smartphone, Ship, HatStand, Hat, Iron 
    }
}