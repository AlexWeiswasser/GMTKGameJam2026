using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
	[Header("References")]
	public static Transform playerTransform;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Transform spriteObject;
	public SpriteRenderer[] playerSpriteRenderers;

	[Header("Stats")]
	[SerializeField] float maxSpeed;
	[SerializeField] float acceleration;
	[SerializeField] float damping;

	[Header("Hop Animation")]
	[SerializeField] float hopDuration;
	[SerializeField] float hopStrength;
	//[SerializeField] float hopMagnitudeThreshold;
	[SerializeField] Ease hopEasing;
	[SerializeField] ParticleSystem landParticles;

	// Tracking 
	bool[] heldDown = new bool[4];

	bool animating = false;
	[HideInInspector] public bool canMove = true;
	[HideInInspector] public bool isWet = false;

	// Veclocities
	Vector2 movementVelocity;
	Vector2 externalVelocity;

	//sounds
	[SerializeField] AudioClip[] walkSounds;

	public void MoveLeft(InputAction.CallbackContext context) { if (context.performed) heldDown[0] = true; else heldDown[0] = false; }
	public void MoveRight(InputAction.CallbackContext context) { if(context.performed) heldDown[1] = true; else heldDown[1] = false; }
	public void MoveDown(InputAction.CallbackContext context) { if(context.performed) heldDown[2] = true; else heldDown[2] = false; }
	public void MoveUp(InputAction.CallbackContext context) { if(context.performed) heldDown[3] = true; else heldDown[3] = false; }

	private void OnEnable()
	{
		AimScript.OnGunFlip += FlipSprite;
	}

	private void OnDisable()
	{
		AimScript.OnGunFlip -= FlipSprite;
	}

	private void FlipSprite(bool flip)
	{
		foreach (SpriteRenderer spr in playerSpriteRenderers)
		{
			spr.flipX = flip;
		}
	}

	private void Awake()
	{
		playerTransform = transform;
	}

	public void Knockback(Vector2 direction, float strength)
	{
		CameraScript.ScreenShake(Camera.main.transform, strength);
		strength *= 50;

		externalVelocity = Vector2.MoveTowards(
			externalVelocity,
			direction * strength,
			strength * Time.fixedDeltaTime);
	}

	private void FixedUpdate()
	{
		rb.linearDamping = damping;
		if (canMove)
		{
			if (BulletSelectController.PlayerBusy()) return;

			float currentMaxSpeed = maxSpeed;
			if (isWet)
			{
			if(this.gameObject.CompareTag("Player"))
				currentMaxSpeed *= .8f;
			else
				currentMaxSpeed *= .5f;
			}

			Vector2 input = Vector2.zero;
			if (heldDown[0]) input.x -= 1;
			if (heldDown[1]) input.x += 1;
			if (heldDown[2]) input.y -= 1;
			if (heldDown[3]) input.y += 1;
			input = input.normalized;

			movementVelocity = Vector2.MoveTowards(
				movementVelocity,
				input * currentMaxSpeed,
				acceleration * Time.fixedDeltaTime);

			if (!animating && input != Vector2.zero)
			{
				animating = true;
				spriteObject.DOLocalJump(Vector2.zero, hopStrength, 1, hopDuration)
					.SetEase(hopEasing)
					.OnComplete(() => 
					{ 
						animating = false;
						landParticles.Play();	

						int choice = 0;
						choice = UnityEngine.Random.Range(0, walkSounds.Length);
						SoundManager.Instance.PlaySound(walkSounds[choice], .9f, 1.1f, .25f);
					});
			}
		}
		externalVelocity *= 0.9f;
		rb.linearVelocity = movementVelocity + externalVelocity;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Trigger"))
		{
			if (collision.TryGetComponent<Trigger>(out var tScript))
			{
				tScript.TriggerEvent();
				collision.enabled = false;
			}
		}
	}
}
