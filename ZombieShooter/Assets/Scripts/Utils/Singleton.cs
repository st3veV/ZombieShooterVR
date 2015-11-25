using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour
{

    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("Singleton - " + typeof (T));
                instance = go.AddComponent<T>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
}
