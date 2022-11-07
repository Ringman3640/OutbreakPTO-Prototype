using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    // Abstract Damage method
    // Call for the Damagable object to take a certain amount of damage
    public abstract void Damage(float damage);

    // Damage with reference to collider object
    // If not overridden, will just call Damage(damage);
    public virtual void Damage(float damage, GameObject collider = null)
    {
        Damage(damage);
    }
}
