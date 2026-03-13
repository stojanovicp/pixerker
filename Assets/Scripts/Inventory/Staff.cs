using UnityEngine;

public class Staff : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        Debug.Log("Staff attack");
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }
}
