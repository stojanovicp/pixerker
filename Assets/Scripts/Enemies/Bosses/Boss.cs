using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// A boss on top of the normal enemy components. It is the EnemyAI's attacker (IEnemy): each attack picks a
// random entry from the current phase. Phase two starts once, below a share of max HP. While engaged it
// summons enemies; on death it drops its weapon and kills its living summons.
[RequireComponent(typeof(EnemyHealth), typeof(EnemyAI), typeof(EnemyPathfinding))]
public class Boss : MonoBehaviour, IEnemy
{
    [Serializable]
    public class PhaseAttack
    {
        [Tooltip("A component on this boss that can run patterns (Shooter or Grape).")]
        public MonoBehaviour attacker;
        public AttackPattern pattern;
    }

    [Serializable]
    public class Phase
    {
        public List<PhaseAttack> attacks = new List<PhaseAttack>();
        [Tooltip("0 keeps the speed from EnemyStats.")]
        [Min(0f)] public float moveSpeed;
        [Tooltip("0 keeps the attack cooldown from EnemyStats.")]
        [Min(0f)] public float attackCooldown;
        [Tooltip("Seconds between summon waves (random between x and y).")]
        public Vector2 summonInterval = new Vector2(10f, 15f);
    }

    [Serializable]
    public class Summon
    {
        public GameObject prefab;
        [Min(0)] public int minCount = 1;
        [Min(0)] public int maxCount = 1;
    }

    // The health bar listens to these. Disengaged also fires when the boss's scene unloads.
    public static event Action<Boss> Engaged;
    public static event Action<Boss> Disengaged;

    // Raised when a boss marked as the final boss dies: the game is won.
    public static event Action FinalBossDefeated;

    [SerializeField] private string displayName = "Boss";
    [SerializeField] private Phase phaseOne = new Phase();
    [SerializeField] private Phase phaseTwo = new Phase();
    [Range(0f, 1f)] [SerializeField] private float phaseTwoHealthFraction = 0.5f;

    [Header("Summons")]
    [SerializeField] private List<Summon> summons = new List<Summon>();
    [Tooltip("Summons appear at a random distance between x and y from the boss.")]
    [SerializeField] private Vector2 summonDistance = new Vector2(4f, 6f);
    [Min(0)] [SerializeField] private int maxAliveSummons = 6;

    [Header("Reward")]
    [SerializeField] private WeaponInfo droppedWeapon;
    [Tooltip("Killing this boss wins the game.")]
    [SerializeField] private bool isFinalBoss;

    public string DisplayName => displayName;
    public EnemyHealth Health => health;

    private readonly List<EnemyHealth> aliveSummons = new List<EnemyHealth>();
    private EnemyHealth health;
    private EnemyAI enemyAI;
    private EnemyPathfinding pathfinding;
    private Phase currentPhase;
    private bool isEngaged;
    private float nextSummonTime;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        enemyAI = GetComponent<EnemyAI>();
        pathfinding = GetComponent<EnemyPathfinding>();

        health.HealthChanged += OnHealthChanged;
        health.Died += OnDied;
        enemyAI.PlayerDetected += OnPlayerDetected;
    }

    private void Start()
    {
        EnterPhase(phaseOne);
    }

    private void OnDestroy()
    {
        health.HealthChanged -= OnHealthChanged;
        health.Died -= OnDied;
        enemyAI.PlayerDetected -= OnPlayerDetected;

        if (isEngaged)
        {
            Disengaged?.Invoke(this);
        }
    }

    private void Update()
    {
        if (!isEngaged || PlayerHealth.Instance.IsDead || Time.time < nextSummonTime) { return; }

        SummonWave();
        ScheduleNextSummon();
    }

    public void Attack()
    {
        if (currentPhase == null || currentPhase.attacks.Count == 0) { return; }

        PhaseAttack chosen = currentPhase.attacks[Random.Range(0, currentPhase.attacks.Count)];

        if (chosen.attacker is IPatternAttacker patternAttacker)
        {
            patternAttacker.AttackWith(chosen.pattern);
        }
    }

    private void OnPlayerDetected()
    {
        if (isEngaged) { return; }

        isEngaged = true;
        ScheduleNextSummon();
        Engaged?.Invoke(this);
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (currentPhase == phaseOne && currentHealth <= maxHealth * phaseTwoHealthFraction)
        {
            EnterPhase(phaseTwo);
            ScheduleNextSummon();
        }
    }

    private void EnterPhase(Phase phase)
    {
        currentPhase = phase;
        pathfinding.SetMoveSpeed(phase.moveSpeed);
        enemyAI.SetAttackCooldown(phase.attackCooldown);
    }

    private void ScheduleNextSummon()
    {
        nextSummonTime = Time.time + Random.Range(currentPhase.summonInterval.x, currentPhase.summonInterval.y);
    }

    private void SummonWave()
    {
        aliveSummons.RemoveAll(summon => summon == null);

        foreach (Summon summon in summons)
        {
            int count = Random.Range(summon.minCount, Mathf.Max(summon.minCount, summon.maxCount) + 1);

            for (int i = 0; i < count && aliveSummons.Count < maxAliveSummons; i++)
            {
                SpawnSummon(summon.prefab);
            }
        }
    }

    private void SpawnSummon(GameObject prefab)
    {
        float distance = Random.Range(summonDistance.x, summonDistance.y);
        Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle.normalized * distance;
        GameObject summoned = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (summoned.TryGetComponent(out PersistentObject persistentObject))
        {
            persistentObject.ExcludeFromWorldState();
        }

        if (summoned.TryGetComponent(out EnemyHealth summonHealth))
        {
            aliveSummons.Add(summonHealth);
        }
    }

    private void OnDied()
    {
        DropWeapon();

        foreach (EnemyHealth summon in aliveSummons)
        {
            if (summon != null) { summon.Kill(); }
        }

        if (isFinalBoss)
        {
            FinalBossDefeated?.Invoke();
        }
    }

    private void DropWeapon()
    {
        if (droppedWeapon == null) { return; }

        Vector2 position = transform.position;
        string sceneName = SceneManager.GetActiveScene().name;
        WorldState.AddDroppedWeapon(new DroppedWeaponRecord(sceneName, droppedWeapon, position, true), position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, summonDistance.x);
        Gizmos.DrawWireSphere(transform.position, summonDistance.y);
    }
}
