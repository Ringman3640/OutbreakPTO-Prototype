using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Requirements for all derived MoveState classes:
//     - Define ControlBlockLevel, AnimationBlockLevel, and PriorityLevel
//     - Implement Execute() and Finish()
//     - Finish() must revert any state changes in case of a priority overwrite

[System.Flags]
public enum ControlRestriction
{
    None        = 0,
    All         = ~0,
    Move        = (1 << 0),
    Look        = (1 << 1),
    Shoot       = (1 << 2),
    HoldWeapon  = (1 << 3)
}

[System.Flags]
public enum AnimationRestriction
{
    None    = 0,
    All     = ~0,
    Top     = (1 << 0),
    Bottom  = (1 << 1),
}

public abstract class MoveState
{
    // Indicate if the MoveState has completed execution
    public bool Completed
    {
        get; protected set;
    }

    // Player control block level
    public ControlRestriction ControlBlockLevel
    {
        get; protected set;
    }

    // Sprite animation block level
    public AnimationRestriction AnimationBlockLevel
    {
        get; protected set;
    }

    // Priority level for overwritting
    // Higher priority states overwrite lower priority states
    // Must be 0 or greater
    public int PriorityLevel
    {
        get; protected set;
    }

    // Indicate if the state should be overwritten by equal priority states
    public bool EqualOverwritten
    {
        get; protected set;
    }

    // Reference components
    protected PlayerManager player;
    protected SpriteManager sm;

    // Default constructor
    public MoveState()
    {
        player = null;
        sm = null;
        PriorityLevel = -1;
        EqualOverwritten = false;
    }

    // Initialize the MoveState given a reference to the caller Player
    public virtual void Initialize(GameObject caller)
    {
        player = caller.GetComponent<PlayerManager>();
        Assert.IsNotNull(player);

        sm = caller.transform.Find("Move Offsetted/Sprite").GetComponent<SpriteManager>();
        Assert.IsNotNull(sm);

        if (PriorityLevel < 0)
        {
            Debug.LogError("MoveStaet Error: Priority level not set.");
        }
    }

    // Execute the given state on an Update() basis
    // Should be called in Player's Update() function
    public abstract void Execute();

    // Executed when the MoveState is completed by the MoveStateManager
    // Must revert any state changes to cover priority overrides
    public abstract void Finish();

    // Notify the MoveState if some condition is met
    // Use as an animation event to signify a specific frame
    public virtual void Nofity()
    {
        return;
    }
}
