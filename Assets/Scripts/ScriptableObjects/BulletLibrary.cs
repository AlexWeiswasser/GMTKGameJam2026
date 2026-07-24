using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletLibrary", menuName = "Bullets/BulletLibrary")]
public class BulletLibrary : ScriptableObject
{
    public List<BulletData> allBulletData;

    public List<BulletData> GetBulletsOfRarity(BulletData.Rarity rarity)
    {
        return allBulletData.Where(e => e.rarity == rarity).ToList();
    }
}
