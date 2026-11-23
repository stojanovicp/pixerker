using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public int maxHealth = 3;
    public float moveSpeed = 1f;
    public int contactDamage = 1;
    [Tooltip("Bosses take only a share of their max HP from Execute hits.")]
    public bool isBoss;

    [Header("Ranges")]
    public float detectionRange = 6f;
    [Tooltip("Must be at least Detection Range; the gap stops the enemy flipping between roaming and chasing.")]
    public float loseRange = 9f;
    [Tooltip("0 means this enemy never attacks (contact damage only).")]
    public float attackRange = 0f;
    [Tooltip("The enemy stops moving toward the player at this distance.")]
    public float stoppingDistance = 0f;

    [Header("Timing")]
    public float attackCooldown = 2f;
    public float roamDirectionChangeTime = 2f;

    private void OnValidate()
    {
        detectionRange = Mathf.Max(0f, detectionRange);
        attackRange = Mathf.Max(0f, attackRange);
        loseRange = Mathf.Max(loseRange, detectionRange, attackRange);
        stoppingDistance = Mathf.Max(0f, stoppingDistance);

        if (attackRange > 0f)
        {
            stoppingDistance = Mathf.Min(stoppingDistance, attackRange);
        }
    }
}
