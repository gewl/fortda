using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatingSoundState : EntityState {
    public InvestigatingSoundState(EntityStateMachine machine) 
        : base (machine) { }

    Transform objectCurrentlyInvestigating;

    public override void Enter()
    {
        objectCurrentlyInvestigating = machine.PlayerThinker.objectsInvestigating.Peek();

        List<AutonomousMovementComponent.MovementBehaviorTypes> movementBehaviors = new List<AutonomousMovementComponent.MovementBehaviorTypes>
        {
            AutonomousMovementComponent.MovementBehaviorTypes.Arrive
        };

        machine.MovementComponent.ArriveTarget = objectCurrentlyInvestigating;
        machine.MovementComponent.GenerateActiveMovementBehaviorsFromEnums(movementBehaviors);
    }

    public override void Update()
    {
        Vector3 toTarget = objectCurrentlyInvestigating.position - machine.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(machine.transform.position, toTarget, out hit))
        {
            if (hit.transform == objectCurrentlyInvestigating)
            {
                machine.SwitchState(new SpottedObjectState(machine));
            }
        }
    }

    public override void Exit()
    {
    }

    float SquareDistanceToInvestigatingPosition()
    {
        return (objectCurrentlyInvestigating.position - machine.transform.position).sqrMagnitude;
    }

}
