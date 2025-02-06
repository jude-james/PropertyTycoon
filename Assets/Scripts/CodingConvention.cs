using UnityEngine;

public class CodingConvention : MonoBehaviour
{
    [SerializeField] private int number; // camel case for unity serialised fields

    //private int _number; // underscore for private variables
    
    // Explicit access modifier, AKA 'private void Foo()' instead of 'void Foo()'
    private void Foo()
    {
        
    }

    // We shall use auto properties for simplification
    public int Number { get; set; }
    
    // is equivalent to

    private int _number;
    
    public int GetNumber() 
    {
        return number;
    }
}