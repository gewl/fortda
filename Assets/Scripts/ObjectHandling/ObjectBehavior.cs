using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehavior : MonoBehaviour {

    [SerializeField]
    GameObject impactSoundPrefab;

    public Material Color; 
    public GameObject Shape;
    public Vector3 Size;

    bool hasSounded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasSounded)
        {
            GameObject impactSound = Instantiate(impactSoundPrefab, transform.position, Quaternion.identity, null);
            impactSound.GetComponent<ImpactSoundHandler>().soundOrigin = transform;
            hasSounded = true;
        }
    }
}
