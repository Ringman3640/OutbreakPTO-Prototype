using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LightsaberEquippedController : WeaponEquippedController
{
    [SerializeField]
    private Hitbox hitbox;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(hitbox);
    }

    // Aim the weapon towards the given point.
    // Lightsaber does not need point aiming, so just use normal aim
    public override void Aim(Vector2 aimDirection, Vector2 aimPoint)
    {
        Aim(aimDirection);
    }

    // Swing lighsaber
    public override bool Fire()
    {
        if (!weaponEnabled)
        {
            return false;
        }

        // Check rate of fire and ammo
        if (Time.time - lastFireTime < wm.rateOfFire)
        {
            return false;
        }

        StartCoroutine(HitboxCoroutine());
        lastFireTime = Time.time;
        wsm.PlayFireAnim();
        return true;
    }

    private IEnumerator HitboxCoroutine()
    {
        hitbox.Enable();

        float startTime = Time.time;
        while (Time.time - startTime < 0.29)
        {
            hitbox.transform.position = transform.position;
            hitbox.transform.right = transform.right;
            yield return null;
        }

        hitbox.Disable();
        yield break;
    }

    private void OnDisable()
    {
        hitbox.Disable();
    }
}
