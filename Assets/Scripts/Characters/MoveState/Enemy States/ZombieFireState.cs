using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFireState : MoveState
{
    private enum FireSubstate
    {
        Aiming,
        AimHolding,
        Shooting,
        Recovering,
        CheckAmmo
    }

    private ZombieEnemyManager zombie;

    private FireSubstate substate = FireSubstate.Aiming;
    private float substateStart;

    private static float aimTime = 0.2f;
    private static float holdPause = 0.1f;
    private static float automaticFireTime = 0.6f;
    private static float recoveryTime = 0.4f;

    private bool automaticWeapon;

    public ZombieFireState(GameObject caller) : base(caller)
    {
        ControlBlockLevel = ControlRestriction.All;
        PriorityLevel = 1;
        EqualOverwritten = false;
    }

    protected override void Initialize(GameObject caller)
    {
        zombie = caller.GetComponent<ZombieEnemyManager>();
        substateStart = Time.time;

        automaticWeapon = zombie.WeaponInventory.CurrentWeapon.automaticFire;
    }

    protected override void Execution()
    {
        if (zombie.DistanceFromPlayer() > zombie.fireRange)
        {
            Completed = true;
            return;
        }

        if (PlayerSystem.Inst.GetPlayer() == null)
        {
            Completed = true;
            return;
        }

        switch(substate)
        {
            case FireSubstate.Aiming:
                substateStart = Time.time;
                zombie.WeaponInventory.AimCurrentWeapon(zombie.DirectionTowardsPlayer());
                zombie.Sprite.CalculateOrientation(zombie.DirectionTowardsPlayer());
                zombie.Sprite.PlayAnimation("idle");
                substate = FireSubstate.AimHolding;
                return;

            case FireSubstate.AimHolding:
                if (Time.time - substateStart >= holdPause)
                {
                    substate = FireSubstate.Shooting;
                    substateStart = Time.time;
                }
                return;

            case FireSubstate.Shooting:
                zombie.WeaponInventory.FireCurrentWeapon(true);
                if (!automaticWeapon | Time.time - substateStart >= automaticFireTime)
                {
                    substate = FireSubstate.Recovering;
                    substateStart = Time.time;
                }
                return;

            case FireSubstate.Recovering:
                if (Time.time - substateStart >= recoveryTime)
                {
                    substate = FireSubstate.CheckAmmo;
                    substateStart = Time.time;
                }
                return;

            case FireSubstate.CheckAmmo:
                if (zombie.WeaponInventory.CurrentWeapon.Ammo <= 0)
                {
                    // stub, drop weapon
                }
                substate = FireSubstate.Aiming;
                substateStart = Time.time;
                return;
        }
    }

    protected override void Restore()
    {
        return;
    }
}
