using UnityEngine;

// On a boss arena's AreaExit: the exit trigger stays off while the boss is alive and opens when it dies.
// A boss killed earlier is already deactivated on load (PersistentObject), so the exit opens straight away.
[RequireComponent(typeof(Collider2D))]
public class BossExitGate : MonoBehaviour
{
    [SerializeField] private EnemyHealth boss;

    private Collider2D exitTrigger;

    private void Awake()
    {
        exitTrigger = GetComponent<Collider2D>();
    }

    private void Start()
    {
        bool bossIsAlive = boss != null && boss.gameObject.activeInHierarchy;

        if (!bossIsAlive)
        {
            Open();
            return;
        }

        exitTrigger.enabled = false;
        boss.Died += Open;
    }

    private void OnDestroy()
    {
        if (boss != null)
        {
            boss.Died -= Open;
        }
    }

    private void Open()
    {
        exitTrigger.enabled = true;
    }
}
