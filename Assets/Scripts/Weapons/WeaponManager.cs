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
    public float rateOfFire = 0.2f;
    public bool automaticFire = true;
    public int ammoCapacity = 10;
    public float frontBackRange = 0.4f;

    public GameObject equippedState = null;
    public GameObject unequippedState = null;
    public GameObject worldWeaponStorage = null;

    private bool equipped;

    public bool Equipped
    {
        get { return equipped; }
    }

    private PlayerManager player;
    private WeaponEquippedController wec;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(weaponName);
        Assert.IsNotNull(projectile);
        Assert.IsNotNull(equippedState);
        Assert.IsNotNull(unequippedState);

        player = null;

        wec = transform.Find("Equipped State").GetComponent<WeaponEquippedController>();
        Assert.IsNotNull(wec);

        equipped = false;
    }

    public void UpdateWeaponState(Vector3 pointDirection)
    {
        if (!equipped)
        {
            return;
        }

        wec.UpdateWeaponState(pointDirection);
    }

    public bool Equip(GameObject caller)
    {
        if (caller.tag != "Weapon Inventory" || equipped)
        {
            return false;
        }

        WeaponInventoryManager wim = caller.GetComponent<WeaponInventoryManager>();
        player = wim.player;
        transform.parent = wim.transform;
        transform.localPosition = Vector3.zero;

        equippedState.SetActive(true);
        unequippedState.SetActive(false);

        wec.Initialize(player);

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

        equipped = false;
    }
}
