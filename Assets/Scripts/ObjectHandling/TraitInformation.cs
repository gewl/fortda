using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class TraitInformation : SerializedMonoBehaviour {

    [OdinSerialize]
    Dictionary<ObjectTraits.Shapes, GameObject> objectShapes;

    [SerializeField]
    Dictionary<ObjectTraits.Colors, Material> objectColors;

    [SerializeField]
    Dictionary<ObjectTraits.Sizes, Vector3> objectSizes;

    static TraitInformation instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    static public GameObject GetObjectShape(ObjectTraits.Shapes shape)
    {
        return instance.objectShapes[shape];
    }

    static public Material GetObjectColor(ObjectTraits.Colors color)
    {
        return instance.objectColors[color];
    }

    static public Vector3 GetObjectSize(ObjectTraits.Sizes size)
    {
        return instance.objectSizes[size];
    }
}
