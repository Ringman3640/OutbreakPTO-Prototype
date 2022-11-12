using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlinchState : MoveState
{
    private bool started;

    public FlinchState() : base()
    {
        ControlBlockLevel = ControlRestriction.None;
        AnimationBlockLevel = AnimationRestriction.Top;
        PriorityLevel = 1;

        started = false;
    }

    public override void Execute()
    {
        if (started)
        {
            return;
        }

        sm.Action = AnimAction.Flinch;
        sm.BodyPart = AnimBodyPart.Top;
        sm.PlaySequenceAnimation();
    }

    public override void Finish()
    {
        sm.OverrideSequences();
    }
}
