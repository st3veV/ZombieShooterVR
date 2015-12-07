using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class WeaponDatabase : ScriptableObject {
    private const string DatabaseName = "WeaponDB";
    private const string DatabasePath = "Assets/Resources/" + DatabaseName + ".asset";

    private static WeaponDatabase _instance;
    public static WeaponDatabase Instance
    {
        get
        {
            _instance = Resources.Load(DatabaseName, typeof(WeaponDatabase)) as WeaponDatabase;
            if (_instance == null)
            {
#if UNITY_EDITOR
                _instance = (WeaponDatabase)AssetDatabase.LoadAssetAtPath(DatabasePath, typeof(WeaponDatabase));
#endif
                if (_instance == null)
                {
                    _instance = CreateInstance<WeaponDatabase>();
#if UNITY_EDITOR
                    AssetDatabase.CreateAsset(_instance, DatabasePath);
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
    [HideInInspector]
    public List<Weapon> Weapons;

    void OnEnable()
    {
        if(Weapons == null)
            Weapons = new List<Weapon>();
    }
}
