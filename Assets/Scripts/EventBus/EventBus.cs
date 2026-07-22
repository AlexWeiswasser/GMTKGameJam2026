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
public struct OnBulletGain : IEvent { public int bulletNumber; }
public struct OnBulletFire : IEvent { public int bulletNumber; }
