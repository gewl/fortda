using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class GameManager : SerializedMonoBehaviour {

    static GameManager instance;

    // State
    static bool isPaused = false;

    // Player references
    static GameObject playerObject;
    static Transform playerTransform;
    Plane playerPlane;

    [SerializeField]
    float entityFallSpeed = 30f;
    [SerializeField]
    float timeToBirthPlayer;
    static public float GetEntityFallSpeed { get { return instance.entityFallSpeed; } }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(BirthPlayer());
    }

    void Update()
    {
        playerPlane = new Plane(Vector3.up, GetPlayerPosition());
    }

    #region lazyload references
    static Transform _landscape;
    public static Transform Landscape
    {
        get
        {
            if (_landscape == null)
            {
                _landscape = GameObject.Find("Landscape").transform;
            }

            return _landscape;
        }
    }

    static Camera _mainCamera;
    static Camera mainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            return _mainCamera;
        }
    }
    
    static GameObject _hud;
    public static GameObject HUD
    {
        get
        {
            if (_hud == null)
            {
                _hud = GameObject.FindGameObjectWithTag("HUD");
            }

            return _hud;
        }
    }

    static List<EntityEmitter> _activeEmittersInScene;
    static List<EntityEmitter> activeEmittersInScene
    {
        get
        {
            if (_activeEmittersInScene == null)
            {
                _activeEmittersInScene = new List<EntityEmitter>();
            }

            return _activeEmittersInScene;
        }
    }


    static CameraController _cameraController;
    static CameraController cameraController
    {
        get
        {
            if (_cameraController == null)
            {
                _cameraController = mainCamera.GetComponent<CameraController>();
            }

            return _cameraController;
        }
    }

    private static GameObject _player;
    private static GameObject player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
            }

            return _player;
        }
    }
    #endregion

    #region gamestate handlers

    public static void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            HUD.SetActive(false);
            MuteAllEmitters();
            Time.timeScale = 0f;
            cameraController.ApplyPauseFilter();
        }
        else
        {
            HUD.SetActive(true);
            UnmuteAllEmitters();
            Time.timeScale = 1f;
            cameraController.RevertToOriginalProfile();
        }
    }

    #endregion

    #region en-masse entity manipulation

    public static void RegisterEmitter(EntityEmitter emitter)
    {
        activeEmittersInScene.Add(emitter);
    }

    public static void DeregisterEmitter(EntityEmitter emitter)
    {
        activeEmittersInScene.Remove(emitter);
    }

    public static void MuteAllEmitters()
    {
        for (int i = 0; i < activeEmittersInScene.Count; i++)
        {
            activeEmittersInScene[i].isMuted = true;
        }
    }

    public static void UnmuteAllEmitters()
    {
        for (int i = 0; i < activeEmittersInScene.Count; i++)
        {
            activeEmittersInScene[i].isMuted = false;
        }
    }

    #endregion

    #region entity data retrieval
    public static GameObject GetPlayerObject()
    {
        return player;
    }

    public static Transform GetPlayerTransform()
    {
        if (playerTransform == null)
        {
            playerTransform = player.transform;
        }
        return playerTransform;
    }

    public static Vector3 GetPlayerPosition()
    {
        if (playerTransform == null)
        {
            playerTransform = player.transform;
        }
        return playerTransform.position;
    }

    public static Transform GetHidingSpot(Transform agent, Transform target)
    {
        Vector3 agentPosition = agent.position;
        Vector3 targetPosition = target.position;

        Vector3 positionNearTarget = ((targetPosition * 2f) + agentPosition) / 3f;

        float distanceFromObstacleToTarget = float.MaxValue;
        Transform nearestObstacle = agent;

        for (int i = 0; i < Landscape.childCount; i++)
        {
            Transform obstacle = Landscape.GetChild(i);
            float sqrDistanceToPosition = (obstacle.position - positionNearTarget).sqrMagnitude;

            if (sqrDistanceToPosition < distanceFromObstacleToTarget)
            {
                nearestObstacle = obstacle;
                distanceFromObstacleToTarget = sqrDistanceToPosition;
            }
        }

        return nearestObstacle;
    }
    #endregion

    IEnumerator BirthPlayer()
    {
        MeshRenderer objectRenderer = player.GetComponent<MeshRenderer>();
        Collider objectCollider = player.GetComponent<Collider>();
        Rigidbody objectRigidbody = player.GetComponent<Rigidbody>();

        objectCollider.enabled = false;
        Vector3 originalPosition = player.transform.position;
        Vector3 destination = new Vector3(originalPosition.x, originalPosition.y + player.transform.lossyScale.y + 20f, originalPosition.z);

        float timeElapsed = 0.0f;
        while (timeElapsed < timeToBirthPlayer)
        {
            float percentageComplete = timeElapsed / timeToBirthPlayer;

            player.transform.position = Vector3.Lerp(originalPosition, destination, percentageComplete);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        objectRigidbody.velocity = Vector3.up * 20f;
        yield return new WaitForFixedUpdate();
        objectCollider.enabled = true;
        player.GetComponent<EntityEmitter>().isMuted = false;
    }

   #region input data retrieval

    public static Vector3 GetMousePositionOnPlayerPlane()
    {
        if (playerTransform == null)
        {
            return Vector3.zero;     
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        Vector3 relativeMousePosition;
        if (instance.playerPlane.Raycast(ray, out distance))
        {
            relativeMousePosition = ray.GetPoint(distance);
        }
        else
        {
            relativeMousePosition = Vector3.zero;
            relativeMousePosition.y = playerTransform.position.y;
        }

        return relativeMousePosition;
    }

    public static Vector3 GetMousePositionInWorldSpace()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100))
        {
            return hit.point;
        }
        else
        {
            Debug.LogWarning("Mouse position not found.");
            return mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    #endregion
}
