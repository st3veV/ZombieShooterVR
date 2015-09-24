using UnityEngine;
using System.Collections;
using UnityEditor;

public class WeaponManagerWindow : EditorWindow {

    public WeaponDatabase Database { get; set; }

    [MenuItem("ZombieShooter/Weapons")]
    public static void Init()
    {
        WeaponManagerWindow window = EditorWindow.GetWindow<WeaponManagerWindow>();
        window.Database = WeaponDatabase.Instance;
        window.Show();
    }

    private int selectedWeaponIndex = -1;

    void OnGUI()
    {
        Rect weaponSelectionAreaSize = new Rect(0, 0, position.width*.3f, position.height - 100f);
        Rect weaponDetailsAreaSize = new Rect(0, 0, position.width*.7f, position.height - 100f);

        GUILayout.Label("Weapon manager",EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        //list
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        bool addWeaponPressed = GUILayout.Button("Add");
        if(selectedWeaponIndex > -1)
        {
            bool removeWeaponPressed = GUILayout.Button("Remove", GUIStyle.none);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginArea(weaponSelectionAreaSize);
        GUILayout.BeginVertical();
        selectedWeaponIndex = -1;
        int i = 0;
        /*
        foreach (IWeapon weapon in Database.weapons)
        {
            bool selected = EditorGUILayout.Foldout(false, weapon.Name);
            if(selected)
            {
                selectedWeaponIndex = i;
            }
            i++;
        }
        */
        GUILayout.EndVertical();
        GUILayout.EndArea();
        GUILayout.EndVertical();

        GUILayout.BeginArea(weaponDetailsAreaSize);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        string weaponName = GUILayout.TextField("New weapon");
        GUILayout.EndHorizontal();

        GUILayout.Button("Save");
        GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.EndHorizontal();
    }

}
