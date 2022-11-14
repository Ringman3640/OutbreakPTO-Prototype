using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ZombieEnemyManager : Enemy
{
    [SerializeField]
    private EnemySpriteManager esm;
    [SerializeField]
    private MoveStateManager msm;
    [SerializeField]
    private WeaponInventoryManager wim;
    [SerializeField]
    private List<GameObject> weaponList;

    public float fireRange = 8f;

    private PlayerManager player;

    // Properties
    public WeaponInventoryManager WeaponInventory
    {
        get { return wim; }
    }
    public EnemySpriteManager Sprite
    {
        get { return esm; }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(esm);
        Assert.IsNotNull(msm);
        Assert.IsNotNull(wim);
        Assert.IsNotNull(weaponList);

        GetStartingWeapon();

        player = null;
    }

    // Update is called once per frame
    void Update()
    {
        msm.Execute();

        player = PlayerSystem.Inst.GetPlayerManager();
        if (player == null || !alerted)
        {
            Idle();
            return;
        }

        float distance = DistanceFromPlayer();
        Vector2 direction = DirectionTowardsPlayer();
        esm.CalculateOrientation(direction);

        if (distance > fireRange)
        {
            ApproachPlayer();
        }
        else
        {
            StopMovement();
            msm.AddMoveState(new ZombieFireState(gameObject));
        }
    }

    // Damageable method implementations
    public override void Kill()
    {
        // stub, add death animation
        Destroy(gameObject);
    }
    public override void RecieveDamage(HitboxData damageInfo, GameObject collider = null)
    {
        base.RecieveDamage(damageInfo, collider);

        if (damageInfo.Source == DamageSource.Friendly)
        {
            alerted = true;
        }
    }

    private void GetStartingWeapon()
    {
        GameObject weapon = weaponList[Random.Range(0, weaponList.Count)];
        weapon = Instantiate(weapon);
        wim.AddWeapon(weapon);
    }

    public void Idle()
    {
        if (msm.ControlBlockLevel.HasFlag(ControlRestriction.Move))
        {
            return;
        }

        esm.PlayAnimation("idle");
    }

    public void ApproachPlayer()
    {
        if (msm.ControlBlockLevel.HasFlag(ControlRestriction.Move))
        {
            return;
        }

        MoveTowardsPlayer();
        esm.PlayAnimation("walk");
    }
}
