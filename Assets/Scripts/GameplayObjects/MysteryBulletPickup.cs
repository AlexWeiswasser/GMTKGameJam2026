using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static BulletData;

public class MysteryBulletPickup : MonoBehaviour
{
    [SerializeField] BulletLibrary bulletLibrary;

    readonly int commonWeight = 10;
    readonly int uncommonWeight = 5;
    readonly int rareWeight = 2;
    int TotalWeight => commonWeight + uncommonWeight + rareWeight;

    public void Pickup()
    {
        BulletData[] threeChoices = GenerateChoices();

        EventBus<OnMysteryBulletPickup>.Call(new OnMysteryBulletPickup() { choices = threeChoices});
    }

    private BulletData[] GenerateChoices()
    {
        BulletData[] choices = new BulletData[3];
        for (int i = 0; i < 3; i++)
        {
            List<BulletData> scouting;
            do
            {
                Rarity rarity = ChooseRarity();
                scouting = new List<BulletData>(bulletLibrary.GetBulletsOfRarity(rarity));

                for (int a = 0; a < i; a++)
                {
                    if (choices[a] != null)
                        scouting.Remove(choices[a]);
                }
            } while (scouting.Count <= 0);

            choices[i] = scouting[UnityEngine.Random.Range(0, scouting.Count)];
        }
        return choices;
    }

    private Rarity ChooseRarity()
    {
        int number = UnityEngine.Random.Range(0, TotalWeight);
        if (number < commonWeight) return Rarity.Common;
        if (number < commonWeight + uncommonWeight) return Rarity.Uncommon;
        return Rarity.Rare;
    }
}
