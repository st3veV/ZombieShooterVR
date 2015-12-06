using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour
{

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("Singleton - " + typeof (T));
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
}
