using System;
using UnityEditor;
using UnityEngine;

public class WeaponManagerWindow : EditorWindow {

    public WeaponDatabase Database { get; set; }

    [MenuItem("ZombieShooter/Weapons")]
    public static void Init()
    {
        WeaponManagerWindow window = GetWindow<WeaponManagerWindow>();
        window.Database = WeaponDatabase.Instance;
        window.Show();
    }

    private int _selectedWeaponIndex = -1;
    private Weapon _editedWeapon = null;

    private State _state;

    private enum State
    {
        Blank,
        Edit,
        Add
    }

    void OnEnable()
    {
        _state = State.Blank;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DisplayList();
        DisplayMain();
        EditorGUILayout.EndHorizontal();
    }

    private void DisplayList()
    {
        GUILayout.BeginVertical(GUILayout.Width(250));
        
        for (var i = 0; i < Database.Weapons.Count; i++)
        {
            var weapon = Database.Weapons[i];
            var selected = GUILayout.Button(weapon.Name, GUILayout.ExpandWidth(true));
            if (selected)
            {
                _selectedWeaponIndex = i;
                _editedWeapon = weapon;
                _state = State.Edit;
            }
        }
        EditorGUILayout.Separator();
        var newWeapon = GUILayout.Button("New weapon");
        if (newWeapon)
        {
            _selectedWeaponIndex = -1;
            _state = State.Add;
        }
        
        GUILayout.EndVertical();
    }

    private void DisplayMain()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        switch (_state)
        {
            case State.Edit:
                DisplayEdit();
                break;
            case State.Add:
                DisplayAdd();
                break;
            case State.Blank:
            default:
                break;
        }
        EditorGUILayout.EndVertical();
    }

    private void DisplayEdit()
    {
        _editedWeapon = Database.Weapons[_selectedWeaponIndex];
        bool delete;
        bool save;
        Database.Weapons[_selectedWeaponIndex] = FillEditorWindow(_editedWeapon, out delete, out save);
        if (delete)
        {
            Database.Weapons.RemoveAt(_selectedWeaponIndex);
            EditorUtility.SetDirty(Database);
            _state = State.Blank;
            _editedWeapon = null;
        }
        if (save)
        {
            EditorUtility.SetDirty(Database);
            _state = State.Blank;
            _editedWeapon = null;
        }
    }

    private void DisplayAdd()
    {
        if (_editedWeapon == null)
        {
            _editedWeapon = new Weapon
            {
                Name = "New weapon",
                CooldownDelay = 1000f,
                Damage = 100f,
                MagazineSize = 10,
                BulletType = 0,
                AvailableAmmo = 10,

                BulletSpreadAngle = 0,
                NumBulletsPerShot = 1
            };
        }
        bool delete;
        bool save;
        _editedWeapon = FillEditorWindow(_editedWeapon, out delete, out save);
        if (delete)
        {
            _editedWeapon = null;
            _state = State.Blank;
        }
        if (save)
        {
            Database.Weapons.Add(_editedWeapon);
            EditorUtility.SetDirty(Database);
            _state = State.Blank;
            _editedWeapon = null;
        }
    }
    
    private Weapon FillEditorWindow(Weapon weapon, out bool delete, out bool save)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        weapon.Name = GUILayout.TextField(weapon.Name);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        weapon.Damage = EditorGUILayout.FloatField(weapon.Damage);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cooldown delay");
        weapon.CooldownDelay = EditorGUILayout.FloatField(weapon.CooldownDelay);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Magazine size");
        weapon.MagazineSize = EditorGUILayout.IntField(weapon.MagazineSize);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Bullet type");
        weapon.BulletType = EditorGUILayout.IntField(weapon.BulletType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Initial ammo");
        weapon.AvailableAmmo = EditorGUILayout.IntField(weapon.AvailableAmmo);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        weapon.WeaponModel = EditorGUILayout.ObjectField("Weapon model", weapon.WeaponModel, typeof(GameObject), false) as GameObject;
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sounds");

        GUILayout.BeginHorizontal();
        weapon.ShootSound = EditorGUILayout.ObjectField("Shoot sound", weapon.ShootSound, typeof (AudioClip), false) as AudioClip;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        weapon.ReloadSound = EditorGUILayout.ObjectField("Reload sound", weapon.ReloadSound, typeof (AudioClip), false) as AudioClip;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        weapon.KlickSound = EditorGUILayout.ObjectField("Klick sound", weapon.KlickSound, typeof(AudioClip), false) as AudioClip;
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Icons");

        GUILayout.BeginHorizontal();
        weapon.BulletImage = EditorGUILayout.ObjectField("Bullet image", weapon.BulletImage, typeof (Texture), false) as Texture;
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        weapon.WeaponImage = EditorGUILayout.ObjectField("Weapon image", weapon.WeaponImage, typeof (Texture), false) as Texture;
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shooting settings");

        GUILayout.BeginHorizontal();
        weapon.BulletSpreadAngle = EditorGUILayout.FloatField("Spread angle", weapon.BulletSpreadAngle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        weapon.NumBulletsPerShot = EditorGUILayout.IntField("Bullets per shot", weapon.NumBulletsPerShot);
        GUILayout.EndHorizontal();
        
        
        GUILayout.BeginHorizontal();
        delete = GUILayout.Button("Delete");
        save = GUILayout.Button("Save");
        GUILayout.EndHorizontal();

        return weapon;
    }
}
