using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Bullets/BulletData")]
public class BulletData : ScriptableObject
{
    [Header("Objects")]
    public GameObject bulletPrefab;
    [Header("Sprites")]
    public Sprite chamberSprite;
    [Header("Text")]
    [TextArea(0,1)]
    public string bulletName;
    [TextArea(2,4)]
    public string bulletDescription;
}
