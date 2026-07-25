using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BulletStorageScript
{
	// Bullet tracking
	static List<Bullet> bulletList = new List<Bullet>();
	public static int MaximumBulletCount => 6;
	public static int bulletCount => bulletList.Count;
	public static int lastBulletIndex => bulletCount - 1;
	public static int currentBulletIndex = 0;
	[SerializeField] private static float reloadTime = 1f;

	public class Bullet
	{
		public BulletData bulletData;
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
			if (currentBulletIndex < 0) EventBus<StartReload>.Call(new StartReload { reloadTime = reloadTime });
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
			EventBus<StartReload>.Call(new StartReload { reloadTime = 0 });
		}
	}

	public static void Reset()
	{
		bulletList = new List<Bullet>();
	}

	public static void Reload() 
	{
		currentBulletIndex = lastBulletIndex;
		EventBus<OnReload>.Call(new OnReload { });
	}

	public static void SwapBullets(int firstBullet, int secondBullet)
	{
		Bullet bulletOne = bulletList.FirstOrDefault(e => e.bulletIndex == firstBullet);
		Bullet bulletTwo = bulletList.FirstOrDefault(e => e.bulletIndex == secondBullet);

		if (bulletOne != null && bulletTwo != null)
		{
			int temp = bulletOne.bulletIndex;
			bulletOne.SetIndex(bulletTwo.bulletIndex);
			bulletTwo.SetIndex(temp);
			
			// Swap the places in the list
			(bulletList[firstBullet], bulletList[secondBullet]) = (bulletList[secondBullet], bulletList[firstBullet]);
			
			EventBus<OnBulletsSwapped>.Call(new OnBulletsSwapped() {firstBullet = firstBullet, secondBullet = secondBullet});
		} 
	}

	public static void ExchangeBullet(int firstBullet, BulletData bulletData, int selection)
	{
		Bullet bulletOne = bulletList.FirstOrDefault(e => e.bulletIndex == firstBullet);
		if (bulletOne != null)
		{
			BulletData temp = bulletOne.bulletData;
			bulletOne.bulletData = bulletData;

			EventBus<OnBulletsExchanged>.Call(new OnBulletsExchanged() { firstBullet = firstBullet, cylinderBulletData = temp, incomingBulletData = bulletData, selection = selection});
		}
	}

	public static BulletData GetBulletData(int id)
	{
		if (id >= bulletList.Count) return null;
		
		return bulletList[id].bulletData;
	}
}
