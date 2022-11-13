using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Assertions;

public class PlayerManager : Damageable
{
    // Component references
    public PlayerInput input;
    public Rigidbody2D rb;
    public SpriteManager sm;
    public MoveStateManager msm;
    public WeaponInventoryManager wim;
    public PlayerInteractManager pim;

    // Speed of character
    public float movementSpeed = 4f;

    // Indicates if the player is using a gamepad (otherwise keyboard)
    private bool usingGamepad = false;

    // Directional vectors for movement and character looking
    // Position for current mouse position
    private Vector3 moveDirection;
    private Vector3 lookDirection;
    private Vector3 pointPosition;

    // Player component properties
    public Rigidbody2D Rigidbody
    {
        get { return rb; } 
        set { rb = value; }
    }
    public SpriteManager Sprite
    {
        get { return sm; }
        set { sm = value; }
    }
    public MoveStateManager MoveState
    {
        get { return msm; }
        set { msm = value; }
    }
    public WeaponInventoryManager WeaponInventory
    {
        get { return wim; }
        set { wim = value; }
    }
    public PlayerInteractManager Interactions
    {
        get { return pim; }
        set { pim = value; }
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

    // Control and Animation restriction properties
    public ControlRestriction ControlBlockLevel
    {
        get
        {
            return msm.ControlBlockLevel;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Assert.IsNotNull(rb);
        Assert.IsNotNull(sm);
        Assert.IsNotNull(msm);
        Assert.IsNotNull(wim);

        moveDirection = Vector3.zero;
        lookDirection = Vector3.zero;
        pointPosition = Vector3.zero;

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
        // stub, add death animations
        msm.FinishCurrentState();

        Destroy(gameObject);
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
            msm.AddMoveState(new RollState(gameObject));
        }

        if (input.actions["Interact"].triggered)
        {
            pim.Interact();
        }

        if (input.actions["Next Weapon"].triggered)
        {
            wim.RotateNextWeapon();
        }
        else if (input.actions["Prev Weapon"].triggered)
        {
            wim.RotatePrevWeapon();
        }

        // TEMP FOR TESTING ONLY
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // stub
    }

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
            if (input.actions["Fire"].triggered)
            {
                wim.FireCurrentWeapon(true);
            }
            else
            {
                wim.FireCurrentWeapon(false);
            }
        }
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

        rb.velocity = moveDirection * movementSpeed;
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
