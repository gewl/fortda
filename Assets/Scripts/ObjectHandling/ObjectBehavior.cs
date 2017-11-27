using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehavior : MonoBehaviour {

    [SerializeField]
    GameObject impactSoundPrefab;

    bool hasSounded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasSounded)
        {
            GameObject impactSound = Instantiate(impactSoundPrefab, transform.position, Quaternion.identity, null);
            hasSounded = true;
        }
    }
}
