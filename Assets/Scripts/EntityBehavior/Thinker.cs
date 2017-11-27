using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thinker : MonoBehaviour {

    [SerializeField]
    SpeechBubbleManager bubbleManager;
    [SerializeField]
    GameObject holePrefab;

    public void Think()
    {
        bubbleManager.CreateNewBubble(holePrefab);
    }

    public void Think(GameObject subject)
    {
        bubbleManager.CreateNewBubble(subject);
    }
}
