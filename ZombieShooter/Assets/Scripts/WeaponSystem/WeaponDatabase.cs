using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[InitializeOnLoad]
public class WeaponDatabase : ScriptableObject {
    private const string databaseName = "WeaponDB";
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
            if (_instance.Weapons == null)
            {
                _instance.Weapons = new List<IWeapon>();
            }
            return _instance;
        }
    }

    [SerializeField]
    public List<IWeapon> Weapons;

}
