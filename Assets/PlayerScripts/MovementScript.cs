using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Rigidbody2D rb;

	[Header("Stats")]
	[SerializeField] float maxSpeed;
	[SerializeField] float acceleration;
	[SerializeField] float damping;

	// Tracking 
	bool[] heldDown = new bool[4];

	public void MoveLeft(InputAction.CallbackContext context) { if (context.performed) heldDown[0] = true; else heldDown[0] = false; }
	public void MoveRight(InputAction.CallbackContext context) { if(context.performed) heldDown[1] = true; else heldDown[1] = false; }
	public void MoveDown(InputAction.CallbackContext context) { if(context.performed) heldDown[2] = true; else heldDown[2] = false; }
	public void MoveUp(InputAction.CallbackContext context) { if(context.performed) heldDown[3] = true; else heldDown[3] = false; }

	private void FixedUpdate()
	{
		rb.linearDamping = damping;

		if (heldDown[0])
			rb.AddForce(Vector2.left * acceleration);
		if (heldDown[1])
			rb.AddForce(Vector2.right * acceleration);
		if (heldDown[2])
			rb.AddForce(Vector2.down * acceleration);
		if (heldDown[3])
			rb.AddForce(Vector2.up * acceleration);

		rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
		rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSpeed, maxSpeed);
	}
}
