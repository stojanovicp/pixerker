using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public static event Action<int, int> HealthChanged;

    public event Action Died;

    public bool IsDead { get; private set; }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float damageRecoveryTime = 0.5f;

    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private Animator myAnimator;

    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this) { return; }

        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        IsDead = false;
        currentHealth = maxHealth;
        RaiseHealthChanged();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();

        if(enemy)
        {
            TakeDamage(enemy.ContactDamage, collision.transform);
        }
    }

    public void HealPlayer(int healAmount)
    {
        if(IsDead) { return; }

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        RaiseHealthChanged();
    }

    public void TakeDamage(int damage, Transform hitTransform)
    {
        if(!canTakeDamage || IsDead) { return; }

        ScreenShakeManager.Instance.ShakeScreen();

        canTakeDamage = false;
        currentHealth -= damage;

        knockback.GetKnockedBack(hitTransform.position, knockbackForce);
        StartCoroutine(flash.FlashRoutine());

        StartCoroutine(DamageRecoveryRoutine());

        CheckIfPlayerDeath();
        RaiseHealthChanged();
    }

    // Clears everything a death can leave behind: health, i-frames, knockback, flash and the death animation.
    public void ResetForRespawn()
    {
        StopAllCoroutines();

        IsDead = false;
        canTakeDamage = true;
        currentHealth = maxHealth;

        knockback.ResetKnockback();
        flash.RestoreDefaultMaterial();

        myAnimator.Rebind();
        myAnimator.Update(0f);

        RaiseHealthChanged();
    }

    private void CheckIfPlayerDeath()
    {
        if(currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            currentHealth = 0;
            myAnimator.SetTrigger(DEATH_HASH);
            Died?.Invoke();
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void RaiseHealthChanged()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
