using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleManager : MonoBehaviour {

    [SerializeField]
    Image speechBubble;
    [SerializeField]
    GameObject dreamObject;
    [SerializeField]
    float dreamObjectXOffset = 80;
    [SerializeField]
    float dreamObjectYOffset = 65;
    [SerializeField]
    float dreamObjectZOffset = -20;
    [SerializeField]
    Transform player;

    Camera mainCamera;

    private void OnEnable()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        //Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(player.position);
        //speechBubble.transform.position = playerScreenPosition;
        speechBubble.transform.position = player.transform.position;
    }
}
