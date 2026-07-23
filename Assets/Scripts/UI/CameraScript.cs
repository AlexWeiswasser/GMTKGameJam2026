using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraScript : MonoBehaviour
{
	[SerializeField] private float smoothDuration = 0.25f;
	[SerializeField] private Ease smoothEase = Ease.OutQuad;

	private Tween moveTween;

	void Update()
	{
		Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
		Vector2 cursorWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);

		Vector3 targetPos = (cursorWorldPos + MovementScript.playerPos + MovementScript.playerPos) / 3f;
		targetPos.z = -0.5f;

		moveTween?.Kill();
		moveTween = transform.DOMove(targetPos, smoothDuration).SetEase(smoothEase);
	}
}