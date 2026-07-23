using Unity.VisualScripting;

public interface ICharInterface
{
	public void Damage(int amount);
	public void SetOnFire();
	public void SetOnOilFire();
	public bool GetOnFire();
	public void SetStunned();
	public void SetMetalMarked();
	public void SetWet();
}
