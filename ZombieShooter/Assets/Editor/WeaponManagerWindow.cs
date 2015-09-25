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

    private int selectedWeaponIndex = -2;
    private bool editMode = false;
    private IWeapon editedWeapon = null;

    void OnGUI()
    {
        Rect weaponSelectionAreaSize = new Rect(0, 0, position.width*.3f, position.height - 100f);
        Rect weaponDetailsAreaSize = new Rect(weaponSelectionAreaSize.x + weaponSelectionAreaSize.width, weaponSelectionAreaSize.y, position.width * .7f, position.height - 100f);

        GUILayout.BeginArea(weaponSelectionAreaSize);
        GUILayout.BeginVertical();

        bool newWeapon = false;
        if (!editMode)
        {
            editedWeapon = null;
            selectedWeaponIndex = -2;
            int i = 0;
            foreach (IWeapon weapon in Database.Weapons)
            {
                bool selected = GUILayout.Button(weapon.Name);
                if (selected)
                {
                    selectedWeaponIndex = i;
                }
                i++;
            }
            if (selectedWeaponIndex > -1)
            {
                editedWeapon = Database.Weapons[selectedWeaponIndex];
            }
            EditorGUILayout.Separator();
            newWeapon = GUILayout.Button("New weapon");
            if (newWeapon)
            {
                selectedWeaponIndex = -1;
            }

            if (selectedWeaponIndex > -2)
                editMode = true;

        }
        GUILayout.EndVertical();
        GUILayout.EndArea();


        GUILayout.BeginArea(weaponDetailsAreaSize);
        GUILayout.BeginVertical();
        if (editMode)
        {
            bool deleteWeapon = false;
            bool save = false;
            if (selectedWeaponIndex == -1)
            {
                editedWeapon = fillEditorWindow(editedWeapon, out deleteWeapon,out save);
            }
            else if (selectedWeaponIndex > -1)
            {
                editedWeapon = fillEditorWindow(editedWeapon, out deleteWeapon, out save);
            }

            int indexOf = Database.Weapons.IndexOf(editedWeapon);
            if (deleteWeapon)
            {
                editMode = false;
                if (indexOf > -1)
                    Database.Weapons.Remove(editedWeapon);
            }
            else if(save)
            {
                editMode = false;
                if (indexOf > -1)
                    Database.Weapons[indexOf] = editedWeapon;
                else
                    Database.Weapons.Add(editedWeapon);
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private IWeapon fillEditorWindow(IWeapon weapon, out bool delete, out bool save)
    {

        string weaponName = weapon == null ? "New weapon" : weapon.Name;
        float cooldownDelay = weapon == null ? 1.0f : weapon.CooldownDelay;
        float damage = weapon == null ? 1000f : weapon.Damage;
        int magazineSize = weapon == null ? 10 : weapon.MagazineSize;
        int bulletType = weapon == null ? 0 : weapon.BulletType;
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        weaponName = GUILayout.TextField(weaponName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        damage = EditorGUILayout.FloatField(damage);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cooldown delay");
        cooldownDelay = EditorGUILayout.FloatField(cooldownDelay);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Magazine size");
        magazineSize = EditorGUILayout.IntField(magazineSize);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Bullet type");
        bulletType = EditorGUILayout.IntField(bulletType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        delete = GUILayout.Button("Delete");
        save = GUILayout.Button("Save");
        GUILayout.EndHorizontal();

        if (weapon == null)
        {
            weapon = new ModularWeapon();
        }
        weapon.SetValues(weaponName, damage, cooldownDelay, magazineSize, bulletType);

        return weapon;
    }
}

class ModularWeapon : IWeapon
{
    private int _bulletType;
    private int _magazineSize;
    private float _cooldownDelay;
    private float _damage;
    private string _name;

    public void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType)
    {
        _name = name;
        _damage = damage;
        _cooldownDelay = cooldownDelay;
        _magazineSize = magazineSize;
        _bulletType = bulletType;
    }

    public int BulletType
    {
        get { return _bulletType; }
    }

    public int MagazineSize
    {
        get { return _magazineSize; }
    }

    public float CooldownDelay
    {
        get { return _cooldownDelay; }
    }

    public float Damage
    {
        get { return _damage; }
    }

    public string Name
    {
        get { return _name; }
    }

    public int AvailableAmmo { get; set; }
}

