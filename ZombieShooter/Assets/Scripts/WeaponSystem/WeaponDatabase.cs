using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class WeaponDatabase : ScriptableObject {
    private const string databaseName = "WeaponDB";
    private const string databasePath = "Assets/Resources/" + databaseName + ".asset";

    private static WeaponDatabase _instance;
    public static WeaponDatabase Instance
    {
        get
        {
            _instance = Resources.Load(databaseName, typeof(WeaponDatabase)) as WeaponDatabase;
            if (_instance == null)
            {
#if UNITY_EDITOR
                _instance = (WeaponDatabase)AssetDatabase.LoadAssetAtPath(databasePath, typeof(WeaponDatabase));
#endif
                if (_instance == null)
                {
                    _instance = CreateInstance<WeaponDatabase>();
#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(_instance, databasePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
#endif
                }
            }
            
            if (_instance.Weapons == null)
            {
                _instance.Weapons = new List<Weapon>();
            }
            return _instance;
        }
    }

    [SerializeField]
    public List<Weapon> Weapons;

    void OnEnable()
    {
        if(Weapons == null)
            Weapons = new List<Weapon>();
    }
}
