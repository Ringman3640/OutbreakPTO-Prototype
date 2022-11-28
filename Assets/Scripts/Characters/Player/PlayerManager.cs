using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Assertions;

public class PlayerManager : Damageable
{
    // Component references
    public PlayerInput input;
    public Rigidbody2D rb;
    public SpriteManager sm;
    public SortingGroup sg;
    public MoveStateManager msm;
    public WeaponInventoryManager wim;
    public PlayerInteractManager pim;
    public Hurtbox hb;

    // Speed of character
    public float movementSpeed = 4f;

    // Cooldown of roll action
    public float rollCoolDown = 4f;

    // Angle of aim assist amount
    public float aimAssistAngle = 25f;

    // Indicates if the player is using a gamepad (otherwise keyboard)
    private bool usingGamepad = false;

    // Directional vectors for movement and character looking
    // Position for current mouse position
    private Vector3 moveDirection;
    private Vector3 lookDirection;
    private Vector3 pointPosition;

    // Player speed modifier
    private float speedMultiplier;

    // Indicates the time of the last roll
    private float lastRoll;

    // Indicates if the player is currently in a reloading move state
    private bool reloading;

    // Filter used for obtaining hurtboxes in gamepad aim assist
    private ContactFilter2D aimAssistFilter;

    // Player component properties
    public Rigidbody2D Rigidbody
    {
        get { return rb; } 
        set { rb = value; }
    }
    public SpriteManager Sprite
    {
        get { return sm; }
    }
    public SortingGroup SortGroup
    {
        get { return sg; }
    }
    public MoveStateManager MoveState
    {
        get { return msm; }
    }
    public WeaponInventoryManager WeaponInventory
    {
        get { return wim; }
    }
    public PlayerInteractManager Interactions
    {
        get { return pim; }
    }

    // Directional and positional vector properties
    public Vector3 MoveDirection
    {
        get { return moveDirection; }
    }
    public Vector3 LookDirection
    {
        get { return lookDirection; }
    }
    public Vector3 PointPosition
    {
        get { return pointPosition; }
    }
    public Vector3 HurtboxCenter
    {
        get { return hb.transform.position; }
    }

    // Player info properties
    public ControlRestriction ControlBlockLevel
    {
        get
        {
            return msm.ControlBlockLevel;
        }
    }
    public float SpeedMultiplier
    {
        get { return speedMultiplier; }
        set { speedMultiplier = value; }
    }
    public bool Reloading
    {
        get { return reloading; }
        set { reloading = value; }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(rb);
        Assert.IsNotNull(sm);
        Assert.IsNotNull(sg);
        Assert.IsNotNull(msm);
        Assert.IsNotNull(wim);
        Assert.IsNotNull(hb);

        moveDirection = Vector3.zero;
        lookDirection = Vector3.zero;
        pointPosition = Vector3.zero;

        speedMultiplier = 1f;
        lastRoll = 0f;
        reloading = false;

        aimAssistFilter = new();
        aimAssistFilter.useTriggers = true;
        aimAssistFilter.useLayerMask = true;
        aimAssistFilter.layerMask = LayerMask.GetMask("Hurtbox");

        PlayerSystem.Inst.SetPlayer(gameObject);

        InputUser.onChange += ControlsChanged;
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveDirection();
        GetLookDirection();

        CheckActionInput();
        msm.Execute();

        UpdateWeaponState();
        UpdateVelocity();
        UpdateSprite();
    }

