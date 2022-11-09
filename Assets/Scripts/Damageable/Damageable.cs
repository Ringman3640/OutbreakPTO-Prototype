using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth = 100;

    protected int currHealth;

    protected virtual void Start()
    {
        currHealth = maxHealth;
    }

    // Abstract Kill method
    // Request for the Damageable object to the killed/destroyed
    // Should be called from Damage() when currHealth <= 0
    public abstract void Kill();

    // Abstract Damage method
    // Call for the Damagable object to take a certain amount of damage
    public abstract void Damage(HitboxData damageInfo);

    // Damage with reference to collider object
    // If not overridden, will just call Damage(damage);
    public virtual void Damage(HitboxData damageInfo, GameObject collider)
    {
        Damage(damageInfo);
    }
}
