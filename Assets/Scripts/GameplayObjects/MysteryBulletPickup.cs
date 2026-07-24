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
        List<BulletData> scouting = new();
        for (int i = 0; i < 3; i++)
        {
            while (scouting.Count <= 0)
            {
                Rarity rarity = ChooseRarity();
                scouting = bulletLibrary.GetBulletsOfRarity(rarity);

                // Prevent Duplicate Choices
                for (int a = 0; a < 2; a++)
                {
                    if (choices[a] != null)
                    {
                        if (scouting.Contains(choices[a]))
                        {
                            scouting.Remove(choices[a]);
                        }
                    }
                }
            }

            BulletData choice = scouting[UnityEngine.Random.Range(0, scouting.Count)];
            choices[i] = choice;
        }
        return choices;
    }

    private Rarity ChooseRarity()
    {
        int number = UnityEngine.Random.Range(0, TotalWeight + 1);
        int i = commonWeight;
        if (number <= commonWeight) return Rarity.Common;
        i += uncommonWeight;
        if (number <= uncommonWeight) return Rarity.Uncommon;
        i += rareWeight;
        if (number <= rareWeight) return Rarity.Rare;
        return Rarity.Rare;
    }
}
