using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerManager : MonoBehaviour
{
    // Speed of character
    public float movementSpeed = 4f;

    // Directional vectors for movement and character looking
    // Position for current mouse position
    private Vector3 moveDirection;
    private Vector3 lookDirection;
    private Vector3 pointPosition;

    // Component references
    public Rigidbody2D rb
    {
        get; set;
    }
    public SpriteManager sm
    {
        get; set;
    }
    private MoveStateManager msm;
    private WeaponController wc;

    // Directional and positional vector properties
    public Vector3 MoveDirection
    {
        get
        {
            return moveDirection;
        }
    }
    public Vector3 LookDirection
    {
        get
        {
            return lookDirection;
        }
    }
    public Vector3 PointPosition
    {
        get
        {
            return pointPosition;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);

        sm = transform.Find("Move Offsetted/Sprite").GetComponent<SpriteManager>();
        Assert.IsNotNull(sm);

        msm = GetComponent<MoveStateManager>();
        Assert.IsNotNull(msm);

        wc = transform.Find("Move Offsetted/Weapon").GetComponent<WeaponController>();
        Assert.IsNotNull(wc);

        moveDirection = Vector3.zero;
        lookDirection = Vector3.zero;
        pointPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveDirection();
        GetLookDirection();

        CheckActionInput();
        if (!msm.Empty())
        {
            // TODO: check block levels
            msm.Execute();
            return;
        }

        UpdateVelocity();
        UpdateSprite();
    }

    // Calculate moveDirection through the user's axis inputs
    private void GetMoveDirection()
    {
        if (msm.ControlBlockLevel == "move" ||msm.ControlBlockLevel == "all")
        {
            return;
        }

        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.y = Input.GetAxisRaw("Vertical");
        moveDirection.z = 0f;
        moveDirection.Normalize();
    }

    // Calculate lookDirection through the user's mouse position
    private void GetLookDirection()
    {
        if (msm.ControlBlockLevel == "look" || msm.ControlBlockLevel == "all")
        {
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            msm.AddMoveState(new RollState());
        }

        // stub
    }

    // Update the player's velocity given the value of moveDirection
    private void UpdateVelocity()
    {
        if (moveDirection == Vector3.zero)
        {
            // play idle animation
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = moveDirection * movementSpeed;
    }

    // Update the player's sprite animation for basic movement
    private void UpdateSprite()
    {
        if (msm.AnimationBlockLevel == "all")
        {
            return;
        }

        if (moveDirection == Vector3.zero)
        {
            sm.Action = "idle";
        }
        else
        {
            sm.Action = "run";
        }

        sm.CalculateDirection(lookDirection);

        if (msm.AnimationBlockLevel == "none")
        {
            sm.BodyPart = "topbottom";
        }
        else if (msm.AnimationBlockLevel == "top")
        {
            sm.BodyPart = "bottom";
        }
        else
        {
            sm.BodyPart = "top";
        }

        sm.UpdateSprite();

        // TODO: Temp solution to weapon update, fix later
        wc.SetDirection(lookDirection);
        wc.SetOrientationFromDirection(lookDirection);
    }
}
