using System.Collections.Generic;
using UnityEngine;

public static class BulletStorageScript
{
	static Stack<Bullet> bulletStack = new Stack<Bullet>();

	public class Bullet
	{
		public GameObject bulletObject;
		public int bulletIndex { get; private set; }
		public void SetIndex(int index) { bulletIndex = index; }

		// Stat changes
		public float speedIncrease { get; private set; } = 0;
		public void IncreaseSpeed(float increase) { speedIncrease += increase; }
	}

	public static Bullet GetBullet()
	{
		if (bulletStack.Count == 0) return null;
		else
		{
			Bullet oldBullet = bulletStack.Pop();
			EventBus<OnBulletFire>.Call(new OnBulletFire { bulletNumber = oldBullet.bulletIndex });
			return oldBullet;
		}
	}

	public static void AddBullet(Bullet newBullet)
	{
		if (bulletStack.Count == 6) return;
		else
		{
			newBullet.SetIndex(bulletStack.Count);
			EventBus<OnBulletGain>.Call(new OnBulletGain { bulletNumber = bulletStack.Count });
			bulletStack.Push(newBullet);
		}
	}
}
