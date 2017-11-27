using System;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{
    private GameObject player;
    public GameObject Player { get { return player; } }

    private EntityState previousState;
    public EntityState PreviousState { get { return previousState; } }

    private EntityState currentState;
    public EntityState CurrentState { get { return currentState; } }

    private EntityState nextState;
    public EntityState NextState { get { return nextState; } }

    private bool forced = false;

    public void OnEnable()
    {
        nextState = new PonderingState(this);
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
        if (forced)
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

        this.nextState = nextState;
        forced = true;
    }

    public bool IsInState(Type type)
    {
        return currentState.GetType() == type || (nextState != null && nextState.GetType() == type);
    }
}