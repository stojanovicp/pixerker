using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool IsDead { get; private set; }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float damageRecoveryTime = 0.5f;

    private Slider healthSlider;
    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;

    const String HEALTH_SLIDER_TEXT = "Health Slider";
    const String RESPAWN = "Scene_1";
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
        UpdateHealthSlider();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();

        if(enemy)
        {
            TakeDamage(1, collision.transform);
        }
    }

    public void HealPlayer(int healAmount)
    {
        if(IsDead) { return; }

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthSlider();
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
        yield return new WaitForSeconds(2f);
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

        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateHealthSlider()
    {
        if(healthSlider == null)
        {
            healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
