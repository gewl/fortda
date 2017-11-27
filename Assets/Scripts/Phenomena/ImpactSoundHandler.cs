using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundHandler : MonoBehaviour {

    bool hasBeenHeard = false;
    float soundTimer = 0.5f;
    public Transform soundOrigin;

    private void Update()
    {
        soundTimer -= Time.deltaTime;
        if (soundTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasBeenHeard)
        {
            Thinker colliderThinker = other.GetComponent<Thinker>();
            colliderThinker.OnHearImpactSound(soundOrigin);
            hasBeenHeard = true;
        }
    }
}
