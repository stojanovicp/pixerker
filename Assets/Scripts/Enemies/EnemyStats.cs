using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public int maxHealth = 3;
    public float moveSpeed = 1f;
    public int contactDamage = 1;
    public float attackRange = 0f;
    public float attackCooldown = 2f;
    public float roamDirectionChangeTime = 2f;
    public bool stopMovingWhileAttacking = false;
}
