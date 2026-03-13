using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Bow attack");
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }
}
