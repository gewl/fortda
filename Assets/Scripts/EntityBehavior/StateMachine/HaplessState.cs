using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaplessState : EntityState {
    public HaplessState(EntityStateMachine machine) 
        : base (machine) { }


    public override void Enter()
    {
        List<AutonomousMovementComponent.MovementBehaviorTypes> movementBehaviors = new List<AutonomousMovementComponent.MovementBehaviorTypes>
        {
            AutonomousMovementComponent.MovementBehaviorTypes.Wander
        };

        machine.MovementComponent.GenerateActiveMovementBehaviorsFromEnums(movementBehaviors);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {

    }
}
