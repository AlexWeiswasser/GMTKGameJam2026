using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject bullet;
    public GameObject GetBullet() => bullet;
}
