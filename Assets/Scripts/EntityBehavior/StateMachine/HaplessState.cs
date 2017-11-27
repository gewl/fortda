using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaplessState : EntityState {
    public HaplessState(EntityStateMachine machine) 
        : base (machine) { }

    AutonomousMovementComponent movementComponent;

    public override void Enter()
    {
        movementComponent = machine.GetComponent<AutonomousMovementComponent>();

        List<AutonomousMovementComponent.MovementBehaviorTypes> movementBehaviors = new List<AutonomousMovementComponent.MovementBehaviorTypes>();
        movementBehaviors.Add(AutonomousMovementComponent.MovementBehaviorTypes.Wander);

        movementComponent.GenerateActiveMovementBehaviorsFromEnums(movementBehaviors);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {

    }
}
