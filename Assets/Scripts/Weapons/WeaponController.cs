using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO:
//    - ammo
//    - position
//    - 

public enum WeaponOrientation
{
    RightFront,
    RightBack,
    Front,
    Back,
    LeftFront,
    LeftBack
}

public class WeaponController : MonoBehaviour
{
    public float rateOfFire = 0.2f;
    public bool automaticFire = true;
    public int ammoCapacity;

    public string weaponName = null;

    private float lastFire;

    private bool weaponEnabled;
    private WeaponOrientation orientation;
    private Vector3 lastDirection;

    private WeaponSpriteManager wsm;

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
    void Start()
    {
        wsm = transform.Find("Sprite").GetComponent<WeaponSpriteManager>();
        Assert.IsNotNull(wsm);

        Assert.IsNotNull(weaponName);

        weaponEnabled = false;
        orientation = WeaponOrientation.RightFront;
        lastDirection = Vector3.right;
    }

    public bool Fire()
    {
        // stub
        return false;
    }

    public void SetDirection(Vector3 direction)
    {
        lastDirection = direction;
        transform.right = direction;
    }

    public void SetOrientationFromDirection(Vector3 direction)
    {
        // TODO: add frontBackRange variable instead of hard coded

        if (direction.y > 0)
        {
            if (Mathf.Abs(direction.x) <= 0.4)
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
            if (Mathf.Abs(direction.x) <= 0.4)
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

    public void SetOrientation(WeaponOrientation inOrientation)
    {
        if (inOrientation == orientation)
        {
            return;
        }

        orientation = inOrientation;
        Transform pivot = transform.Find("Pivot Points/" + orientation.ToString());
        transform.localPosition = pivot.localPosition;
        wsm.Flipped = (pivot.transform.localScale.y < 0);
        wsm.SortOrder = (int)pivot.transform.localScale.z;
    }

    public void Enable()
    {
        weaponEnabled = true;
        wsm.Visible = true;
    }

    public void Disable()
    {
        weaponEnabled = false;
        wsm.Visible = false;
    }
}
