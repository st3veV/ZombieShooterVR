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

    private int selectedWeaponIndex = -2;
    private bool editMode = false;
    private Weapon editedWeapon = null;

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
            foreach (Weapon weapon in Database.Weapons)
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
                {
                    Database.Weapons.Remove(editedWeapon);
                    EditorUtility.SetDirty(Database);
                }
            }
            else if(save)
            {
                editMode = false;
                if (indexOf > -1)
                    Database.Weapons[indexOf] = editedWeapon;
                else
                    Database.Weapons.Add(editedWeapon);
                EditorUtility.SetDirty(Database);
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private Weapon fillEditorWindow(Weapon weapon, out bool delete, out bool save)
    {
        string weaponName = weapon == null ? "New weapon" : weapon.Name;
        float cooldownDelay = weapon == null ? 1000f : weapon.CooldownDelay;
        float damage = weapon == null ? 100f : weapon.Damage;
        int magazineSize = weapon == null ? 10 : weapon.MagazineSize;
        int bulletType = weapon == null ? 0 : weapon.BulletType;
        int initialAmmo = weapon == null ? 10 : weapon.AvailableAmmo;
        GameObject weaponModel = weapon == null ? null : weapon.WeaponModel;
        AudioClip shootSound = weapon == null ? null : weapon.ShootSound;
        AudioClip reloadSound = weapon == null ? null : weapon.ReloadSound;
        AudioClip klickSound = weapon == null ? null : weapon.KlickSound;
        Texture bulletImage = weapon == null ? null : weapon.BulletImage;
        
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
        GUILayout.Label("Initial ammo");
        initialAmmo = EditorGUILayout.IntField(initialAmmo);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        weaponModel = EditorGUILayout.ObjectField("Weapon model", weaponModel, typeof(GameObject), false) as GameObject;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        shootSound = EditorGUILayout.ObjectField("Shoot sound", shootSound, typeof (AudioClip), false) as AudioClip;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        reloadSound = EditorGUILayout.ObjectField("Reload sound", reloadSound, typeof (AudioClip), false) as AudioClip;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        klickSound = EditorGUILayout.ObjectField("Klick sound", klickSound, typeof(AudioClip), false) as AudioClip;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        bulletImage = EditorGUILayout.ObjectField("Bullte image", bulletImage, typeof (Texture), false) as Texture;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        delete = GUILayout.Button("Delete");
        save = GUILayout.Button("Save");
        GUILayout.EndHorizontal();

        if (weapon == null)
        {
            weapon = new Weapon();
        }
        weapon.SetValues(weaponName, damage, cooldownDelay, magazineSize, bulletType, initialAmmo, weaponModel,
            shootSound, reloadSound, klickSound, bulletImage);

        return weapon;
    }
}
