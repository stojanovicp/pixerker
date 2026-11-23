using System.Collections;
using UnityEngine;

// A lobbed projectile: arcs to a landing position with a ground shadow, then spawns its splatter.
// The thrower must call Initialize right after Instantiate.
public class FallingProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private GameObject grapeProjectileShadow;
    [SerializeField] private GameObject splatterPrefab;
    [SerializeField] private float shadowOffsetY = -0.3f;

    private Vector2 landingPosition;
    private bool isInitialized;

    public void Initialize(Vector2 targetLandingPosition)
    {
        landingPosition = targetLandingPosition;
        isInitialized = true;
    }

    private void Start()
    {
        if (!isInitialized)
        {
            Debug.LogError($"{nameof(FallingProjectile)} on '{name}' was spawned without Initialize.", this);
            Destroy(gameObject);
            return;
        }

        StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        Vector2 shadowStart = transform.position + new Vector3(0f, shadowOffsetY, 0f);
        GameObject shadow = Instantiate(grapeProjectileShadow, shadowStart, Quaternion.identity);

        // The shadow follows the same timing on the ground (no height).
        StartCoroutine(ArcMovement.Move(shadow.transform, shadowStart, landingPosition, animationCurve, 0f, duration));
        yield return ArcMovement.Move(transform, transform.position, landingPosition, animationCurve, heightY, duration);

        Destroy(shadow);
        Instantiate(splatterPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
