using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Hurtbox : MonoBehaviour
{
    [SerializeField]
    private Damageable damageTarget;

    private void Awake()
    {
        Assert.IsNotNull(damageTarget);
    }

    // Trigger enter function for Hitbox->Hurtbox collisions
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Hitbox hitbox = collision.GetComponent<Hitbox>();
        if (hitbox == null)
        {
            return;
        }

        damageTarget.Damage(hitbox.Data, collision.gameObject);
    }

    // Recieve a Hit with a corresponding damage amount and optional
    // object that caused the hit
    public virtual void Hit(HitboxData damageInfo, GameObject collider = null)
    {
        damageTarget.Damage(damageInfo, collider);
    }
}
