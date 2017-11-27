using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinDreamObject : MonoBehaviour {

    float spinSpeed = 40f;

    void Update()
    {
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.y += spinSpeed * Time.deltaTime;
        eulerRotation.y %= 360f;

        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}
