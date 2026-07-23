using UnityEngine;

public class Timer
{
	float duration, elapsed;
	bool pause;

	public bool Finished => elapsed >= duration;
	public float Progress => Mathf.Clamp01(elapsed / duration);
	public float Remaining => Mathf.Max(0f, duration - elapsed);
	public bool Paused => pause;

	public Timer(float duration, bool startFinished = false)
	{
		this.duration = duration;
		elapsed = startFinished ? duration : 0f;
	}

	public void Tick(float dt)
	{
		if (!pause) elapsed += dt;
	}

	public void Reset(float newDuration = -1)
	{
		elapsed = 0f;
		if (newDuration >= 0) duration = newDuration;
	}

	public void Pause() { pause = true; }
	public void UnPause() { pause = false; }
}