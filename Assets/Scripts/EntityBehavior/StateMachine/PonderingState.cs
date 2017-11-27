using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonderingState : EntityState {
    public PonderingState(EntityStateMachine machine) 
        : base (machine) { }

    float timer;
    Thinker thinker;

    public override void Enter()
    {
        timer = 2f;
        thinker = machine.transform.GetComponent<Thinker>();
        thinker.Think();
    }

    public override void Update()
    {
        if (timer <= 0)
        {
            machine.SwitchState(new HaplessState(machine));
        }

        timer -= Time.deltaTime;
    }

    public override void Exit()
    {
        Debug.Log("exiting pondering");
    }
}
