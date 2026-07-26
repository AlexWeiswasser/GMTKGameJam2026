using System.Collections.Generic;
using UnityEngine;

public interface ICharInterface
{
	public void Damage(int amount);
	public void Knockback(Vector2 direction, float amount);
	public void SetOnFire();
	public void SetOnOilFire();
	public bool GetOnFire();
	public void SetStunned(List<Collider2D> thoseAlreadyHit = null);
	public void SetMetalMarked();
	public void SetWet();
	public bool GetWet();
	public bool GetMetalMarked();
}
