using System;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    [SerializeField] Vector2 areaSize;
    [SerializeField] Transform areaSpawnerParent;
    [SerializeField] GameObject[] areas;

    void Start()
    {
        foreach (Transform zone in areaSpawnerParent.GetComponentInChildren<Transform>())
        {
            GameObject area = Instantiate(areas[UnityEngine.Random.Range(0, areas.Length)], transform);
            area.transform.position = new Vector2(zone.position.x - (areaSize.x / 2), zone.position.y + (areaSize.y / 2));
        }
    }
}
