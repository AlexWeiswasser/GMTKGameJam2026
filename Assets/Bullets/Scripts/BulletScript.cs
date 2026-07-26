using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static BulletStorageScript;

public class BulletScript : MonoBehaviour
{
	public enum BulletType
	{
		Default,
		Oil,
		Fire,
		FullMetalJacket,
		Lightning,
		Water,
		Knockback, 
		Recoil, 
		Vortex, 
		Shotgun, 
		Explosive
	}

	[Header("References")]
	[HideInInspector] public bool playerCreated;
	Rigidbody2D rb;
	

	[Header("Stats")]
	[SerializeField] float speed = 35f;
	[SerializeField] int knockback = 10;
	[SerializeField] int selfKnockback = 0;
	public BulletType type;
	public int damage = 10;

	[Header("Shotgun Stuff")]
	[SerializeField] bool isShotgun = false;
	[SerializeField] bool isShotgunPellet = false;
	[SerializeField] GameObject bulletPrefab = null;
	[SerializeField] TrailRenderer tr;

	public void Init(bool createdByPlayer)
	{
		playerCreated = createdByPlayer;
		if (!createdByPlayer)
			speed /= 2;

		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		Vector2 mouseScreenPos;
		Vector2 cursorWorldPos;
		Vector2 direction;

		// Rotation
		if (playerCreated)
		{
			if (selfKnockback != 0)
			{
				MovementScript mScript = GameObject.FindAnyObjectByType<MovementScript>();
				if (mScript != null)
				{
					direction = (MovementScript.playerTransform.position - transform.position).normalized;
					mScript.Knockback(direction, selfKnockback);
				}
			}

			mouseScreenPos = Mouse.current.position.ReadValue();
			cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);
			direction = (cursorWorldPos - (Vector2)MovementScript.playerTransform.position).normalized;
		}
		else
		{
			direction = ((Vector2)MovementScript.playerTransform.position - (Vector2)transform.position).normalized;
		}

		if (!isShotgunPellet)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle + 90);

			if (isShotgun)
			{
				for (int i = 0; i < 3; i++)
				{
					float angleOffset = 30f - (i * 30f);
					Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, angleOffset);

					BulletScript bulletScript = Instantiate(bulletPrefab, transform.position, rotation)
						.GetComponent<BulletScript>();
					bulletScript.Init(createdByPlayer: playerCreated);
				}
				Destroy(gameObject);
				return;
			}

			// Velocity
			rb.linearVelocity = speed * direction;
		}
		else
		{
			rb.linearVelocity = speed * -transform.up;
		}

		// Lifetime
		Destroy(gameObject, 5);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Effect"))
			OnEffectEnter(collision);
		else if (collision.gameObject.layer == LayerMask.NameToLayer("Character"))
		{
			if (collision.CompareTag("Player") != playerCreated)
			{
				if (collision.TryGetComponent<ICharInterface>(out var face))
				{
					Vector2 direction = (collision.transform.position - transform.position).normalized;
					face.Knockback(direction, knockback);
					face.Damage(damage);
				}
				OnCharacterEnter(collision);
				OnBulletDeath();
				tr.transform.parent = null;
				tr.autodestruct = true;
				tr.time = 0.1f;
				Destroy(gameObject);
			}
		}
		else 
		{
			OnBulletDeath();
			tr.transform.parent = null;
			tr.autodestruct = true;
			tr.time = 0.1f;
			Destroy(gameObject);
		}
	}


	protected virtual void OnEffectEnter(Collider2D collision) { }
	protected virtual void OnCharacterEnter(Collider2D collision) { }
	protected virtual void OnBulletDeath() { }
}
