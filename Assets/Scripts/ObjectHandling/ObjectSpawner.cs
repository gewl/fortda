using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    [SerializeField]
    Material initialSkin;
    [SerializeField]
    float timeToEmerge;

    Transform landscape;
    Transform objectsParent;
    Bounds landscapeBounds;

    int numberOfShapes;
    int numberOfColors;
    int numberOfSizes;

	void Start () {
        landscape = GameManager.Landscape;
        objectsParent = GameObject.Find("Objects").transform;
        TerrainCollider[] terrainColliders = landscape.GetComponentsInChildren<TerrainCollider>();

        for (int i = 0; i < terrainColliders.Length; i++)
        {
            TerrainCollider terrainCollider = terrainColliders[i];
            landscapeBounds.Encapsulate(terrainCollider.bounds);
        }

        numberOfShapes = Enum.GetNames(typeof(ObjectTraits.Shapes)).Length;
        numberOfColors = Enum.GetNames(typeof(ObjectTraits.Colors)).Length;
        numberOfSizes = Enum.GetNames(typeof(ObjectTraits.Sizes)).Length;

        SpawnObjectRandomly();
	}
	
    void SpawnObjectRandomly()
    {
        int randomShapeIndex = (int)Mathf.Round(UnityEngine.Random.Range(0, numberOfShapes));
        int randomColorIndex = (int)Mathf.Round(UnityEngine.Random.Range(0, numberOfColors));
        int randomSizeIndex = (int)Mathf.Round(UnityEngine.Random.Range(0, numberOfSizes));

        GameObject shape = TraitInformation.GetObjectShape((ObjectTraits.Shapes)randomShapeIndex);
        Material color = TraitInformation.GetObjectColor((ObjectTraits.Colors)randomColorIndex);
        Vector3 size = TraitInformation.GetObjectSize((ObjectTraits.Sizes)randomSizeIndex);

        float xLocation = UnityEngine.Random.Range(-landscapeBounds.extents.x + size.x, landscapeBounds.extents.x - size.x);
        float zLocation = UnityEngine.Random.Range(-landscapeBounds.extents.z + size.z, landscapeBounds.extents.z - size.z);

        Vector3 spawnLocation = new Vector3(xLocation, landscape.position.y - (size.y / 2f), zLocation);

        GameObject newObject = Instantiate(shape, spawnLocation, Quaternion.identity, objectsParent);
        newObject.transform.localScale = size;

        StartCoroutine(BirthTheThing(newObject, color));
    }

    IEnumerator BirthTheThing(GameObject newObject, Material color)
    {
        MeshRenderer objectRenderer = newObject.GetComponent<MeshRenderer>();
        Collider objectCollider = newObject.GetComponent<Collider>();

        objectCollider.enabled = false;
        objectRenderer.material = initialSkin;
        Vector3 originalPosition = newObject.transform.position;
        Vector3 destination = new Vector3(originalPosition.x, originalPosition.y + newObject.transform.localScale.y + 10, originalPosition.z);

        float timeElapsed = 0.0f;
        while (timeElapsed < timeToEmerge)
        {
            float percentageComplete = timeElapsed / timeToEmerge;

            newObject.transform.position = Vector3.Lerp(originalPosition, destination, percentageComplete);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        objectCollider.enabled = true;
    }
}
