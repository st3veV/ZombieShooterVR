using UnityEngine;

namespace Utils
{
    public static class ObjUtil
    {
        public static bool IsNull(object o)
        {
            if (o is Component)
                return (o as Component) == null;
            if (o is GameObject)
                return (o as GameObject) == null;
            return o == null;
        }
    }

}