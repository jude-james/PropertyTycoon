using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<T>();
            }
            
            return _instance;
        }
    }
}