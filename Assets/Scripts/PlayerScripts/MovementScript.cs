using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
	[Header("References")]
	public static Vector2 playerPos;
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Transform spriteObject;
	[SerializeField] SpriteRenderer[] playerSpriteRenderers;

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

	private void Update()
	{
		playerPos = transform.position;
	}

	private void FixedUpdate()
	{
		rb.linearDamping = damping;

		if (!canMove) return;

		bool anyHeldDown = heldDown[0] || heldDown[1] || heldDown[2] || heldDown[3];

		if (heldDown[0])
			rb.AddForce(Vector2.left * acceleration);
		if (heldDown[1])
			rb.AddForce(Vector2.right * acceleration);
		if (heldDown[2])
			rb.AddForce(Vector2.down * acceleration);
		if (heldDown[3])
			rb.AddForce(Vector2.up * acceleration);

		float currentMaxSpeed = maxSpeed;
		if(isWet) currentMaxSpeed *= .8f;

		rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
		rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSpeed, maxSpeed);

		if (!animating && anyHeldDown)
		{
			animating = true;
			spriteObject.DOLocalJump(Vector2.zero, hopStrength, 1, hopDuration)
				.SetEase(hopEasing)
				.OnComplete(() => { animating = false; landParticles.Play();});
		}
	}
}
