using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thinker : MonoBehaviour {

    [SerializeField]
    SpeechBubbleManager bubbleManager;

    [SerializeField]
    GameObject holePrefab;
    [SerializeField]
    GameObject interrobangPrefab;

    EntityStateMachine stateMachine;
    public Stack<Transform> objectsInvestigating;

    HashSet<Material> perceivedColorsSet;
    HashSet<GameObject> perceivedShapesSet;
    HashSet<Vector3> perceivedSizesSet;

    List<Material> perceivedColorsList;
    List<GameObject> perceivedShapesList;
    List<Vector3> perceivedSizesList;

    void OnEnable()
    {
        stateMachine = GetComponent<EntityStateMachine>();
        objectsInvestigating = new Stack<Transform>();

        perceivedColorsSet = new HashSet<Material>();
        perceivedShapesSet = new HashSet<GameObject>();
        perceivedSizesSet = new HashSet<Vector3>();

        perceivedColorsList = new List<Material>();
        perceivedShapesList = new List<GameObject>();
        perceivedSizesList = new List<Vector3>();
    }

    #region Objectival comprehension
    public void PerceiveObject(GameObject theObject)
    {
        ObjectBehavior objectBehavior = theObject.GetComponent<ObjectBehavior>();
        GameObject shape = objectBehavior.Shape;
        Material color = objectBehavior.Color;
        Vector3 size = objectBehavior.Size;
        bool hasSeenColor = true;
        bool hasSeenSize = true;
        bool hasSeenShape = true;

        if (!perceivedColorsSet.Contains(color))
        {
            hasSeenColor = false;
            perceivedColorsSet.Add(color);
            perceivedColorsList.Add(color);
        }

        if (!perceivedShapesSet.Contains(shape))
        {
            hasSeenShape = false;
            perceivedShapesSet.Add(shape);
            perceivedShapesList.Add(shape);
        }

        if (!perceivedSizesSet.Contains(size))
        {
            hasSeenSize = false;
            perceivedSizesList.Add(size);
            perceivedSizesSet.Add(size);
        }

        if (!hasSeenShape && !hasSeenColor && !hasSeenSize)
        {
            //visual representation of new object discovered
            Debug.Log("Totally new object");
            bubbleManager.CreateNewDiscoveryBubble(shape, color, size);
        }
    }
    #endregion  

    #region Phenomena management

    public void OnHearImpactSound(Transform noisyObject)
    {
        objectsInvestigating.Push(noisyObject);
        if (stateMachine.IsInState(typeof(SpottedObjectState)))
        {
            return;
        }
        Alert();
        if (stateMachine.IsInState(typeof(InvestigatingSoundState)))
        {
            stateMachine.Hesitate();
            CompareDistancesToInvestigate(noisyObject);
        }
        else
        {
            stateMachine.SwitchState(new InvestigatingSoundState(stateMachine));
        }
    }

    #endregion

    #region Bubble management
    public void Mull()
    {
        bubbleManager.CreateNewBubble(holePrefab);
    }

    public void Alert()
    {
        bubbleManager.CreateNewBubble(interrobangPrefab);
    }

    public void Think(GameObject subject)
    {
        bubbleManager.CreateNewBubble(subject);
    }
    #endregion

    void CompareDistancesToInvestigate(Transform newObject)
    {
        Transform oldObject = objectsInvestigating.Pop();
        Vector3 oldPosition = oldObject.position;
        Vector3 newPosition = newObject.position;

        float sqrDistanceToNewPosition = (newPosition - transform.position).sqrMagnitude;
        float sqrDistanceToOldPosition = (oldPosition - transform.position).sqrMagnitude;

        if (sqrDistanceToOldPosition > sqrDistanceToNewPosition)
        {
            objectsInvestigating.Push(oldObject);
            objectsInvestigating.Push(newObject);
        }
        else
        {
            objectsInvestigating.Push(oldObject);
            objectsInvestigating.Push(newObject);
        }
    }
}
