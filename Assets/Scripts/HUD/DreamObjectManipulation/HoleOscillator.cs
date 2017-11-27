using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleOscillator : MonoBehaviour {

    float timeElapsed = 0.0f;

    void Update()
    {
        timeElapsed += Time.deltaTime;

        float xDisplacement = Mathf.PingPong(timeElapsed, 2f);
        xDisplacement -= 1f;

        transform.localPosition = new Vector3(transform.localPosition.x + xDisplacement, transform.localPosition.y, transform.localPosition.z);
    }
}
