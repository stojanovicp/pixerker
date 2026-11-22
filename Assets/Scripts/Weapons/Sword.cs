using UnityEngine;

// Runs after PlayerController (default order 0) so the collider flip reads this frame's FacingLeft.
[DefaultExecutionOrder(1)]
public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private WeaponInfo weaponInfo;

    private Transform weaponCollider;
    private DamageSource weaponDamageSource;
    private Animator myAnimator;
    private GameObject slashAnim;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        weaponDamageSource = weaponCollider.GetComponent<DamageSource>();
        slashAnimSpawnPoint = GameObject.Find("SlashAnimationSpawnPoint").transform;
    }

    private void Update() {
        FlipWeaponColliderToFacing();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void Attack() {
        myAnimator.SetTrigger("Attack");
        weaponDamageSource.Initialize(weaponInfo);
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
    }

    public void DoneAttackingAnimEvent() {
        weaponCollider.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if (weaponCollider) { weaponCollider.gameObject.SetActive(false); }
    }

    public void SwingUpFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft) { 
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void FlipWeaponColliderToFacing() {
        float yRotation = PlayerController.Instance.FacingLeft ? WeaponAim.MIRRORED_Y_ROTATION : 0f;
        weaponCollider.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
