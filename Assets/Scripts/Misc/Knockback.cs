using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool GettingKnockedBack { get; private set; }

    [SerializeField] private float knockBackTime = .2f;

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Vector2 sourcePosition, float knockbackForce) {
        GettingKnockedBack = true;
        Vector2 difference = ((Vector2)transform.position - sourcePosition).normalized * knockbackForce * rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    public void ResetKnockback() {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        GettingKnockedBack = false;
    }

    private IEnumerator KnockRoutine() {
        yield return new WaitForSeconds(knockBackTime);
        rb.linearVelocity = Vector2.zero;
        GettingKnockedBack = false;
    }
}
