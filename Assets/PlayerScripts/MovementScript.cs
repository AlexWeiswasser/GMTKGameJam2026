using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
	[Header("References")]
	[SerializeField] Rigidbody2D rb;

	[Header("Stats")]
	[SerializeField] float maxSpeed;
	[SerializeField] float acceleration;

	public void MoveLeft(InputAction.CallbackContext context) { if (context.performed) rb.AddForce(Vector2.left * acceleration); }
	public void MoveRight(InputAction.CallbackContext context) { if(context.performed) rb.AddForce(Vector2.right * acceleration); }
	public void MoveDown(InputAction.CallbackContext context) { if(context.performed) rb.AddForce(Vector2.down * acceleration); }
	public void MoveUp(InputAction.CallbackContext context) { if(context.performed) rb.AddForce(Vector2.up * acceleration); }

	private void FixedUpdate()
	{
		rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
		rb.linearVelocityY = Mathf.Clamp(rb.linearVelocityY, -maxSpeed, maxSpeed);
	}
}
