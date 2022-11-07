using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private Damageable damageTarget;

    private void Awake()
    {
        Assert.IsNotNull(damageTarget);
    }

    // Recieve a Hit with a corresponding damage amount and optional
    // object that caused the hit
    public virtual void Hit(float damage, GameObject collider = null)
    {
        damageTarget.Damage(damage, collider);
    }
}
