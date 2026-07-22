using System.Collections.Generic;
using UnityEngine;

public static class BulletStorageScript
{
	static Queue<Bullet> bulletQueue = new Queue<Bullet>();

	public class Bullet
	{
		public GameObject bulletObject;

		// Stat changes
		public float speedIncrease = 0;
		public void IncreaseSpeed(float increase) { speedIncrease += increase; }
	}

	public static Bullet GetBullet()
	{
		if (bulletQueue.Count == 0) return null;
		else return bulletQueue.Dequeue();
	}

	public static void AddBullet(Bullet newBullet)
	{
		if (bulletQueue.Count == 6) return;
		else bulletQueue.Enqueue(newBullet);
	}
}
