using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleManager : MonoBehaviour {

    [SerializeField]
    GameObject speechBubblePrefab;
    [SerializeField]
    GameObject darkSurfacePrefab;
    [SerializeField]
    Transform player;

    [SerializeField]
    ObjectSpawner objectSpawner;

    [SerializeField]
    float bubbleContentsXPos = 84f;
    [SerializeField]
    float bubbleContentsYPos = 66f;

    [SerializeField]
    float bubbleDuration = 1f;
    [SerializeField]
    float bubbleFadeTime = 0.5f;
    [SerializeField]
    AnimationCurve bubbleFadeCurve;
    float bubbleTimer;

    Camera mainCamera;
    Queue<GameObject> activeSpeechBubbles;
    float speechBubbleHeight;
    Vector3 contentsPosition;

    private void OnEnable()
    {
        contentsPosition = new Vector3(bubbleContentsXPos, bubbleContentsYPos, 0f);
        bubbleTimer = bubbleDuration;
        mainCamera = Camera.main;
        activeSpeechBubbles = new Queue<GameObject>();
        speechBubbleHeight = speechBubblePrefab.GetComponent<RectTransform>().rect.height;
    }

    private void Update()
    {
        if (activeSpeechBubbles.Count == 0)
        {
            return;
        }

        bubbleTimer -= Time.deltaTime;
        if (bubbleTimer <= 0)
        {
            GameObject lastBubble = activeSpeechBubbles.Dequeue();
            StartCoroutine(FadeBubble(lastBubble, false));
            bubbleTimer = bubbleDuration;
        }

        UpdateBubblePositions();
    }

    public void CreateNewBubble(GameObject bubbleContents)
    {
        GameObject newBubble = Instantiate(speechBubblePrefab, player.transform.position, transform.rotation, transform);
        GameObject contents = Instantiate(bubbleContents, newBubble.transform);
        contents.transform.localPosition = contentsPosition;

        StartCoroutine(FadeBubble(newBubble));
        activeSpeechBubbles.Enqueue(newBubble);
    }

    public void CreateNewDiscoveryBubble(GameObject shape, Material color, Vector3 size)
    {
        GameObject newBubble = Instantiate(speechBubblePrefab, player.transform.position, transform.rotation, transform);

        GameObject contents = objectSpawner.CreateAndReturnThoughtObject(shape, color, size);
        contents.transform.parent = newBubble.transform;

        Vector3 discoveryObjectPosition = new Vector3(contentsPosition.x, contentsPosition.y, -1f);
        contents.transform.localPosition = discoveryObjectPosition;

        Vector3 darkSurfacePosition = new Vector3(contentsPosition.x, contentsPosition.y, -3f);
        GameObject darkSurface = Instantiate(darkSurfacePrefab, newBubble.transform);
        darkSurface.transform.localPosition = darkSurfacePosition;

        StartCoroutine(FadeBubble(newBubble));
        activeSpeechBubbles.Enqueue(newBubble);
    }

    void UpdateBubblePositions()
    {
        Queue<GameObject>.Enumerator bubbleEnumerator = activeSpeechBubbles.GetEnumerator();
        int bubbleCounter = 0;
        while (bubbleEnumerator.MoveNext())
        {
            GameObject speechBubble = bubbleEnumerator.Current;

            Vector3 newPosition = player.transform.position;
            newPosition.y += (bubbleCounter * speechBubbleHeight);

            speechBubble.transform.position = newPosition;
            bubbleCounter++;
        }
    }

    IEnumerator FadeBubble(GameObject speechBubble, bool isFadingIn = true)
    {
        float initialFadeValue = 0f;
        float targetFadeValue = 1f;
        if (!isFadingIn)
        {
            initialFadeValue = 1f;
            targetFadeValue = 0f;
        }
        Renderer[] renderers = speechBubble.GetComponentsInChildren<Renderer>();
        List<Renderer> colorRenderers = new List<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer currentRenderer = renderers[i];
            if (currentRenderer.material.HasProperty("_Color"))
            {
                colorRenderers.Add(currentRenderer);
            }
        }
        Image speechBubbleImage = speechBubble.GetComponent<Image>();
        speechBubbleImage.CrossFadeAlpha(initialFadeValue, 0.0f, true);
        speechBubbleImage.CrossFadeAlpha(targetFadeValue, bubbleFadeTime, true);
        float timeElapsed = 0.0f;

        while (timeElapsed < bubbleFadeTime)
        {
            float percentageComplete = timeElapsed / bubbleFadeTime;

            for (int i = 0; i < colorRenderers.Count; i++)
            {
                Renderer currentRenderer = colorRenderers[i];
                Color materialColor = currentRenderer.material.color;
                materialColor.a = Mathf.Lerp(initialFadeValue, targetFadeValue, bubbleFadeCurve.Evaluate(percentageComplete));
                currentRenderer.material.color = materialColor;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (!isFadingIn)
        {
            Destroy(speechBubble);
        }
    }
} 