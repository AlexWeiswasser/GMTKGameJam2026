using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Bullets/BulletData")]
public class BulletData : ScriptableObject
{
    public GameObject bulletPrefab;
    public Sprite chamberSprite;
}
