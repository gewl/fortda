using System;
using System.Collections;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{
    private Thinker playerThinker;
    public Thinker PlayerThinker { get { return playerThinker; } }
    
    private AutonomousMovementComponent movementComponent;
    public AutonomousMovementComponent MovementComponent { get { return movementComponent; } }

    private EntityState previousState;
    public EntityState PreviousState { get { return previousState; } }

    private EntityState currentState;
    public EntityState CurrentState { get { return currentState; } }

    private EntityState nextState;
    public EntityState NextState { get { return nextState; } }

    public EntityState queuedNextState;

    private bool forced = false;
    public bool StateLocked = false;

    public void Hesitate()
    {
        queuedNextState = new InvestigatingSoundState(this);
        nextState = new PonderingState(this);
    }

    public void StartChildCoroutine(IEnumerator coroutineMethod)
    {
        StartCoroutine(coroutineMethod);
    }

    public void OnEnable()
    {
        nextState = new PonderingState(this);
        playerThinker = GetComponent<Thinker>();
        movementComponent = GetComponent<AutonomousMovementComponent>();
    }

    public void Start() { }

    public void Update()
    {
        if (nextState != null)
        {
            if (currentState != null)
            {
                previousState = currentState;
                previousState.Exit();
            }

            currentState = nextState;
            currentState.Enter();

            nextState = null;

            forced = false;
        }

        currentState.Update();
    }

    public void SwitchState(EntityState nextState)
    {
        if (forced || StateLocked)
        {
            return;
        }

        if (nextState == null || (nextState != null && this.nextState != null && nextState.Priority < this.nextState.Priority))
        {
            return;
        }

        this.nextState = nextState;
    }

    public void ForceSwitchState(EntityState nextState)
    {
        if (forced)
        {
            return;
        }

        queuedNextState = null;
        this.nextState = nextState;
        forced = true;
    }

    public bool IsInState(Type type)
    {
        return currentState.GetType() == type || (nextState != null && nextState.GetType() == type);
    }
}