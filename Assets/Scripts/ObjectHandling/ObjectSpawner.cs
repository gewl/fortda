using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    Transform landscape;
    Bounds landscapeBounds;

	void Start () {
        landscape = GameManager.Landscape;
        TerrainCollider[] terrainColliders = landscape.GetComponentsInChildren<TerrainCollider>();

        for (int i = 0; i < terrainColliders.Length; i++)
        {
            TerrainCollider terrainCollider = terrainColliders[i];
            landscapeBounds.Encapsulate(terrainCollider.bounds);
        }

        Debug.Log(landscapeBounds.size);
	}
	
	void Update () {
		
	}
}
