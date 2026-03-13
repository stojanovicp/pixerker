using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 22f;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 13f;

    private Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
    }

    public void MoveProjectile()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (!collision.isTrigger && (enemyHealth || playerHealth))
        {
            if(playerHealth && isEnemyProjectile)
            {
                playerHealth.TakeDamage(1, collision.transform);
            }
            else if(enemyHealth && !isEnemyProjectile)
            {
                enemyHealth.TakeDamage(1);
            }
            Instantiate(particleOnHitPrefabVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }

    private void DetectFireDistance()
    {
               if (Vector3.Distance(transform.position, startPos) >= projectileRange)
        {
            Destroy(gameObject);
        }
    }
}
