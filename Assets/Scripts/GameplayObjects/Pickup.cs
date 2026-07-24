using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] BulletData bullet;
    public BulletData GetBullet() => bullet;
}
