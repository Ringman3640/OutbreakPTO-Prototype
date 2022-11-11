using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth = 100;

    protected int currHealth;

    public bool Invincible
    {
        get; set;
    }

    protected virtual void Start()
    {
        currHealth = maxHealth;
        Invincible = false;
    }

    // Abstract Kill method
    // Request for the Damageable object to the killed/destroyed
    // Should be called from Damage() when currHealth <= 0
    public abstract void Kill();

    // Abstract RecieveDamage method
    // Call for the Damagable object to take a certain amount of damage
    public abstract void RecieveDamage(HitboxData damageInfo, GameObject collider = null);

    // Damage interface method
    // Used by outside objects to signal for the Damageable class to be damaged
    public virtual void Damage(HitboxData damageInfo, GameObject collider = null)
    {
        if (Invincible)
        {
            return;
        }

        RecieveDamage(damageInfo, collider);
    }
}
