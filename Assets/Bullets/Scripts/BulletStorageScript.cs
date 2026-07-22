using System.Collections.Generic;
using UnityEngine;

public static class BulletStorageScript
{
	// Bullet tracking
	static List<Bullet> bulletList = new List<Bullet>();
	public static int bulletCount = bulletList.Count;
	static int currentBulletIndex = 0;

	public class Bullet
	{
		public GameObject bulletObject;
		public int bulletIndex { get; private set; }
		public void SetIndex(int index) { bulletIndex = index; }

		// Stat changes
		public float speedIncrease { get; private set; } = 0;
		public void IncreaseSpeed(float increase) { speedIncrease += increase; }
	}

	public static Bullet FireBullet()
	{
		if (bulletList.Count == 0 || currentBulletIndex >= bulletCount) return null;
		else
		{
			Bullet oldBullet = bulletList[currentBulletIndex];
			currentBulletIndex++;

			EventBus<OnBulletFire>.Call(new OnBulletFire { bulletNumber = oldBullet.bulletIndex });
			return oldBullet;
		}
	}

	public static void AddBullet(Bullet newBullet)
	{
		if (bulletList.Count == 6) return;
		else
		{
			newBullet.SetIndex(bulletList.Count);
			bulletList.Add(newBullet);
			Reload();
		}
	}

	public static void Reload()
	{
		currentBulletIndex = 0;
		EventBus<OnReload>.Call(new OnReload { });
	}
}
