﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class AutonomousMovementComponent : EntityComponent {

    public enum MovementBehaviorTypes
    {
        Seek,
        Flee,
        Arrive,
        Pursuit,
        Evade,
        Wander,
        ObstacleAvoidance,
        WallAvoidance,
        Interpose,
        Hide,
        PathFollowing,
        OffsetPursuit,
        Separation,
        Alignment,
        Flocking, 
        Cohesion
    }

    Vector3 currentVelocity;
    public Vector3 CurrentVelocity { get { return currentVelocity; } }

    [Title("Behaviors dictating entity movement", "Sorted in decreasing order of priority")]
    [SerializeField]
    private List<MovementBehaviorTypes> initialMovementBehaviors;
    private List<AutonomousMovementBehavior> activeMovementBehaviors;

    [SerializeField]
    float maximumSteeringForce = 50f;
    public float maxSpeed = 50f;

    // A number of the implementations of behaviors from the Buckland book involve some eyeball-y values for
    // individual features—things like deceleration in Arrive, distance to hide behind things in Hide, etc.
    // Rather than set up a bunch of situational cases to allow for this component tweaking individual behaviors at run-time,
    // this is an attempt to establish a single (manipulable) value to account for all these specific traits, on the assumption that
    // differing values correspond roughly to responsive/dextrous/graceful/precise vs. clumsy/sloppy/error-prone.
    [Range(0, 5)]
    public int Clumsiness = 3;

    [Header("Wander Configuration")]
    [SerializeField, HideInInspector]
    float wanderDistance = 2f;
    public float WanderDistance { get { return wanderDistance; } }
    [SerializeField, HideInInspector]
    float wanderRadius = 1f;
    public float WanderRadius { get { return wanderRadius; } }
    [SerializeField, HideInInspector]
    float wanderJitter = 0.5f;
    public float WanderJitter { get { return wanderJitter; } }

    [Header("Path Following Configuration")]
    [OdinSerialize, HideInInspector]
    public List<Transform> PathNodes;

    [Header("Interpose Configuration")]
    [HideInInspector]
    public Transform PrimaryInterposeTarget;
    [HideInInspector]
    public Transform SecondaryInterposeTarget;

    [Header("Seek Configuration")]
    [HideInInspector]
    public Transform SeekTarget;

    [Header("Pursuit Configuration")]
    [HideInInspector]
    public Transform PursuitTarget;

    [Header("Offset Pursuit Configuration")]
    [HideInInspector]
    public Transform OffsetPursuitTarget;
    [OdinSerialize, HideInInspector]
    public Vector3 PursuitOffset;

    [Header("Hide Configuration")]
    [HideInInspector]
    public Transform HideTarget;

    [Header("Flee Configuration")]
    [HideInInspector]
    public Transform FleeTarget;

    [Header("Evade Configuration")]
    [HideInInspector]
    public Transform EvadeTarget;

    [Header("Arrive Configuration")]
    [HideInInspector]
    public Transform ArriveTarget;

    [Header("Separation Configuration")]
    [HideInInspector]
    public float SeparationRadius;

    [Header("Alignment Configuration")]
    [HideInInspector]
    public float AlignmentRadius;

    [Header("Cohesion Configuration")]
    [HideInInspector]
    public float CohesionRadius;

    Rigidbody entityRigidbody;
    public Rigidbody EntityRigidbody { get { return entityRigidbody; } }

    bool isOnARamp;
    int groundedCount = 0;
    int terrainLayer;
    int terrainLayerMask;

    protected override void Awake()
    {
        base.Awake();
        currentVelocity = Vector3.zero;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        terrainLayerMask = (1 << terrainLayer);
        entityRigidbody = GetComponent<Rigidbody>();

        GenerateActiveMovementBehaviorsFromEnums(initialMovementBehaviors);
    }

    public void ClearMovementBehaviors()
    {
        activeMovementBehaviors.Clear();
    }

    public void GenerateActiveMovementBehaviorsFromEnums(List<MovementBehaviorTypes> newBehaviors)
    {
        activeMovementBehaviors = new List<AutonomousMovementBehavior>();

        for (int i = 0; i < newBehaviors.Count; i++)
        {
            AutonomousMovementBehavior behaviorToAdd = GetMovementBehaviorClass(newBehaviors[i]);

            activeMovementBehaviors.Add(behaviorToAdd);
        }
    }

    protected override void Start()
    {
        base.Start();
        entityEmitter.EmitEvent(EntityEvents.TargetUpdated);
    }

    protected override void Subscribe()
    {
        entityEmitter.SubscribeToEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    protected override void Unsubscribe()
    {
        entityEmitter.UnsubscribeFromEvent(EntityEvents.FixedUpdate, OnFixedUpdate);
    }

    void OnFixedUpdate()
    {
        //if (!isOnARamp && groundedCount == 0)
        //{
        //    Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - GameManager.GetEntityFallSpeed * Time.deltaTime, transform.position.z);
        //    transform.position = newPosition;

        //    currentVelocity = Vector3.zero;
        //    return;
        //}
        if (ArriveTarget != null)
        {
            Vector3 toTarget = ArriveTarget.position - transform.position;
            if (toTarget.sqrMagnitude < 0.1f)
            {
                entityRigidbody.velocity = Vector3.zero;
                return;
            }
        }

        AccumulateForce();
    }

    AutonomousMovementBehavior GetMovementBehaviorClass(MovementBehaviorTypes behaviorType)
    {
        string ns = typeof(MovementBehaviorTypes).Namespace;

        string typeName = ns + "." + behaviorType.ToString();

        return (AutonomousMovementBehavior)Activator.CreateInstance(Type.GetType(typeName));
    }

    void AccumulateForce()
    {
        if (activeMovementBehaviors.Count == 0)
        {
            currentVelocity = Vector3.zero;
            return;
        }
        Vector3 accumulatedForce = Vector3.zero;
        for (int i = 0; i < activeMovementBehaviors.Count; i++)
        {
            AutonomousMovementBehavior behavior = activeMovementBehaviors[i];

            Vector3 resultantForce = behavior.CalculateForce(this);

            float magnitudeOfResultantForce = resultantForce.magnitude;
            float remainingForce = maximumSteeringForce - accumulatedForce.magnitude;

            if (magnitudeOfResultantForce <= remainingForce)
            {
                accumulatedForce += resultantForce;
            }
            else
            {
                resultantForce = Vector3.ClampMagnitude(resultantForce, remainingForce);
                accumulatedForce += resultantForce;
                break;
            }
        }

        accumulatedForce.y = 0f;

        Vector3 acceleration = accumulatedForce / entityRigidbody.mass;
        currentVelocity += acceleration * Time.deltaTime;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

        Vector3 projectedPosition = transform.position + (currentVelocity * Time.deltaTime);
        projectedPosition.y = 20f;

        RaycastHit hit;
        if (Physics.Raycast(projectedPosition, Vector3.down, out hit, float.MaxValue, terrainLayerMask))
        {
            projectedPosition.y = hit.point.y + (transform.lossyScale.y);
            Vector3 toPosition = projectedPosition - transform.position;
            toPosition = Vector3.ClampMagnitude(toPosition, maxSpeed);
            transform.position += toPosition;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == terrainLayer)
        {
            groundedCount++;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == terrainLayer)
        {
            groundedCount--;
        }
    }
}
