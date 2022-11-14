using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;

public class WeaponManager : MonoBehaviour
{
    public string weaponName = null;
    public GameObject projectile = null;
    public GameObject muzzleFlash = null;
    public float inaccuracyAngle = 5f;
    public float projectileSpeed = 75f;
    public float projectileMaxDist = 100f;
    public DamageSource projectileSource = DamageSource.Neutral;
    public int baseDamage = 10;
    public float rateOfFire = 0.2f;
    public bool automaticFire = true;
    public bool projectilePenetration = false;
    public int ammoCapacity = 10;
    public float frontBackRange = 0.4f;

    public SpriteRenderer sr = null;
    public WeaponEquippedController wec = null;
    public GameObject equippedState = null;
    public GameObject unequippedState = null;
    public GameObject worldWeaponStorage = null;
    public GameObject worldProjectileStorage = null;

    private bool equipped;
    private Sprite unequippedSprite;

    // Properties
    public DamageSource ProjectileSource
    {
        get { return projectileSource; }
        set { projectileSource = value; }
    }
    public int Ammo
    {
        get { return wec.Ammo; }
    }
    public bool Equipped
    {
        get { return equipped; }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(weaponName);
        Assert.IsNotNull(projectile);
        Assert.IsNotNull(sr);
        Assert.IsNotNull(wec);
        Assert.IsNotNull(equippedState);
        Assert.IsNotNull(unequippedState);

        wec = transform.Find("Equipped State").GetComponent<WeaponEquippedController>();
        Assert.IsNotNull(wec);

        unequippedSprite = sr.sprite;

        // Check if weapon should start equipped
        if (transform.parent != null && transform.parent.tag == "Weapon Inventory")
        {
            WeaponInventoryManager wim = transform.parent.GetComponent<WeaponInventoryManager>();
            wim.AddWeapon(gameObject);
        }
        else
        {
            equipped = false;
        }
    }

    public void Aim(Vector2 aimDirection)
    {
        if (!equipped)
        {
            return;
        }

        wec.Aim(aimDirection);
    }

    public void Aim(Vector2 aimDirection, Vector2 aimPoint)
    {
        if (!equipped)
        {
            return;
        }

        wec.Aim(aimDirection, aimPoint);
    }

    public bool Fire(bool triggerStarted)
    {
        if (!equipped)
        {
            return false;
        }

        if (!triggerStarted && !automaticFire)
        {
            return false;
        }

        return wec.Fire();
    }

    public bool Equip(GameObject caller)
    {
        if (caller.tag != "Weapon Inventory" || equipped)
        {
            return false;
        }

        WeaponInventoryManager wim = caller.GetComponent<WeaponInventoryManager>();
        transform.parent = wim.transform;
        transform.localPosition = Vector3.zero;

        equippedState.SetActive(true);
        unequippedState.SetActive(false);
        sr.sprite = null;

        wec.Initialize(caller);

        equipped = true;
        return true;
    }

    public void Unequip()
    {
        if (!equipped)
        {
            return;
        }

        if (worldWeaponStorage == null)
        {
            transform.parent = null;
        }
        else
        {
            transform.parent = worldWeaponStorage.transform;
        }

        wec.Remove();
        equippedState.SetActive(false);
        unequippedState.SetActive(true);
        sr.sprite = unequippedSprite;

        equipped = false;
    }

    public void Enable()
    {
        if (!equipped)
        {
            return;
        }

        wec.Enable();
    }

    public void Disable()
    {
        if (!equipped)
        {
            return;
        }

        wec.Disable();
    }
}
