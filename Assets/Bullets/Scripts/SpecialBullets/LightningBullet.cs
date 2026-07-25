using UnityEngine;

public class LightningBullet : BulletScript
{
	protected override void OnCharacterEnter(Collider2D collision)
	{
		if (collision.TryGetComponent<ICharInterface>(out var character))
		{
			character.SetStunned();
		}
	}
}
