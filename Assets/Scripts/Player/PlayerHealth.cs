using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public static event Action<int, int> HealthChanged;

    public bool IsDead { get; private set; }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float damageRecoveryTime = 0.5f;
    [SerializeField] private float respawnDelay = 2f;

    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;

    const string RESPAWN = "Scene_1";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        knockback = GetComponent<Knockback>();
        flash = GetComponent<Flash>();
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

    private void CheckIfPlayerDeath()
    {
        if(currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }

    private IEnumerator DeathLoadSceneRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        Destroy(gameObject);
        Stamina.Instance.ReplenishStaminaOnDeath();
        SceneManagement.Instance.ClearTransitionName();
        SceneManager.LoadScene(RESPAWN);
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
