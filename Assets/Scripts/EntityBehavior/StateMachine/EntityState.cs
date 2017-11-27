using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityState 
{
    protected EntityStateMachine machine;

    public EntityState(EntityStateMachine machine)
    {
        this.machine = machine;
    }

    protected int priority = 1;
    public int Priority { get { return priority; } }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}