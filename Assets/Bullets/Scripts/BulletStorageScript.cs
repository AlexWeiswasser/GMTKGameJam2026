using System.Collections.Generic;
using UnityEngine;

public static class BulletStorageScript
{
	// Bullet tracking
	static List<Bullet> bulletList = new List<Bullet>();
	public static int MaximumBulletCount => 6;
	public static int bulletCount => bulletList.Count;
	public static int lastBulletIndex => bulletCount - 1;
	public static int currentBulletIndex = 0;

	public class Bullet
	{
		public GameObject bulletObject;
		public int bulletIndex { get; private set; }
		public void SetIndex(int index) { bulletIndex = index; }
	}

	public static Bullet FireBullet()
	{
		if (bulletList.Count == 0 || currentBulletIndex < 0) return null;
		else
		{
			Bullet oldBullet = bulletList[currentBulletIndex];
			currentBulletIndex--;

			EventBus<OnBulletFire>.Call(new OnBulletFire { bulletNumber = oldBullet.bulletIndex });
			if (currentBulletIndex < 0) Reload();
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
		currentBulletIndex = lastBulletIndex;
		EventBus<OnReload>.Call(new OnReload { });
	}
}
