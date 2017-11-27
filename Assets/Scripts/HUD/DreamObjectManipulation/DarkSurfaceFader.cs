using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSurfaceFader : MonoBehaviour {

    ParticleSystem thisParticleSystem;

    void OnEnable()
    {
        thisParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!thisParticleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