    // Damagable method implementation
    public override void Kill()
    {
        msm.AddMoveState(new CollapseDeathState(gameObject));
    }
    public override void RecieveDamage(HitboxData damageInfo, GameObject collider = null)
    {
        currHealth -= damageInfo.Damage;
        UISystem.Inst.UpdateHealthBar();
        sm.Effects.PlayFlash();

        switch (damageInfo.Response)
        {
            case DamageResponse.Flinch:
                msm.AddMoveState(new FlinchState(gameObject));
                break;

            default:
                // stub, add other responses
                break;
        }

        if (currHealth <= 0)
        {
            Kill();
        }
    }
    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        UISystem.Inst.UpdateHealthBar();
    }
    public override void IncreaseHealth(int increaseAmount)
    {
        base.IncreaseHealth(increaseAmount);
        currHealth = maxHealth;
        UISystem.Inst.UpdateHealthBar();
    }

    // Calculate moveDirection through the user's axis inputs
    private void GetMoveDirection()
    {
        if (ControlBlockLevel.HasFlag(ControlRestriction.Move))
        {
            return;
        }

        moveDirection = input.actions["Move"].ReadValue<Vector2>();
        moveDirection.z = 0f;
        moveDirection.Normalize();
    }

    // Calculate lookDirection through the user's mouse position
    private void GetLookDirection()
    {
        if (ControlBlockLevel.HasFlag(ControlRestriction.Look))
        {
            return;
        }

        if (usingGamepad)
        {
            pointPosition = transform.position;
            Vector2 lookValue = input.actions["Look"].ReadValue<Vector2>();
            if (lookValue == Vector2.zero)
            {
                return;
            }

            lookDirection = lookValue;
            lookDirection.z = 0f;
            lookDirection.Normalize();
            return;
        }

        pointPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = pointPosition - transform.position;
        lookDirection.z = 0f;
        lookDirection.Normalize();
    }

    // Check for User inputs to perform player actions
    private void CheckActionInput()
    {
        if (input.actions["Roll"].triggered)
        {
            if (Time.time - lastRoll >= rollCoolDown)
            {
                lastRoll = Time.time;
                msm.AddMoveState(new RollState(gameObject));
            }
        }

        if (input.actions["Interact"].triggered)
        {
            pim.Interact();
            UISystem.Inst.UpdateAmmoCounter();
        }

        if (input.actions["Next Weapon"].triggered)
        {
            if (reloading)
            {
                msm.FinishCurrentState();
            }
            wim.RotateNextWeapon();
            UISystem.Inst.UpdateAmmoCounter();
        }
        else if (input.actions["Prev Weapon"].triggered)
        {
            if (reloading)
            {
                msm.FinishCurrentState();
            }
            wim.RotatePrevWeapon();
            UISystem.Inst.UpdateAmmoCounter();
        }

        if (input.actions["Reload"].triggered)
        {
            ReloadCurrentWeapon();
        }
    }

    // Update the current weapon state for aiming and firing
    private void UpdateWeaponState()
    {
        // Aim weapon
        if (usingGamepad)
        {
            wim.AimCurrentWeapon(lookDirection);
        }
        else
        {
            wim.AimCurrentWeapon(lookDirection, pointPosition);
        }

        // Fire weapon
        if (input.actions["Fire"].ReadValue<float>() != 0)
        {
            // Aim assist if using gamepad
            if (usingGamepad)
            {
                if (ApplyAimAssist())
                {
                    wim.AimCurrentWeapon(lookDirection, pointPosition);
                }
            }

            // Send fire signal and check for auto reload
            if (input.actions["Fire"].triggered)
            {
                if (WeaponInventory.CurrentWeapon.Ammo <= 0)
                {
                    ReloadCurrentWeapon();
                }
                else
                {
                    wim.FireCurrentWeapon(true);
                }
            }
            else
            {
                wim.FireCurrentWeapon(false);
            }

            UISystem.Inst.UpdateAmmoCounter();
        }
    }

    // Reload the current weapon
    private void ReloadCurrentWeapon()
    {
        WeaponManager wm = WeaponInventory.CurrentWeapon;
        if (wm != null && wm.Ammo < wm.AmmoCapacity)
        {
            switch (wm.weaponName)
            {
                case "weapon_pistol":
                    msm.AddMoveState(new ReloadPistolState(gameObject, wm));
                    break;

                case "weapon_ar":
                    msm.AddMoveState(new ReloadARState(gameObject, wm));
                    break;

                case "weapon_mg":
                    msm.AddMoveState(new ReloadMGState(gameObject, wm));
                    break;
            }
        }
    }

    // Apply look aim assist when using gamepad controls
    // Auto-aims at the closest hurtbox within a certain angle of the look direction
    // Returns true if aim assist is applied
    // Returns false if there was not hurtbox to snap to
    private bool ApplyAimAssist()
    {
        float targetDist = float.MaxValue;
        Vector3 targetPos = transform.position;
        Collider2D[] targets = new Collider2D[20];
        Vector2 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, -Camera.main.transform.position.z));
        Vector2 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, -Camera.main.transform.position.z));

        // Get hurtboxes visible on screen
        int targetCount = Physics2D.OverlapArea(screenBottomLeft, screenTopRight, aimAssistFilter, targets);

        // Target closest detected hurtbox within aim assist range
        for (int i = 0; i < targetCount; ++i)
        {
            // Ignore player's hurtbox
            if ((targets[i].transform.position == hb.transform.position))
            {
                continue;
            }

            Vector2 relativeDirection = targets[i].transform.position - transform.position;
            relativeDirection.Normalize();

            if (Vector3.Angle(lookDirection, relativeDirection) > aimAssistAngle)
            {
                continue;
            }

            float sqrDist = (targets[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDist < targetDist)
            {
                targetPos = targets[i].transform.position;
                targetDist = sqrDist;
            }
        }

        // Apply aim assist
        if (targetPos != transform.position)
        {
            lookDirection = targetPos - transform.position;
            lookDirection.Normalize();
            pointPosition = targetPos;
            return true;
        }

        return false;
    }

    // Update the player's velocity given the value of moveDirection
    private void UpdateVelocity()
    {
        if (ControlBlockLevel.HasFlag(ControlRestriction.Move))
        {
            return;
        }

        if (moveDirection == Vector3.zero)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = moveDirection * (movementSpeed * speedMultiplier);
    }

    // Update the player's sprite animation for basic movement
    private void UpdateSprite()
    {
        if (moveDirection == Vector3.zero)
        {
            sm.Action = AnimAction.Idle;
        }
        else
        {
            sm.Action = AnimAction.Run;
        }

        sm.CalculateDirection(lookDirection);
        sm.BodyPart = AnimBodyPart.TopBottom;
        sm.UpdateState();
    }

    private void ControlsChanged(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change != InputUserChange.ControlSchemeChanged)
        {
            return;
        }

        if (user.controlScheme.Value.name == "Gamepad")
        {
            usingGamepad = true;
        }
        else
        {
            usingGamepad = false;
        }
    }
}
