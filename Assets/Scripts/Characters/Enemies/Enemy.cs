using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Enemy : Damageable
{
    [SerializeField]
    protected Rigidbody2D rb;

    [SerializeField]
    protected float speed = 3f;

    protected bool alerted;

    private int raycastMask;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);

        alerted = false;
        raycastMask = LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Obstacle");
    }

    public override void RecieveDamage(HitboxData damageInfo, GameObject collider = null)
    {
        currHealth -= damageInfo.Damage;

        // TODO: add damage response effects
        switch (damageInfo.Type)
        {
            case DamageType.None:

                break;

            case DamageType.Poke:

                break;

            case DamageType.Pierce:

                break;

            case DamageType.Slash:

                break;

            case DamageType.Impact:

                break;
        }

        if (currHealth <= 0)
        {
            Kill();
        }
    }

    // Alert the enemy if the player is nearby
    public virtual void Alert()
    {
        alerted = true;
    }

    // Move the enemy towards the player 
    public void MoveTowardsPlayer()
    {
        GameObject player = PlayerSystem.Inst.GetPlayer();
        if (player == null)
        {
            return;
        }

        MoveTowardsPoint(player.transform.position);
    }

    // Move the enemy towards a coordinate point
    public void MoveTowardsPoint(Vector2 point)
    {
        Vector2 moveDirection = point - (Vector2)transform.position;
        moveDirection.Normalize();

        rb.velocity = moveDirection * speed;
    }

    // Stop the movement of the enemy (set velocity to zero)
    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    // Get a direction vector pointing towards the player
    public Vector2 DirectionTowardsPlayer()
    {
        GameObject player = PlayerSystem.Inst.GetPlayer();
        if (player == null)
        {
            return Vector2.zero;
        }

        return DirectionTowardsPoint(player.transform.position);
    }

    // Get a direction vector pointing towards a coordinate point
    public Vector2 DirectionTowardsPoint(Vector2 point)
    {
        return point - (Vector2)transform.position;
    }

    // Get the distance from the player in units
    public float DistanceFromPlayer()
    {
        GameObject player = PlayerSystem.Inst.GetPlayer();
        if (player == null)
        {
            return -1f;
        }

        return DistanceFromPoint(player.transform.position);
    }

    // Get the distance from a coordinate point
    public float DistanceFromPoint(Vector2 point)
    {
        return Vector2.Distance((Vector2)transform.position, point);
    }

    // Check if there is a sightline to the player
    // distance is the distance from 
    public bool SightlineToPlayer()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, 
                DirectionTowardsPlayer(), Mathf.Infinity, raycastMask);

        if (hitInfo.collider == null || hitInfo.collider.tag != "Player")
        {
            return false;
        }

        return true;
    }
}
