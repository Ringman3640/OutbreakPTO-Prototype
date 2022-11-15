using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemBoxManager : Damageable
{
    [SerializeField]
    private SpriteEffects sEffects;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(sEffects);
    }

    // Damageable method implementations
    public override void Kill()
    {
        // stub, spawn items
        Destroy(gameObject);
    }
    public override void RecieveDamage(HitboxData damageInfo, GameObject collider = null)
    {
        currHealth -= damageInfo.Damage;
        sEffects.PlayFlash();

        if (currHealth <= 0)
        {
            Kill();
        }
    }
}
