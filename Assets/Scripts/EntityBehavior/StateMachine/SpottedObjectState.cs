using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpottedObjectState : EntityState {
    public SpottedObjectState(EntityStateMachine machine) 
        : base (machine) { }

    Transform spottedObject;
    float timeToPickUp = 1f;
    float timeToAbsorb = 2f;
    bool begunToInspect = false;

    public override void Enter()
    {
        machine.StateLocked = true;

        spottedObject = machine.PlayerThinker.objectsInvestigating.Pop();
        List<AutonomousMovementComponent.MovementBehaviorTypes> movementBehaviors = new List<AutonomousMovementComponent.MovementBehaviorTypes>
        {
            AutonomousMovementComponent.MovementBehaviorTypes.Arrive
        };

        machine.MovementComponent.ArriveTarget = spottedObject;
        machine.MovementComponent.GenerateActiveMovementBehaviorsFromEnums(movementBehaviors);
    }

    public override void Update()
    {
        if (SqrDistanceToSpottedObject() <= 0.1f)
        {
            machine.MovementComponent.ClearMovementBehaviors();
        }
        else if (!begunToInspect)
        {
            begunToInspect = true;
            machine.StartChildCoroutine(ProcessObject());
        }
    }

    public override void Exit()
    {
    }

    float SqrDistanceToSpottedObject()
    {
        Vector3 spottedObjectPosition = spottedObject.position;
        Vector3 playerPosition = machine.transform.position;

        return (spottedObjectPosition - playerPosition).sqrMagnitude;
    }

    IEnumerator ProcessObject()
    {
        spottedObject.parent = machine.transform;
        spottedObject.GetComponent<Rigidbody>().useGravity = false;
        machine.MovementComponent.ClearMovementBehaviors();
        Vector3 initialLocalPosition = spottedObject.localPosition;
        Vector3 destinationPosition = Vector3.forward;

        float timeElapsed = 0.0f;

        while (timeElapsed < timeToPickUp)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToPickUp;

            spottedObject.localPosition = Vector3.Lerp(initialLocalPosition, destinationPosition, percentageComplete);
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        timeElapsed = 0.0f;

        initialLocalPosition = spottedObject.localPosition;
        destinationPosition = Vector3.zero;

        Vector3 initialSize = spottedObject.localScale;
        Vector3 destinationSize = Vector3.zero;

        while (timeElapsed < timeToAbsorb)
        {
            timeElapsed += Time.deltaTime;

            float percentageComplete = timeElapsed / timeToPickUp;

            spottedObject.localPosition = Vector3.Lerp(initialLocalPosition, destinationPosition, percentageComplete);
            spottedObject.localScale = Vector3.Lerp(initialSize, destinationSize, percentageComplete);
            yield return null;
        }

        ObjectBehavior objectBehavior = spottedObject.GetComponent<ObjectBehavior>();
        GameObject shape = objectBehavior.Shape;
        Material color = objectBehavior.Color;
        Vector3 size = objectBehavior.Size;
    }
}
