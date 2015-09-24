using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponDatabase : ScriptableObject {

    private static string databaseName = "WeaponDB";
    private static WeaponDatabase _instance;
    public static WeaponDatabase Instance
    {
        get
        {
            _instance = Resources.Load(databaseName, typeof(WeaponDatabase)) as WeaponDatabase;
            if(_instance == null)
            {
                _instance = ScriptableObject.CreateInstance<WeaponDatabase>();
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(_instance, "Assets/Resources/"+databaseName+".asset");
#endif
            }
            if (_instance.weapons == null)
            {
                _instance.weapons = new List<IWeapon>();
            }
            return _instance;
        }
    }

    public List<IWeapon> weapons;

}
