using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// A lightning strike at this object's position: a few jagged bolts fall from above onto the impact point,
// re-randomize a few times (flicker) and fade out together with a 2D light flash. Destroys itself when done.
public class LightningStrikeVFX : MonoBehaviour
{
    [Header("Bolts")]
    [Tooltip("Use an unlit sprite material so the scene's 2D lighting does not darken the bolts.")]
    [SerializeField] private Material boltMaterial;
    [SerializeField] private Color boltColor = new Color(0.75f, 0.9f, 1f, 1f);
    [Min(1)] [SerializeField] private int minBolts = 3;
    [Min(1)] [SerializeField] private int maxBolts = 5;
    [Tooltip("How far above the impact point the bolts start.")]
    [SerializeField] private float boltHeight = 6f;
    [Tooltip("Horizontal spread of the bolts' start points.")]
    [SerializeField] private float startSpread = 1.5f;
    [Min(2)] [SerializeField] private int pointsPerBolt = 10;
    [Tooltip("Maximum sideways offset of each point along a bolt.")]
    [SerializeField] private float jaggedness = 0.4f;
    [SerializeField] private float boltWidth = 0.12f;
    [SerializeField] private int sortingOrder = 20;

    [Header("Timing")]
    [SerializeField] private float duration = 0.35f;
    [Tooltip("How many times the bolts are re-randomized during the strike.")]
    [Min(1)] [SerializeField] private int flickerCount = 3;

    [Header("Light flash")]
    [SerializeField] private Light2D flashLight;
    [SerializeField] private float flashIntensity = 3f;

    private readonly List<LineRenderer> bolts = new List<LineRenderer>();

    private void Start()
    {
        int boltCount = Random.Range(minBolts, Mathf.Max(minBolts, maxBolts) + 1);

        for (int i = 0; i < boltCount; i++)
        {
            bolts.Add(CreateBolt());
        }

        StartCoroutine(StrikeRoutine());
    }

    private IEnumerator StrikeRoutine()
    {
        float flickerInterval = duration / flickerCount;
        float timeUntilFlicker = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (timeUntilFlicker <= 0f)
            {
                bolts.ForEach(RandomizeBolt);
                timeUntilFlicker = flickerInterval;
            }

            float remaining = 1f - elapsed / duration;
            ApplyFade(remaining);

            elapsed += Time.deltaTime;
            timeUntilFlicker -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private LineRenderer CreateBolt()
    {
        GameObject boltObject = new GameObject("Bolt");
        boltObject.transform.SetParent(transform, false);

        LineRenderer bolt = boltObject.AddComponent<LineRenderer>();
        bolt.material = boltMaterial;
        bolt.useWorldSpace = true;
        bolt.positionCount = pointsPerBolt;
        bolt.numCapVertices = 0;
        bolt.sortingOrder = sortingOrder;
        return bolt;
    }

    // From a random point above down to the impact point, with every inner point pushed sideways at random.
    private void RandomizeBolt(LineRenderer bolt)
    {
        Vector2 impactPoint = transform.position;
        Vector2 startPoint = impactPoint + new Vector2(Random.Range(-startSpread, startSpread), boltHeight);

        for (int i = 0; i < pointsPerBolt; i++)
        {
            float progress = (float)i / (pointsPerBolt - 1);
            Vector2 point = Vector2.Lerp(startPoint, impactPoint, progress);

            bool isEndPoint = i == 0 || i == pointsPerBolt - 1;
            if (!isEndPoint)
            {
                point.x += Random.Range(-jaggedness, jaggedness);
            }

            bolt.SetPosition(i, point);
        }
    }

    private void ApplyFade(float remaining)
    {
        Color color = new Color(boltColor.r, boltColor.g, boltColor.b, boltColor.a * remaining);

        foreach (LineRenderer bolt in bolts)
        {
            bolt.startColor = color;
            bolt.endColor = color;
            bolt.widthMultiplier = boltWidth * remaining;
        }

        if (flashLight != null)
        {
            flashLight.intensity = flashIntensity * remaining;
        }
    }
}
