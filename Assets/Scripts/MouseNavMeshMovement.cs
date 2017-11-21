using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseNavMeshMovement : MonoBehaviour {
    NavMeshAgent agent;
    LayerMask terrainLayer;
    Camera mainCamera;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        terrainLayer = LayerMask.NameToLayer("Terrain");
        terrainLayer = ~terrainLayer;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, terrainLayer))
            {
                Debug.Log("ding");
                agent.destination = hit.point;
            }

        }
    }
}
