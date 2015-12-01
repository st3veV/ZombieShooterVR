using UnityEngine;

namespace Utils
{
    public class AutoObject<T>:MonoBehaviour where T:MonoBehaviour
    {
        public static T Create()
        {
            var go = new GameObject(typeof (T) + "");
            return go.AddComponent<T>();
        }
    }
}