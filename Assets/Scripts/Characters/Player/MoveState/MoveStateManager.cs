using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MoveStateManager : MonoBehaviour
{
    // Current MoveState being executed
    private MoveState currentState;

    // Block level properties
    public ControlRestriction ControlBlockLevel
    {
        get
        {
            if (currentState == null)
            {
                return ControlRestriction.None;
            }
            return currentState.ControlBlockLevel;
        }
    }
    public AnimationRestriction AnimationBlockLevel
    {
        get
        {
            if (currentState == null)
            {
                return AnimationRestriction.None;
            }
            return currentState.AnimationBlockLevel;
        }
    }

    // Component references
    private PlayerManager player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerManager>();
        Assert.IsNotNull(player);

        currentState = null;
    }

    // Execute the current MoveState
    // Call this method in Update() if there is a MoveState
    public void Execute()
    {
        if (currentState == null)
        {
            return;
        }

        if (currentState.Completed)
        {
            FinishCurrentState();
        }

        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    // Add a MoveState as the current state
    // If there is already a state, override if a higher priority
    public void AddMoveState(MoveState state)
    {
        if (currentState == null)
        {
            currentState = state;
            currentState.Initialize(gameObject);
            return;
        }

        if (state.PriorityLevel > currentState.PriorityLevel)
        {
            currentState.Finish();
            currentState = state;
            currentState.Initialize(gameObject);
            return;
        }

        if (state.PriorityLevel == currentState.PriorityLevel && currentState.EqualOverwritten)
        {
            currentState.Finish();
            currentState = state;
            currentState.Initialize(gameObject);
            return;
        }
    }

    // Get the current MoveState
    public MoveState GetMoveState()
    {
        return currentState;
    }

    // Notify the current state about some event
    public void NofityCurrentState()
    {
        if (currentState == null)
        {
            return;
        }

        currentState.Nofity();
    }

    // Finish the current MoveState and remove it
    // The buffered MoveState will be set to current if present and within the buffer time
    public void FinishCurrentState()
    {
        if (currentState == null)
        {
            return;
        }

        currentState.Finish();
        currentState = null;
    }

    // Check if there is no MoveState currently being executed
    public bool Empty()
    {
        return currentState == null;
    }
}
