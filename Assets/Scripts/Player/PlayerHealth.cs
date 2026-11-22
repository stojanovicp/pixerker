using System.Collections;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth>
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float damageRecoveryTime = 0.5f;

    private int currentHealth;
    private bool canTakeDamage = true;

    private Knockback knockback;
    private Flash flash;

    protected override void Awake()
    {
        base.Awake();

        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();

        if(enemy)
        {
            TakeDamage(1, collision.transform);
        }
    }

    public void HealPlayer(int healRate)
    {
        currentHealth += healRate;
    }

    public void TakeDamage(int damage, Transform hitTransform)
    {
        if(!canTakeDamage) { return; }

        ScreenShakeManager.Instance.ShakeScreen();

        canTakeDamage = false;
        currentHealth -= damage;

        knockback.GetKnockedBack(hitTransform, knockbackForce);
        StartCoroutine(flash.FlashRoutine());

        StartCoroutine(DamageRecoveryRoutine());
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }
}
