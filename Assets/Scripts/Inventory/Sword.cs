using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;    
    [SerializeField] private float swordAttackCD = 0.35f;
        
    private Animator animator;
    private Transform weaponCollider;
    private GameObject slashAnim;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // safe-get weapon collider from PlayerController
        if (PlayerController.Instance != null)
            weaponCollider = PlayerController.Instance.GetWeaponCollider();
        else
            Debug.LogWarning("PlayerController.Instance is null in Sword.Start.");

             // Prefer inspector value, then ask PlayerController, then fallback-create
        if (slashAnimSpawnPoint == null)
        {
            if (PlayerController.Instance != null)
            {
                slashAnimSpawnPoint = PlayerController.Instance.GetSlashAnimSpawnPoint();
            }

            if (slashAnimSpawnPoint == null)
            {
                var found = GameObject.Find("slashAnimSpawnPoint");
                if (found != null)
                {
                    slashAnimSpawnPoint = found.transform;
                }
                else
                {
                    Debug.LogWarning("slashAnimSpawnPoint not assigned and not found. Creating fallback spawn point.");
                    var go = new GameObject("slashAnimSpawnPoint_AUTO");
                    if (transform.parent != null)
                        go.transform.SetParent(transform.parent, false);
                    else
                        go.transform.SetParent(transform, false);
                    go.transform.localPosition = Vector3.zero;
                    slashAnimSpawnPoint = go.transform;
                }
            }
        }
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    public void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (weaponCollider != null && weaponCollider.gameObject != null)
            weaponCollider.gameObject.SetActive(true);
        else
            Debug.LogWarning("weaponCollider is null when starting Attack.");

        if (slashAnimPrefab != null && slashAnimSpawnPoint != null)
        {
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
        }
        else
        {
            Debug.LogWarning("slashAnimPrefab or slashAnimSpawnPoint is not assigned; skipping slash animation spawn.");
            slashAnim = null;
        }

        StartCoroutine(AttackCDRoutine());
    }

    private void DoneAttackingAnimEvent()
    {
        if (weaponCollider != null && weaponCollider.gameObject != null)
            weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent()
    {
        if (slashAnim == null)
            return;

        slashAnim.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance != null && PlayerController.Instance.FacingLeft)
        {
            var sr = slashAnim.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = true;
        }
    }

    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }

    public void SwingDownFlipAnimEvent()
    {
        if (slashAnim == null)
            return;

        slashAnim.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (PlayerController.Instance != null && PlayerController.Instance.FacingLeft)
        {
            var sr = slashAnim.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = true;
        }
    }

    private void MouseFollowWithOffset()
    {
        if (Camera.main == null || PlayerController.Instance == null || ActiveWeapon.Instance == null)
            return;

        Vector3 mousePosition = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(Mathf.Abs(mousePosition.y - playerScreenPoint.y), Mathf.Abs(mousePosition.x - playerScreenPoint.x)) * Mathf.Rad2Deg;

        if (ActiveWeapon.Instance.transform == null)
            return;

        if (mousePosition.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            if (weaponCollider != null) weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (weaponCollider != null) weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
