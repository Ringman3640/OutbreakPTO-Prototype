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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb);
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
}
