using UnityEngine;

public class JacketBullet : BulletScript
{
	protected override void OnCharacterEnter(Collider2D collision)
	{
		if(collision.TryGetComponent<ICharInterface>(out var character))
		{
			character.SetMetalMarked();
		}
	}
}
