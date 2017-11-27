using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonderingState : EntityState {
    public PonderingState(EntityStateMachine machine) 
        : base (machine) { }

    float timer;

    public override void Enter()
    {
        timer = 2f;
        machine.PlayerThinker.Mull();
    }

    public override void Update()
    {
        if (timer <= 0)
        {
            if (machine.queuedNextState != null)
            {
                EntityState nextState = machine.queuedNextState;
                machine.queuedNextState = null;
                machine.SwitchState(nextState);
            }
            else
            {
                machine.SwitchState(new HaplessState(machine));
            }
        }

        timer -= Time.deltaTime;
    }

    public override void Exit()
    {

    }
}
