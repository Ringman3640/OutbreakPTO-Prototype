using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Requirements for all derived MoveState classes:
//     - Define ControlBlockLevel and AnimationBlockLevel
//     - Implement Execute()

public abstract class MoveState
{
    // Indicate if the MoveState has completed execution
    public bool completed
    {
        get; protected set;
    }

    // Player control block level
    // "move"
    // "look"
    // "all"
    // "none"
    public string ControlBlockLevel
    {
        get; protected set;
    }

    // Sprite animation block level
    // "top"
    // "bottom"
    // "all"
    // "none"
    public string AnimationBlockLevel
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
    }

    // Initialize the MoveState given a reference to the caller Player
    public virtual void Initialize(GameObject caller)
    {
        player = caller.GetComponent<PlayerManager>();
        Assert.IsNotNull(player);

        sm = caller.transform.Find("Move Offsetted/Sprite").GetComponent<SpriteManager>();
        Assert.IsNotNull(sm);
    }

    // Execute the given state on an Update() basis
    // Should be called in Player's Update() function
    public abstract void Execute();

    // Notify the MoveState if some condition is met
    // Use as an animation event to signify a specific frame
    public virtual void Nofity()
    {
        return;
    }

    // Executed when the MoveState is completed by the MoveStateManager
    public virtual void Completed()
    {
        return;
    }
}
