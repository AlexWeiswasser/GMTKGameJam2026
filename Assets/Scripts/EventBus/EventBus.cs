using System;
public interface IEvent { }

public static class EventBus<T> where T : IEvent
{
	private static event Action<T> OnEvent;

	public static void Subscribe(Action<T> handler) => OnEvent += handler;
	public static void Unsubscribe(Action<T> handler) => OnEvent -= handler;
	public static void Call(T evt) => OnEvent?.Invoke(evt);
}

// Bullet events
public struct StartReload : IEvent { public float reloadTime; }
public struct OnReload : IEvent { }
public struct OnBulletFire : IEvent { public int bulletNumber; }

// Swap events
public struct OnBulletsSwapped : IEvent { public int firstBullet; public int secondBullet; }
public struct OnBulletsExchanged : IEvent { public int firstBullet; public BulletData cylinderBulletData; public BulletData incomingBulletData; public int selection; }
