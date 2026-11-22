using System.Collections;
using UnityEngine;

// Moves a transform from start to end along a straight line, lifted by a height curve (0..1 over the move).
public static class ArcMovement
{
    public static IEnumerator Move(Transform mover, Vector2 start, Vector2 end, AnimationCurve heightCurve, float height, float duration)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;

            float linearT = timePassed / duration;
            float currentHeight = Mathf.Lerp(0f, height, heightCurve.Evaluate(linearT));

            mover.position = Vector2.Lerp(start, end, linearT) + new Vector2(0f, currentHeight);

            yield return null;
        }
    }
}
