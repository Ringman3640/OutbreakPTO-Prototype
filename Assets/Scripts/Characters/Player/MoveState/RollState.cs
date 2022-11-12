using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollState : MoveState
{
    private bool started;
    private bool madeInvincible;
    private float speed;
    private const float DEFAULT_SPEED = 6f;
    private Vector3 moveDirection;
    
    public RollState(float rollSpeed = DEFAULT_SPEED) : base()
    {
        ControlBlockLevel = ControlRestriction.All;
        AnimationBlockLevel = AnimationRestriction.All;
        PriorityLevel = 5;

        started = false;
        madeInvincible = false;
        speed = rollSpeed;
    }

    public override void Execute()
    {
        if (player == null)
        {
            Debug.LogError("RollState: Player reference was null");
            Completed = true;
            return;
        }

        if (!started)
        {
            PlayRollAnimation();
            started = true;
        }

        player.Rigidbody.velocity = moveDirection * speed;
    }

    public override void Finish()
    {
        sm.OverrideSequences();
        player.Invincible = false;
    }

    // Notify is used to indicate if the Player should be made invincible or vincible
    public override void Nofity()
    {
        if (!madeInvincible)
        {
            player.Invincible = true;
            madeInvincible = true;
            return;
        }

        player.Invincible = false;
    }

    private void PlayRollAnimation()
    {
        moveDirection = player.MoveDirection;
        if (moveDirection == Vector3.zero)
        {
            moveDirection = player.LookDirection;
        }

        sm.CalculateDirection(moveDirection);
        sm.Action = AnimAction.Roll;
        sm.BodyPart = AnimBodyPart.Full;
        sm.PlaySequenceAnimation();
    }
}
