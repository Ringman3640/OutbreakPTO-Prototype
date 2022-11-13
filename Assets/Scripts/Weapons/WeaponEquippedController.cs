using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum WeaponOrientation
{
    Invalid,
    RightFront,
    RightBack,
    Front,
    Back,
    LeftFront,
    LeftBack
}

public class WeaponEquippedController : MonoBehaviour
{
    public WeaponManager wm;
    public WeaponSpriteManager wsm;
    public Transform normalFirePoint;
    public Transform flippedFirePoint;

    private float lastFireTime;
    private int currAmmo;

    private bool weaponEnabled;
    private WeaponOrientation orientation;

    private GameObject holder;

    public bool WeaponEnabled
    {
        get { return weaponEnabled; }
        set
        {
            if (value)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
    }

    public WeaponOrientation Orientation
    {
        get { return orientation; }
        set
        {
            SetOrientation(value);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        Assert.IsNotNull(wm);
        Assert.IsNotNull(wsm);

        lastFireTime = 0;
        currAmmo = wm.ammoCapacity;

        holder = null;

        Disable();
    }

    public void Initialize(GameObject weaponHolder)
    {
        holder = weaponHolder;
        Enable();
    }

    public void Remove()
    {
        holder = null;
        Disable();
    }

    public void Aim(Vector2 aimDirection)
    {
        if (!weaponEnabled)
        {
            return;
        }

        SetDirection(aimDirection);
    }

    public void Aim(Vector2 aimDirection, Vector2 aimPoint)
    {
        if (!weaponEnabled)
        {
            return;
        }

        Vector2 pointDirection = aimPoint - (Vector2)transform.position;
        pointDirection.Normalize();

        // TODO: Check if pointDirection is too extreme and use aimDirection instead

        SetDirection(pointDirection);
    }

    public bool Fire()
    {
        if (wm.projectile == null)
        {
            Debug.LogError("WeaponController: Projectile not initialized");
            return false;
        }

        if (!weaponEnabled)
        {
            return false;
        }

        // Check rate of fire and ammo
        if (Time.time - lastFireTime < wm.rateOfFire || currAmmo <= 0)
        {
            return false;
        }
        
        // Get target transform of bullet
        Transform targTransform;
        if (wsm.Flipped)
        {
            targTransform = flippedFirePoint;
        }
        else
        {
            targTransform = normalFirePoint;
        }

        // Spawn bullet projectile
        GameObject bullet = Instantiate(wm.projectile);
        bullet.transform.position = targTransform.position;
        if (wm.worldProjectileStorage != null)
        {
            bullet.transform.parent = wm.worldProjectileStorage.transform;
        }

        // Set bullet trajectory with inacurracy
        float inacurracyAngle = Random.Range(-wm.inaccuracyAngle / 2, wm.inaccuracyAngle / 2);
        bullet.transform.right = Quaternion.AngleAxis(inacurracyAngle, Vector3.forward) * transform.right;

        // Override bullset settings with weapon settings
        ProjectileController bulletSettings = bullet.GetComponent<ProjectileController>();
        bulletSettings.speed = wm.projectileSpeed;
        bulletSettings.damage = wm.baseDamage; // todo: add damage multiplier when added
        bulletSettings.maxDistance = wm.projectileMaxDist;
        bulletSettings.penetrateThrough = wm.projectilePenetration;

        // Spawn muzzle flash effect if provided
        if (wm.muzzleFlash != null)
        {
            GameObject flash = Instantiate(wm.muzzleFlash);
            flash.transform.position = targTransform.position;

            // Set far light component of muzzle flash to player ground level
            // (Makes shadows look properly aligned)
            flash.GetComponent<MuzzleFlashController>().farLight.transform.position 
                    = holder.transform.position + transform.right;

            // Set parent if given
            if (wm.worldProjectileStorage != null)
            {
                flash.transform.parent = wm.worldProjectileStorage.transform;
            }
        }

        lastFireTime = Time.time;
        --currAmmo;
        wsm.PlayFireAnim();
        return true;
    }

    private void SetDirection(Vector3 direction)
    {
        transform.right = direction;

        // Update orientation
        if (direction.y > 0)
        {
            if (Mathf.Abs(direction.x) <= wm.frontBackRange)
            {
                SetOrientation(WeaponOrientation.Back);
            }
            else if (direction.x > 0)
            {
                SetOrientation(WeaponOrientation.RightBack);
            }
            else
            {
                SetOrientation(WeaponOrientation.LeftBack);
            }
        }
        else
        {
            if (Mathf.Abs(direction.x) <= wm.frontBackRange)
            {
                SetOrientation(WeaponOrientation.Front);
            }
            else if (direction.x > 0)
            {
                SetOrientation(WeaponOrientation.RightFront);
            }
            else
            {
                SetOrientation(WeaponOrientation.LeftFront);
            }
        }
    }

    private void SetOrientation(WeaponOrientation inOrientation)
    {
        if (inOrientation == orientation)
        {
            return;
        }

        orientation = inOrientation;
        Transform pivot = transform.Find("Pivot Points/" + orientation.ToString());
        PivotPointData pivotData = pivot.GetComponent<PivotPointData>();

        transform.localPosition = pivot.localPosition;
        wsm.Flipped = pivotData.flipWeaponY;
        wsm.SortOrder = pivotData.LayerOrder;
    }

    public void Enable()
    {
        weaponEnabled = true;
        wsm.Visible = true;
    }

    public void Disable()
    {
        weaponEnabled = false;
        orientation = WeaponOrientation.Invalid;
        wsm.Visible = false;
    }
}
