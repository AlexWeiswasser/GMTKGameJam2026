using System;
using UnityEngine;

public class AreaSpawner : MonoBehaviour
{
    [SerializeField] AreaType[] areaTypes;

    [Serializable]
    public class AreaType
    {
        public Vector2 areaSize;
        public Transform areaSpawnerParent;
        public GameObject[] areas;
    }

    void Start()
    {
        foreach (AreaType areaType in areaTypes)
        {
            foreach (Transform zone in areaType.areaSpawnerParent.GetComponentInChildren<Transform>())
            {
                GameObject area = Instantiate(areaType.areas[UnityEngine.Random.Range(0, areaType.areas.Length)], transform);
                area.transform.position = new Vector2(zone.position.x - (areaType.areaSize.x / 2), zone.position.y + (areaType.areaSize.y / 2));
            }
        }
    }
}
