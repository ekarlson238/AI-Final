using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField]
    private float maxMinX, maxMinZ;
    private float minX, maxX, minZ, maxZ;

    private void Start()
    {
        minX = -maxMinX;
        maxX = maxMinX;
        minZ = -maxMinZ;
        maxZ = maxMinZ;
    }

    public void SpawnNewCarrot(GameObject carrot)
    {
        Vector3 tarPos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        carrot.transform.position = tarPos;
        carrot.SetActive(true);
    }
}
